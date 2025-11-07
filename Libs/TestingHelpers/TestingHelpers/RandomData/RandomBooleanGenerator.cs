using System;

namespace TestingHelpers.RandomData;

public static class RandomBooleanGenerator
{
    public static bool Generate() => new Random().Next(2) == 1;

}//Cls