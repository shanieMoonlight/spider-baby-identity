using ID.Application.AppAbs.TrustedDevices;
using ID.Application.JWT;
using ID.Domain.Claims.AuthMethods;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandler(
    IDeviceTrustService<AppUser> _deviceTrustService,
    IJwtRefreshTokenService<AppUser> _refreshTokenService)
    : IIdCommandHandler<TrustDeviceCmd, TrustDeviceCreateResponseDto>
{
    public async Task<GenResult<TrustDeviceCreateResponseDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        var dto = request.Dto;

        var addResult = await _deviceTrustService.TrustAsync(
            user: user,
            deviceFingerprint: dto.DeviceFingerprint,
            deviceName: dto.DeviceName,
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustDeviceCreateResponseDto>();

        var device = addResult.Value!;
        var tokenGenerationDto = await _refreshTokenService.GenerateAndStoreWithDeviceAsync(
             user,
             [AuthMethodRef.mfa],
             device,
             cancellationToken);


        var responseDto = new TrustDeviceCreateResponseDto(device.ToDto(), tokenGenerationDto.ClientToken);

        return GenResult<TrustDeviceCreateResponseDto>.Success(responseDto);
    }


}//Cls
