using System.Collections;
using System;

namespace ZKit
{
    public static class EnumHelper
    {
        /// <summary>
        /// 문자열을 Enum 값으로 변환한다.
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="s">문자열</param>
        /// <returns>변환된 Enum 값</returns>
        public static T Parse<T>(string s)
            where T : struct, IFormattable, IConvertible, IComparable // enum
        {
            return (T)System.Enum.Parse(typeof(T), s);
        }

        /// <summary>
        /// 특정 값을 Enum 값으로 변환한다.
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <typeparam name="V">변환을 원하는 값 타입</typeparam>
        /// <param name="value">변환을 원하는 값</param>
        /// <returns>변환된 Enum 값</returns>
        public static T Parse<T, V>(V value)
            where T : struct, IFormattable, IConvertible, IComparable // enum
            where V : struct
        {
            return (T)System.Enum.ToObject(typeof(T), value);
        }
    }
}