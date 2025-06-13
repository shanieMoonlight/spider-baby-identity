// In GlobalSuppressions.cs
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CS8625:Cannot convert null literal to non-nullable reference type.", Justification = "Temporarily suppressing CS8625 during NRT migration.", Scope = "module")]
[assembly: SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Testing null values where not expected", Scope = "module")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~M:ID.Infrastructure.Tests.Jobs.Service.HF.Recurring.AHfRecurringJobMgrTests.TestJobHandler.DoWorkAsync~System.Threading.Tasks.Task")]
