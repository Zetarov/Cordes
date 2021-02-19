using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class CutableTrailRenderer : MonoBehaviour
{
    public TrailRenderer TrailRenderer
    {
        get => _trailRenderer;
    }

    public float Time {
        get => _time;
        set
        {
            _time = value;
            if (_shortenCoroutine == null)
                SyncTime();
        }
    }

    private TrailRenderer _trailRenderer;
    Coroutine _shortenCoroutine = null;
    float _time;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _time = _trailRenderer.time;
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
        float startTime = UnityEngine.Time.timeSinceLevelLoad;
        float endTime = startTime + duration;

        while (UnityEngine.Time.timeSinceLevelLoad < endTime)
        {
            float progress = Mathf.InverseLerp(startTime, endTime, UnityEngine.Time.timeSinceLevelLoad);
            _trailRenderer.time = Mathf.Lerp(begin, end, progress);
            yield return new WaitForEndOfFrame();
        }

        if (endEmitting)
        {
            _trailRenderer.emitting = false;
        }

        SyncTime();
        _shortenCoroutine = null;
    }

    public void StartEmitting()
    {
        if(_shortenCoroutine != null)
        {
            StopCoroutine(_shortenCoroutine);
        }
        _trailRenderer.emitting = true;
        SyncTime();
    }

    public void StopEmission()
    {
        Shorten(0f, endEmitting: true);
    }

    private void SyncTime()
    {
        _trailRenderer.time = _time;
    }
}
