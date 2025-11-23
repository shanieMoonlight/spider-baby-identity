
using System;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using System.Collections;
using Microsoft.CSharp;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class TrustedDeviceDataFactory
{
    private static int _counter = 0;

    //--------------------------// 

    public static List<TrustedDevice> CreateMany(int count = 20) =>
        [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];

    //- - - - - - - - - - - - - //

    public static TrustedDevice Create(
        Guid? id = null,
        Guid? userId = null,
        AppUser? user = null,
        string? deviceFingerprint = null,
        string? name = null,
        string? userAgent = null,
        DateTime? trustedUntil = null,
        DateTime? lastUsedDate = null,
        string? administratorUsername = null,
        string? administratorId = null,
        DateTime? dateCreated = null,
        DateTime? lastModifiedDate = null
        )
    {

        userId ??= user?.Id ?? Guid.NewGuid();
        deviceFingerprint ??= $"{RandomStringGenerator.Generate(20)}";
        name ??= $"{RandomStringGenerator.Generate(20)}";
        userAgent ??= $"{RandomStringGenerator.Generate(20)}";
        id ??= Guid.NewGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}";

        var paramaters = new[]
            {
                         new PropertyAssignment(nameof(TrustedDevice.UserId),  () => userId ),
                  new PropertyAssignment(nameof(TrustedDevice.User),  () => user ),
                  new PropertyAssignment(nameof(TrustedDevice.Fingerprint),  () => deviceFingerprint ),
                  new PropertyAssignment(nameof(TrustedDevice.Name),  () => name ),
                  new PropertyAssignment(nameof(TrustedDevice.UserAgent),  () => userAgent ),
                  new PropertyAssignment(nameof(TrustedDevice.TrustedUntil),  () => trustedUntil ),
                  new PropertyAssignment(nameof(TrustedDevice.LastUsedDate),  () => lastUsedDate ),
                  new PropertyAssignment(nameof(TrustedDevice.Id),  () => id ),
                  new PropertyAssignment(nameof(TrustedDevice.AdministratorUsername),  () => administratorUsername ),
                  new PropertyAssignment(nameof(TrustedDevice.AdministratorId),  () => administratorId ),
                  new PropertyAssignment(nameof(TrustedDevice.DateCreated),  () => dateCreated ),
                  new PropertyAssignment(nameof(TrustedDevice.LastModifiedDate),  () => lastModifiedDate )
            };

        return ConstructorInvoker.CreateNoParamsInstance<TrustedDevice>(paramaters);
    }

    //--------------------------// 

    public static TrustedDevice Update(
           TrustedDevice trustedDevice,
           Guid? id = null,
         Guid? userId = null,
         AppUser? user = null,
         string? deviceFingerprint = null,
         string? name = null,
         string? userAgent = null,
         DateTime? trustedUntil = null,
         DateTime? lastUsedDate = null,
         string? administratorUsername = null,
         string? administratorId = null,
         DateTime? dateCreated = null,
         DateTime? lastModifiedDate = null
         )
    {

        List<PropertyAssignment> propertAssignments = [];



        if (userId is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.UserId), () => userId));

        if (user is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.User), () => user));

        if (deviceFingerprint is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.Fingerprint), () => deviceFingerprint));

        if (name is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.Name), () => name));

        if (userAgent is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.UserAgent), () => userAgent));

        if (trustedUntil is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.TrustedUntil), () => trustedUntil));

        if (lastUsedDate is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.LastUsedDate), () => lastUsedDate));

        if (id is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.Id), () => id));

        if (administratorUsername is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.AdministratorUsername), () => administratorUsername));

        if (administratorId is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.AdministratorId), () => administratorId));

        if (dateCreated is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.DateCreated), () => dateCreated));

        if (lastModifiedDate is not null)
            propertAssignments.Add(new PropertyAssignment(nameof(TrustedDevice.LastModifiedDate), () => lastModifiedDate));


        return PrivatePropertyUpdater.UpdateProperties(trustedDevice, [.. propertAssignments]);
    }



}//Cls

