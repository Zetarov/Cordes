using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRecord
{
    public Vector3 Point;
    public float Time;

    public PointRecord(Vector3 point, float time)
    {
        this.Point = point;
        this.Time = time;
    }

    public PointRecord(Vector3 point = new Vector3())
    {
        this.Point = point;
        this.Time = UnityEngine.Time.timeSinceLevelLoad;
    }
}
