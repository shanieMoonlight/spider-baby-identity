using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ID.Persistence.Ef.Interceptors;

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

        // Normalize both Local and Unspecified to UTC according to policy
        DateTime normalized = dt.Kind switch
        {
            DateTimeKind.Local => DateTime.SpecifyKind(dt.ToUniversalTime(), DateTimeKind.Utc),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            _ => dt
        };

        // 1) Try backing field
        TryApplyToBackingField(prop, normalized, out var actualFromField);
        if (VerifyDateTimeApplied(normalized, actualFromField))
            return;

        // 2) Try tracked value
        if (TryApplyToTrackedValue(prop, normalized) && VerifyDateTimeApplied(normalized, prop.CurrentValue))
            return;

        // 3) Try CLR setter (public) and verify; if setter doesn't persist expected value, force tracked value
        if (TryApplyToClrSetter(prop, normalized, out var actualFromProp))
        {
            if (VerifyDateTimeApplied(normalized, actualFromProp))
                return;

            prop.CurrentValue = normalized;
            prop.IsModified = true;
            _logger.LogWarning("Setter didn't persist expected UTC for {Property}; forced tracked value.", prop.Metadata.Name);
            return;
        }

        // As a last resort ensure EF persists a UTC value via fallback
        ConvertByAddingTick(prop);
    }

    //- - - - - - - - - - - - - - - // 

    private void HandleNullableDateTimeProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTime dtNullable)
            return;

        // Early-return if already UTC
        if (dtNullable.Kind == DateTimeKind.Utc)
            return;

        // Normalize Local and Unspecified to UTC (policy: Unspecified treated as UTC)
        DateTime? normalized = dtNullable.Kind switch
        {
            DateTimeKind.Local => DateTime.SpecifyKind(dtNullable.ToUniversalTime(), DateTimeKind.Utc),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dtNullable, DateTimeKind.Utc),
            _ => dtNullable
        };

        TryApplyToBackingField(prop, normalized, out var actualFromField);
        if (VerifyNullableDateTimeApplied(normalized, actualFromField))
            return;

        if (TryApplyToTrackedValue(prop, normalized) && VerifyNullableDateTimeApplied(normalized, prop.CurrentValue))
            return;

        if (TryApplyToClrSetter(prop, normalized, out var actualFromProp))
        {
            if (VerifyNullableDateTimeApplied(normalized, actualFromProp))
                return;

            prop.CurrentValue = (DateTime?)normalized;
            prop.IsModified = true;
            _logger.LogWarning("Setter didn't persist expected UTC for {Property}; forced tracked value.", prop.Metadata.Name);
            return;
        }

        ConvertByAddingTick(prop);
    }

    //- - - - - - - - - - - - - - - // 

    private void HandleDateTimeOffsetProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTimeOffset dto)
            return;

        // Early-return if already UTC offset
        if (dto.Offset == TimeSpan.Zero)
            return;

        // Normalize to UTC offset (offset = 0)
        var utc = dto.ToUniversalTime();

        TryApplyToBackingField(prop, utc, out var actualFieldDto);
        if (VerifyDateTimeOffsetApplied(utc, actualFieldDto))
            return;

        if (TryApplyToTrackedValue(prop, utc) && VerifyDateTimeOffsetApplied(utc, prop.CurrentValue))
            return;

        if (TryApplyToClrSetter(prop, utc, out var actualPropDto))
        {
            if (VerifyDateTimeOffsetApplied(utc, actualPropDto))
                return;

            prop.CurrentValue = utc;
            prop.IsModified = true;
            _logger.LogWarning("Setter didn't persist expected UTC offset for {Property}; forced tracked value.", prop.Metadata.Name);
            return;
        }

        // fallback
        prop.CurrentValue = utc;
        prop.IsModified = true;
    }

    //- - - - - - - - - - - - - - - // 

    private void HandleNullableDateTimeOffsetProperty(PropertyEntry prop)
    {
        if (prop.CurrentValue is not DateTimeOffset dtoNullable)
            return;

        // Early-return if already UTC offset
        if (dtoNullable.Offset == TimeSpan.Zero)
            return;

        var utc = dtoNullable.ToUniversalTime();

        TryApplyToBackingField(prop, (DateTimeOffset?)utc, out var actualFieldDto);
        if (VerifyNullableDateTimeOffsetApplied((DateTimeOffset?)utc, actualFieldDto))
            return;

        if (TryApplyToTrackedValue(prop, (DateTimeOffset?)utc) && VerifyNullableDateTimeOffsetApplied((DateTimeOffset?)utc, prop.CurrentValue))
            return;

        if (TryApplyToClrSetter(prop, (DateTimeOffset?)utc, out var actualPropDto))
        {
            if (VerifyNullableDateTimeOffsetApplied((DateTimeOffset?)utc, actualPropDto))
                return;

            prop.CurrentValue = (DateTimeOffset?)utc;
            prop.IsModified = true;
            _logger.LogWarning("Setter didn't persist expected UTC offset for {Property}; forced tracked value.", prop.Metadata.Name);
            return;
        }

        prop.CurrentValue = (DateTimeOffset?)utc;
        prop.IsModified = true;
    }

    //--------------------------// 

    // Try to set backing field; sets 'actual' to the backing value read back (or null)
    private static void TryApplyToBackingField(PropertyEntry prop, object value, out object? actual)
    {
        actual = null;
        var entry = prop.EntityEntry;
        var fi = prop.Metadata.FieldInfo;
        if (fi is null)
            return;

        try
        {
            fi.SetValue(entry.Entity, value);
            actual = fi.GetValue(entry.Entity);
            prop.IsModified = true;
        }
        catch
        {
            // ignore
        }
    }

    // Try to set EF tracked value
    private static bool TryApplyToTrackedValue(PropertyEntry prop, object value)
    {
        try
        {
            prop.CurrentValue = value;
            prop.IsModified = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Try to set CLR setter if public; returns actual read-back value via out parameter
    private static bool TryApplyToClrSetter(PropertyEntry prop, object value, out object? actual)
    {
        actual = null;
        var entry = prop.EntityEntry;
        var pi = prop.Metadata.PropertyInfo;
        if (pi is null || pi.SetMethod is null || !pi.SetMethod.IsPublic)
            return false;

        try
        {
            pi.SetValue(entry.Entity, value);
            actual = pi.GetValue(entry.Entity);
            entry.Property(prop.Metadata.Name).IsModified = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Verification helpers
    private static bool VerifyDateTimeApplied(DateTime expectedUtc, object? actual)
    {
        if (actual is not DateTime a)
            return false;
        return a.Kind == DateTimeKind.Utc && a.Ticks == expectedUtc.Ticks;
    }

    private static bool VerifyNullableDateTimeApplied(DateTime? expectedUtc, object? actual)
    {
        if (expectedUtc is null)
            return actual is null;
        return VerifyDateTimeApplied(expectedUtc.Value, actual);
    }

    private static bool VerifyDateTimeOffsetApplied(DateTimeOffset expectedUtc, object? actual)
    {
        if (actual is not DateTimeOffset a)
            return false;
        return a.Offset == TimeSpan.Zero && a.UtcDateTime.Ticks == expectedUtc.UtcDateTime.Ticks;
    }

    private static bool VerifyNullableDateTimeOffsetApplied(DateTimeOffset? expectedUtc, object? actual)
    {
        if (expectedUtc is null)
            return actual is null;
        return VerifyDateTimeOffsetApplied(expectedUtc.Value, actual);
    }

    //--------------------------// 

    /// <summary>
    /// Ensure the tracked property's value is treated as UTC and force EF to persist the change.
    /// (unchanged)
    /// </summary>
    private void ConvertByAddingTick(PropertyEntry prop)
    {
        var entry = prop.EntityEntry;

        // Handle DateTime and nullable DateTime
        if (prop.CurrentValue is not DateTime dt)
            return;

        var utcPreserve = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

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







