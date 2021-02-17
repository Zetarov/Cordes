using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRopeDispenserEventListener : RopeDispenserEventListener
{
    TrailRenderer _trailRenderer;
    float _time;

    Coroutine _shortenCoroutine = null;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _time = _trailRenderer.time;
    }

    public override void OnActivatedChanged(bool on)
    {
        base.OnActivatedChanged(on);
        if(on)
        {
            _trailRenderer.emitting = true;
        }
        else
        {
            Shorten(0f, endEmitting: true);
        }
    }

    public override void OnClosedContour(float timeSinceLevelLoad)
    {
        base.OnClosedContour(timeSinceLevelLoad);
        float timeTruncate = Time.timeSinceLevelLoad - timeSinceLevelLoad;
        Shorten(timeTruncate);
    }

    public override void OnRopeDurationChanged(float time)
    {
        base.OnRopeDurationChanged(time);
        _time = time;
        SyncTime();
    }

    public void Shorten(float timeTruncate, bool endEmitting = false)
    {
        if (_shortenCoroutine != null)
            StopCoroutine(_shortenCoroutine);
        _shortenCoroutine = StartCoroutine(Shorten_Coroutine(timeTruncate, endEmitting));
    }

    private IEnumerator Shorten_Coroutine(float timeTruncate, bool endEmitting)
    {
        float begin = _trailRenderer.time;
        float end = timeTruncate;
        const float duration = 0.5f;
        float startTime = Time.timeSinceLevelLoad;
        float endTime = startTime + duration;

        while(Time.timeSinceLevelLoad < endTime)
        {
            float progress = Mathf.InverseLerp(startTime, endTime, Time.timeSinceLevelLoad);
            _trailRenderer.time = Mathf.Lerp(begin, end, progress);
            yield return new WaitForEndOfFrame();
        }

        if(endEmitting)
        {
            _trailRenderer.emitting = false;
        }

        SyncTime();
        _shortenCoroutine = null;
    }

    private void SyncTime()
    {
        _trailRenderer.time = _time;
    }
}
