using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleRecord<T>
{
    public T Value;
    public float Time;

    public SampleRecord(T value, float time)
    {
        this.Value = value;
        this.Time = time;
    }

    public SampleRecord(T value)
    {
        this.Value = value;
        this.Time = UnityEngine.Time.timeSinceLevelLoad;
    }
}
