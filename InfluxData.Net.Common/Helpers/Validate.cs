using System;
using System.Collections.Generic;

namespace InfluxData.Net.Common.Helpers
{
    /// <summary>
    /// Validation class which throws Argument exceptions if checks fail.
    /// </summary>
    public static class Validate
    {
        public static void IsNotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNotNull<T>(T value, string paramName, string message) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName, message);
        }

        public static void IsFalse(bool value, string message)
        {
            if (value)
                throw new ArgumentException(message);
        }

        public static void IsTrue(bool value, string message)
        {
            if (!value)
                throw new ArgumentException(message);
        }

        public static void IsNotNullOrEmpty(string value, string paramName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException(paramName);
        }

        public static void IsNotZeroLength<T>(T[] array, string paramName)
        {
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        public static void IsNotZeroLength<T>(T[] array, string paramName, string message)
        {
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        public static void IsNotZeroLength<T>(List<T> list, string paramName)
        {
            if (list.Count == 0)
                throw new ArgumentOutOfRangeException(paramName);
        }
    }
}