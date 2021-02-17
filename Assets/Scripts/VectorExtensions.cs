using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 XZ(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }
}
