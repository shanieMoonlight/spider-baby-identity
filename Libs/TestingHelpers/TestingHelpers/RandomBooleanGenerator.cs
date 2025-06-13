using System;

namespace TestingHelpers;

public static class RandomBooleanGenerator
{
    public static bool Generate() => new Random().Next(2) == 1;

}//Cls