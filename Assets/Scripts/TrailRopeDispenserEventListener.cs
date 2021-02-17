using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRopeDispenserEventListener : RopeDispenserEventListener
{
    TrailRenderer _trailRenderer;
    float _time;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _time = _trailRenderer.time;
    }

    public override void OnActivatedChanged(bool on)
    {
        base.OnActivatedChanged(on);
        _trailRenderer.emitting = on;
    }

    public override void OnClosedContour(float timeSinceLevelLoad)
    {
        base.OnClosedContour(timeSinceLevelLoad);
        float timeTruncate = Time.timeSinceLevelLoad - timeSinceLevelLoad;
        _trailRenderer.time = timeTruncate;
        Invoke("SyncTime", 0.1f);
    }

    public override void OnRopeDurationChanged(float time)
    {
        base.OnRopeDurationChanged(time);
        _time = time;
        SyncTime();
    }

    private void SyncTime()
    {
        _trailRenderer.time = _time;
    }
}
