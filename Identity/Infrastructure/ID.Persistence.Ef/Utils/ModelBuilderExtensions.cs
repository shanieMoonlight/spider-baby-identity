using ID.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID.Persistence.Ef.Utils;
public static class ModelBuilderExtensions
{
    public static void ApplyClientSideIdGeneration(this ModelBuilder modelBuilder)
    {
        // 1. Iterate over all entities in the model
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 2. Check if the current entity inherits from IdDomainEntity
            if (typeof(IdDomainEntity).IsAssignableFrom(entityType.ClrType))
                // 3. Configure the Id property to never generate values from the DB
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IdDomainEntity.Id))
                    .ValueGeneratedNever();

        }
    }

}//Cls