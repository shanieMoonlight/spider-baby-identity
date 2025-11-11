using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ID.Persistence.Ef.Persistance.EF.Interceptors;

public sealed class DateTimeNormalizationSaveChangesInterceptor(ILogger<DateTimeNormalizationSaveChangesInterceptor> _logger)
    : SaveChangesInterceptor
{
    private void NormalizeDates(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var prop in entry.Properties)
            {
                var clrType = prop.Metadata.ClrType;
                if (clrType is null)
                    continue;

                if (clrType == typeof(DateTime))
                    HandleDateTimeProperty(prop);
                else if (clrType == typeof(DateTime?))
                    HandleNullableDateTimeProperty(prop);
                else if (clrType == typeof(DateTimeOffset))
                    HandleDateTimeOffsetProperty(prop);
                else if (clrType == typeof(DateTimeOffset?))
                    HandleNullableDateTimeOffsetProperty(prop);
            }
        }
    }

    //--------------------------// 

    private void HandleDateTimeProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTime dt)
            return;

        // Early-return if already UTC
        if (dt.Kind == DateTimeKind.Utc)
            return;

        if (dt.Kind == DateTimeKind.Local)
        {
            prop.CurrentValue = dt.ToUniversalTime();
            prop.IsModified = true;//Tell Ef core that things have changed
            return;
        }

        if (dt.Kind != DateTimeKind.Unspecified)
            return;

        // Unspecified: ensure the value is treated as UTC (preserve ticks if possible)
        EnsureUtcAndMarkModified(prop);
    }

    //- - - - - - - - - - - - - - - // 

    private void HandleNullableDateTimeProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTime dtNullable)
            return;

        // Early-return if already UTC
        if (dtNullable.Kind == DateTimeKind.Utc)
            return;

        if (dtNullable.Kind == DateTimeKind.Local)
        {
            prop.CurrentValue = dtNullable.ToUniversalTime();
            prop.IsModified = true;//Tell Ef core that things have changed
            return;
        }

        if (dtNullable.Kind != DateTimeKind.Unspecified)
            return;

        EnsureUtcAndMarkModified(prop);
    }

    //- - - - - - - - - - - - - - - // 

    private static void HandleDateTimeOffsetProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTimeOffset dto)
            return;

        // Early-return if already UTC offset
        if (dto.Offset == TimeSpan.Zero)
            return;

        // Normalize to UTC offset (offset = 0)
        var utc = dto.ToUniversalTime();
        prop.CurrentValue = utc;
        prop.IsModified = true;//Tell Ef core that things have changed
    }

    //- - - - - - - - - - - - - - - // 

    private static void HandleNullableDateTimeOffsetProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTimeOffset dtoNullable)
            return;

        // Early-return if already UTC offset
        if (dtoNullable.Offset == TimeSpan.Zero)
            return;

        var utc = dtoNullable.ToUniversalTime();
        prop.CurrentValue = utc;
        prop.IsModified = true;//Tell Ef core that things have changed
    }

    //--------------------------// 

    /// <summary>
    /// Ensure the tracked property's value is treated as UTC and force EF to persist the change.
    ///
    /// Behavior:
    /// - Reads the current value from <c>prop.CurrentValue</c> (expected DateTime or DateTime?).
    /// - Creates a DateTime with the same ticks but Kind=Utc (preserve instant).
    /// - Attempts to assign the new value to the actual CLR property (via PropertyInfo) so the entity
    ///   instance reflects the change (covers private setters).
    /// - If that fails, attempts to assign the mapped backing field (FieldInfo).
    /// - If both CLR assignments fail, sets the EF entry's CurrentValue. Because assigning a value with
    ///   identical ticks may be ignored by EF, as a last resort we change ticks by one tick to guarantee
    ///   the entry observes a difference and will persist the value.
    ///
    /// The method always marks the EF property as modified so the value will be sent to the database.
    /// </summary>
    private void EnsureUtcAndMarkModified(PropertyEntry prop)
    {
        var entry = prop.EntityEntry;      
        

        // Handle DateTime and nullable DateTime
        if (prop.CurrentValue is not DateTime dt)
            return;

        var utcPreserve = DateTime.SpecifyKind(dt, DateTimeKind.Utc);


        // Try to set CLR property
        var pi = prop.Metadata.PropertyInfo;
        if (pi is not null && pi.CanWrite)
        {
            try
            {
                pi.SetValue(entry.Entity, utcPreserve);
                entry.Property(prop.Metadata.Name).IsModified = true;
                return;
            }
            catch
            {
                // fall through
            }
        }

        // Try backing field
        var fi = prop.Metadata.FieldInfo;
        if (fi is not null)
        {
            try
            {
                fi.SetValue(entry.Entity, utcPreserve);
                prop.IsModified = true;
                return;
            }
            catch
            {
                // fall through
            }
        }

        // Last resort: set CurrentValue; if EF ignores because ticks equal, tweak by one tick
        prop.CurrentValue = utcPreserve;
        prop.IsModified = true;

        // If EF still reports Unspecified (assignment ignored), force a tiny tick change
        if (prop.CurrentValue is DateTime after && after.Kind != DateTimeKind.Utc)
        {
            // Use parity-based tweak to avoid monotonic drift. Alternate between +1 and -1 tick deterministically.
            var tweaked = utcPreserve.Ticks % 2 == 0 
                ? utcPreserve.AddTicks(1) 
                : utcPreserve.AddTicks(-1);

            prop.CurrentValue = tweaked;
            prop.IsModified = true;
            _logger.LogWarning("[EfHelpers] Tweaked ticks for '{Property}' to force change: {Value}", prop.Metadata.Name, tweaked);
        }

        return;
    }

    //--------------------------// 

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        NormalizeDates(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    //--------------------------// 

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        NormalizeDates(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

}//Cls




