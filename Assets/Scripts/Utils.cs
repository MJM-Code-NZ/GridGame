using System;

namespace MJM
{
    public static class Utils
    {
        private static readonly Random _RandomInstance = new Random();

        public static int RandomRange(int minInclusive, int maxInclusive)
        {
            return _RandomInstance.Next(minInclusive, maxInclusive + 1);
        }

        public static T RandomEnumValue<T>()
        {
            Array valuesArray = Enum.GetValues(typeof(T));

            int randomEnumIndex = +_RandomInstance.Next(valuesArray.Length);

            return (T)valuesArray.GetValue(randomEnumIndex);
        }
    }
}
