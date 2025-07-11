﻿using ID.Domain.Utility.Exceptions;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Defaults;
using ID.Infrastructure.Auth.JWT.Utils;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Auth.JWT.Setup;


internal static class JwtOptionsSetup
{

    internal static IServiceCollection ConfigureJwtOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {

        if (setupOptions == null)
            throw new SetupDataException(nameof(ConfigureJwtOptions), nameof(IdInfrastructureSetupOptions));

        var jwtOptions = new JwtOptions();


        if (setupOptions.TokenExpirationMinutes > 0)
            jwtOptions.TokenExpirationMinutes = setupOptions.TokenExpirationMinutes;


        if (setupOptions.RefreshTokenTimeSpan <= TimeSpan.Zero)
            jwtOptions.RefreshTokenTimeSpan = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN;
        else
            jwtOptions.RefreshTokenTimeSpan = setupOptions.RefreshTokenTimeSpan;


        if (!string.IsNullOrWhiteSpace(setupOptions.TokenIssuer))
            jwtOptions.TokenIssuer = setupOptions.TokenIssuer;
        else
            jwtOptions.TokenIssuer = IdGlobalDefaultValues.TOKEN_ISSUER;

        // Configure refresh token update policy
        jwtOptions.RefreshTokenUpdatePolicy = setupOptions.RefreshTokenUpdatePolicy ?? RefreshTokenUpdatePolicy.ThreeQuarterLife;

        ConfigureSymmetricCrypto(setupOptions, jwtOptions);

        ConfigureAsymmetricCrypto(setupOptions, jwtOptions);


        services.Configure<JwtOptions>(opts => CopyOptionsValues(jwtOptions, opts));

        return services;
    }


    //--------------------------//        


    /// <summary>
    /// Configures JWT options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the JWT section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new SetupDataException(nameof(ConfigureJwtOptions), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<JwtOptions>(configSection);


        return services;
    }


    //--------------------------//

    private static void CopyOptionsValues(JwtOptions source, JwtOptions target)
    {
        target.TokenExpirationMinutes = source.TokenExpirationMinutes;
        target.SymmetricTokenSigningKey = source.SymmetricTokenSigningKey;
        target.TokenIssuer = source.TokenIssuer;
        target.SecurityAlgorithm = source.SecurityAlgorithm;
        
        target.AsymmetricAlgorithm = source.AsymmetricAlgorithm;
        target.CurrentAsymmetricKeyPair = source.CurrentAsymmetricKeyPair;
        target.LegacyAsymmetricKeyPairs = source.LegacyAsymmetricKeyPairs;
       
        target.RefreshTokenUpdatePolicy = source.RefreshTokenUpdatePolicy;
        target.RefreshTokenTimeSpan = source.RefreshTokenTimeSpan;
    }

    //--------------------------//


    private static JwtOptions ConfigureSymmetricCrypto(IdInfrastructureSetupOptions setupOptions, JwtOptions jwtOptions)
    {

        if (!string.IsNullOrWhiteSpace(setupOptions.SymmetricTokenSigningKey))
        {
            if (setupOptions.SymmetricTokenSigningKey.Length < IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH)
                throw new SetupDataException(IDMsgs.Error.Jwt.TOKEN_SIGNING_KEY_TOO_SHORT(IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH));
            else
                jwtOptions.SymmetricTokenSigningKey = setupOptions.SymmetricTokenSigningKey;
        }


        if (!string.IsNullOrWhiteSpace(setupOptions.SecurityAlgorithm))
            jwtOptions.SecurityAlgorithm = setupOptions.SecurityAlgorithm;

        return jwtOptions;

    }


    //--------------------------//


    private static JwtOptions ConfigureAsymmetricCrypto(IdInfrastructureSetupOptions setupOptions, JwtOptions jwtOptions)
    {

        //Use symmetric when available and don't bother setting or validating asymmetric keys
        if (!string.IsNullOrWhiteSpace(setupOptions.SymmetricTokenSigningKey))
            return jwtOptions;

        if (!string.IsNullOrWhiteSpace(setupOptions.AsymmetricAlgorithm))
            jwtOptions.AsymmetricAlgorithm = setupOptions.AsymmetricAlgorithm;

        jwtOptions.CurrentAsymmetricKeyPair = GetCurrentAsymmetricPemKeyPair(setupOptions);

        jwtOptions.LegacyAsymmetricKeyPairs = GetLegacyAsymmetricPemKeyPairs(setupOptions);


        return jwtOptions;
    }


    //--------------------------//


    /// <summary>
    /// Gets the asymmetric public key in PEM format, with fallback conversion from XML.
    /// </summary>
    private static AsymmetricPemKeyPair GetCurrentAsymmetricPemKeyPair(IdInfrastructureSetupOptions options)
    {
        if (options.AsymmetricPemKeyPair is not null)
            return options.AsymmetricPemKeyPair;

        if (options.AsymmetricXmlKeyPair is not null)
            return options.AsymmetricXmlKeyPair.ToPemPair();

        throw new SetupDataException(IDMsgs.Error.Setup.MISSING_ASSYMETRIC_KEY_PAIR);
    }


    //--------------------------//

    /// <summary>
    /// Gets the asymmetric public key in PEM format, with fallback conversion from XML.
    /// </summary>
    private static List<AsymmetricPemKeyPair> GetLegacyAsymmetricPemKeyPairs(IdInfrastructureSetupOptions options)
    {
        if (options.LegacyAsymmetricPemKeyPairs?.Any() == true)
            return [.. options.LegacyAsymmetricPemKeyPairs];


        if (options.LegacyAsymmetricXmlKeyPairs?.Any() == true)
            return [.. options.LegacyAsymmetricXmlKeyPairs.Select(p => p.ToPemPair())];


        return [];
    }


}//Cls
