using DelaunayVoronoi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointExtension
{
    public static Vector3 XYtoVector3XZ(this Point point)
    {
        return new Vector3(point.X, 0f, point.Y);
    }
}
