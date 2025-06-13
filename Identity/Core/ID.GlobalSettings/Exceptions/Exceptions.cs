namespace ID.GlobalSettings.Exceptions;
public class GlobalSettingMissingSetupDataException(string missingDataName)
    : Exception($"MyId.{nameof(GlobalSettings)}: Missing setup data: {missingDataName}")
{ }

public class GlobalSettingInvalidSetupDataException(string missingDataName, string reason)
    : Exception($"MyId.{nameof(GlobalSettings)}: Invalid setup data: {missingDataName}. {reason}")
{ }



