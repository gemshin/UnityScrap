using System;
using System.Reflection;
using UnityEngine;

public static class Extensions
{
    public static void FromString(this Vector2 t, ref Vector2 v, string s)
    {
        string[] tmp = s.Replace(" ", "").Split(',');
        v.x = float.Parse(tmp[0]);
        v.y = float.Parse(tmp[1]);
    }
    public static void FromString(this Vector3 t, ref Vector3 v, string s)
    {
        string[] tmp = s.Replace(" ", "").Split(',');
        v.x = float.Parse(tmp[0]);
        v.y = float.Parse(tmp[1]);
        v.z = float.Parse(tmp[2]);
    }

    public static string ToStringWithoutBracket(this Vector2 v)
    {
        return v.ToString("F3").Replace("(", "").Replace(")", "");
    }

    public static string ToStringWithoutBracket(this Vector3 v)
    {
        return v.ToString("F3").Replace("(", "").Replace(")", "");
    }

    public static void SetPositionX(this Transform t, float x)
    {
        t.position = new Vector3(x, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float y)
    {
        t.position = new Vector3(t.position.x, y, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float z)
    {
        t.position = new Vector3(t.position.x, t.position.y, z);
    }

    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    public static bool HasRigidbody(this GameObject go)
    {
        return (go.GetComponent<Rigidbody>() != null);
    }

    public static bool HasAnimation(this GameObject go)
    {
        return (go.GetComponent<Animation>() != null);
    }

    public static void SetSpeed(this Animation anim, float newSpeed)
    {
        anim[anim.clip.name].speed = newSpeed;
    }

    public static int ColorToInt(this Color c)
    {
        int retVal = 0;
        retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
        retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
        retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
        retVal |= Mathf.RoundToInt(c.a * 255f);
        return retVal;
    }

    public static string ToHexString(this Color color)
    {
        return ZKit.Math.Util.DecimalToHex32(color.ColorToInt());
    }

    public static string ToXmlColorString(this string message, Color color)
    {
        return string.Format("<color=#{0}>{1}</color>", color.ToHexString(), message);
    }
}

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