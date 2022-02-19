using FixedPoint64;
using UnityEngine;

/**
* @brief Extensions added by FixedPoint64.
**/
public static class FixedPoint64Extensions
{
    public static bool IsZero(this TSVector2 vector)
    {
        return vector == TSVector2.zero;
    }

    public static bool IsValid(this TSVector2 vector)
    {
        return vector == TSVector2.valid;
    }

    public static Vector3 ToVector(this TSVector2 v2)
    {
        var v3 = Vector3.zero;
        v3.x = v2.x.AsFloat();
        v3.z = v2.y.AsFloat();
        return v3;
    }

    public static Vector3 ToVector(this TSVector2 jVector, Vector3 v3)
    {
        v3.x = jVector.x.AsFloat();
        v3.y = 0;
        v3.z = jVector.y.AsFloat();
        return v3;
    }

    //csvector2 -> csvector2
    //public static CSVector2 ToCSVector2(this CSVector2 v1, TSVector2 v2)
    //{
    //    v1.X = v2.x.RawValue;
    //    v1.Y = v2.y.RawValue;
    //    return v1;
    //}

    //public static TSVector2 ToTSVector2(this CSVector2 v1, TSVector2 v2)
    //{
    //    v2.x = FP.FromRaw(v1.X);
    //    v2.y = FP.FromRaw(v1.Y);
    //    return v2;
    //}
}