
using System;
using MassTransit;
using TestingHelpers;
using ID.Application.Features.OutboxMessages;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Tests.Data.Factories.Dtos;

public static class OutboxMessageDtoDataFactory
{

    public static List<IdOutboxMessageDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static IdOutboxMessageDto Create(
          Guid? id = null,
        string? type = null,
        string? contentJson = null,
        string? error = null
        )
    {

        type ??= $"{RandomStringGenerator.Generate(20)}{id}";
        contentJson ??= $"{RandomStringGenerator.Generate(20)}{id}";
        error ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(IdOutboxMessage.Id),  () => id ),
        new PropertyAssignment(nameof(IdOutboxMessage.Type),  () => type ),
        new PropertyAssignment(nameof(IdOutboxMessage.ContentJson),  () => contentJson ),
        new PropertyAssignment(nameof(IdOutboxMessage.Error),  () => error )
        };


        return ConstructorInvoker.CreateNoParamsInstance<IdOutboxMessageDto>(paramaters);
    }

    //------------------------------------//

}//Cls

