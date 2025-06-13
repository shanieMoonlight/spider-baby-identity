
using System;


namespace TestingHelpers;

public static class MyRandomNumberGenerator
{
    private static readonly Random _random = new();

    //-----------------------------//

    public static int Integer(int min, int max)
    {
        lock (_random)
        {
            return _random.Next(min, max);
        }
    }
    public static int Integer(int max) => _random.Next(0, Math.Max(1, max));

    //- - - - - - - - - - - - - - -//

    public static double Double(double min, double max)
    {
        lock (_random)
        {
            return min + (_random.NextDouble() * (max - min));
        }
    }
    public static double Double(double max) => Double(0, max);

    //- - - - - - - - - - - - - - -//

    public static long Long(long min, long max)
    {
        lock (_random)
        {
            // Generate a random long using two random ints
            byte[] buf = new byte[8];
            _random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min + 1)) + min);
        }
    }

    public static long Long(long max) => Long(0, max);

    //- - - - - - - - - - - - - - -//

    public static short Short(short min, short max)
    {
        lock (_random)
        {
            // Random.Next only works with int, so we need to scale and cast
            return (short)(min + (_random.Next(max - min + 1)));
        }
    }

    public static short Short(short max) => Short(0, max);

    //- - - - - - - - - - - - - - -//

    public static decimal Decimal(decimal min, decimal max)
    {
        lock (_random)
        {
            // Scale random double to decimal range
            double range = (double)(max - min);
            double sample = _random.NextDouble();
            decimal scaled = (decimal)(range * sample);
            return min + scaled;
        }
    }
    public static decimal Decimal(decimal max) => Decimal(0, max);

    //-----------------------------//

}//Cls
