using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kaliber3D
{

    /// <summary>
    ///   A helper class to do common <see cref = "Math" /> operations.
    /// </summary>
    public static class MathHelper
    {
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        ///   Round a value to the nearest multiple of another value.
        /// </summary>
        /// <typeparam name = "T">The type of the value.</typeparam>
        /// <param name = "value">The value to round to the nearest multiple.</param>
        /// <param name = "roundToMultiple">The passed value will be rounded to the nearest multiple of this value.</param>
        /// <returns>A multiple of roundToMultiple, nearest to the passed value.</returns>
        public static T RoundToNearest<T>(T value, T roundToMultiple)
        {
            double factor = CastOperator<T, double>.Cast(roundToMultiple);
            double result = Math.Round(CastOperator<T, double>.Cast(value) / factor) * factor;
            return CastOperator<double, T>.Cast(result);
        }
    }

    /// <summary>
    ///   Allows access to cast operator for a generic type.
    /// </summary>
    /// <typeparam name="T">The type which to cast.</typeparam>
    /// <typeparam name="TResult">The type to cast to.</typeparam>
    public static class CastOperator<T, TResult>
    {
        /// <summary>
        ///   A delegate to cast the given type to the desired resulting type.
        /// </summary>
        public static Func<T, TResult> Cast { get; private set; }


        static CastOperator()
        {
            ParameterExpression arg = Expression.Parameter(typeof(T), "arg");
            Cast = Expression.Lambda<Func<T, TResult>>(Expression.Convert(arg, typeof(TResult)), arg).Compile();
        }
    }
}
