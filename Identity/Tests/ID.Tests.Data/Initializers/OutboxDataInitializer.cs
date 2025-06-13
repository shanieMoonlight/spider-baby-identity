using ID.Domain.Entities.OutboxMessages;
using ID.Tests.Data.Factories;

namespace ID.Tests.Data.Initializers;
public class OutboxDataInitializer
{
    //------------------------------------//

    public static List<IdOutboxMessage> GenerateData()
    {
        return OutboxMessageDataFactory.CreateMany(50);
    }

    //------------------------------------//

}//Cls
