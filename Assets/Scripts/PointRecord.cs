using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class PointRecord: SampleRecord<Vector3>
{
    public Vector3 Point {
        get => Value;
        set => Value = value;
    }

    public PointRecord(Vector3 point, float time) : base(point, time) { }

    public PointRecord(Vector3 point = new Vector3()): base(point) { }
}
