﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;


[assembly: SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters",
                           Justification = "Forcing null into non-null parameters for edge case testing",
                           Scope = "namespaceanddescendants",
                           Target = "~N:StringHelpers.Tests")]
