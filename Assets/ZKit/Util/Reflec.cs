using System;
using System.Reflection;

public static class Reflec
{
    /// <summary>
    /// 인스턴스의 필드값을 가져온다.
    /// </summary>
    /// <typeparam name="T">가져올 필드의 타입</typeparam>
    /// <param name="type">인스턴스의 타입</param>
    /// <param name="instance">인스턴스</param>
    /// <param name="fieldName">가져올 필드의 이름</param>
    /// <param name="nonPublic">private 필드는 false, public 필드는 true</param>
    /// <returns>필드의 값을 반환.</returns>
    public static T GetFieldValue<T>(Type type, object instance, string fieldName, bool nonPublic = true)
    {
        BindingFlags flags = BindingFlags.Instance;
        if (nonPublic) flags |= BindingFlags.NonPublic;
        else flags |= BindingFlags.Public;

        FieldInfo info = type.GetField(fieldName, flags);

        if (info == null) return default(T);

        return (T)info.GetValue(instance);
    }

    //public static object GetReferenceValue(Type type, object instance, string fieldName)
    //{
    //    BindingFlags flags = BindingFlags.Instance;
    //    flags |= BindingFlags.NonPublic;

    //    FieldInfo info = type.GetField(fieldName, flags);

    //    if (info.FieldType.IsValueType)
    //    {
    //        TypedReference typedRef = __makeref(instance);
    //        return info.GetValueDirect(typedRef);
    //    }
    //    else
    //        return info.GetValue(instance);
    //}
}