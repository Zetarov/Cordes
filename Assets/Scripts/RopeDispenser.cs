using DG.Tweening;
using Ludiq.PeekCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the points of the rope.
/// </summary>
public class RopeDispenser : MonoBehaviour
{
    [SerializeField, Tooltip("The minimum distance between two points.")]
    private float _pointStep;

    [SerializeField, Tooltip("The maximum duration the rope can reach.")]
    private float _duration = 4f;

    [SerializeField, Tooltip("The minimum distance between this and a rope point to consider the shape closed.")]
    private float _minCloseSnapDist = 0.1f;

    [SerializeField, Tooltip("Line renderer for debugging purpose, not mandatory")]
    private LineRenderer _lineRenderer;

    [SerializeField]
    private bool _isCaptureRopeDispenser = true;

    [SerializeField]
    private GameObject _magicWater = null;

    [System.NonSerialized]
    public UnityEventFloat ClosedContour = new UnityEventFloat();

    public bool Activated
    {
        get => _activated;
        set
        {
            if (_activated == value)
                return;
            _activated = value;
            if(!_activated && _isCaptureRopeDispenser)
            {
                EraseAllPoints();
            }
        }
    }

    public float Duration
    {
        get => _duration;
    }
    
    private float _pointStepSquared;
    private Queue<PointRecord> _points = new Queue<PointRecord>();
    private PointRecord _lastEnqueuedRecord = null;
    private bool _activated = false;

    void Awake()
    {
        _pointStepSquared = _pointStep * _pointStep;
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        // Search a point for closing the shape
        if(Activated)
        {
            if(_isCaptureRopeDispenser)
            {
                (int, PointRecord)? closePointRes = FindAClosePoint();
                if(closePointRes.HasValue)
                {
                    (int, PointRecord) closePoint = closePointRes.Value;
                    TestInsidePoint(startIndex: closePoint.Item1);
                    EraseAllPoints();
                    ClosedContour.Invoke(closePoint.Item2.Time);
                }
            }

            PutRopePointOrEditLastOne();
        }

        RemoveOldPoints();
        UpdateLineRenderer();
    }

    void EraseAllPoints()
    {
        _points.Clear();
        _lastEnqueuedRecord = null;
    }

    /// <summary>
    /// Put a new rope point at the current position of the object or edit the last point entered if the point is not far enough from the previous one.
    /// </summary>
    void PutRopePointOrEditLastOne()
    {
        Vector3 newPoint = transform.position;

        if(_lastEnqueuedRecord != null && (newPoint - _lastEnqueuedRecord.Point).sqrMagnitude < _pointStepSquared)
        {
            _lastEnqueuedRecord.Time = Time.timeSinceLevelLoad;
        }
        else
        {
            PointRecord newRecord = new PointRecord(newPoint, Time.timeSinceLevelLoad);
            _points.Enqueue(newRecord);
            _lastEnqueuedRecord = newRecord;
        }
    }

    /// <summary>
    /// Remove the oldest points which have exceeded the maximum duration.
    /// </summary>
    void RemoveOldPoints()
    {
        float currentTime = Time.timeSinceLevelLoad;
        while(_points.Count > 0 && currentTime - _points.Peek().Time > _duration)
        {
            _points.Dequeue();
        }
    }

    /// <summary>
    /// Updates the <see cref="_lineRenderer"/> to show the <see cref="_points"/>.
    /// </summary>
    void UpdateLineRenderer()
    {
        if (_lineRenderer == null)
            return;
        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.Select(pointRecord => pointRecord.Point).ToArray());
    }

    (int, PointRecord)? FindAClosePoint()
    {
        // Prevent for finding last put points as enclosing shape
        const int skipPointNb = 3;

        if (_points.Count <= skipPointNb)
            return null;

        Vector3 currentPos = transform.position;

        int bestCandidateIndex = -1;
        PointRecord bestCandidate = null;
        float minSqrMagnitude = float.PositiveInfinity;

        int index = 1;

        foreach(PointRecord pointRecord in _points.Take(_points.Count - skipPointNb))
        {
            float sqrMagnitude = (pointRecord.Point - currentPos).sqrMagnitude;

            if(sqrMagnitude < minSqrMagnitude)
            {
                bestCandidateIndex = index;
                bestCandidate = pointRecord;
                minSqrMagnitude = sqrMagnitude;
            }

            ++index;
        }

        bool candidateRetained = minSqrMagnitude <= _minCloseSnapDist * _minCloseSnapDist;
        return candidateRetained ? (bestCandidateIndex, bestCandidate) : null as (int, PointRecord)?;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
    }

    public static float GetRadius(IEnumerable<Vector2> points)
    {
        float minX = points.First().x;
        float minY = points.First().y;
        float maxX = minX;
        float maxY = minY;

        foreach(Vector2 point in points)
        {
            if(point.x < minX)
            {
                minX = point.x;
            }
            if(point.x > maxX)
            {
                maxX = point.x;
            }

            if (point.y < minY)
            {
                minY = point.y;
            }
            if (point.y > maxY)
            {
                maxY = point.y;
            }
        }

        return Mathf.Max(maxX - minX, maxY - minY) / 2f;
    }

    public static Vector2 GetMean(IEnumerable<Vector2> points)
    {
        return points.Aggregate(Vector2.zero,
            (sum, newEl) => sum += newEl,
            sum => sum / points.Count());
    }

    public void TestInsidePoint(int startIndex = 0)
    {
        List<Vector2> points = _points.Skip(startIndex).Select(pointRecord => new Vector2(pointRecord.Point.x, pointRecord.Point.z)).ToList();
        if (points.Count == 0)
            return;

        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Interest"))
        {
            Vector2 goPoint = new Vector2(go.transform.position.x, go.transform.position.z);
            go.GetComponent<MaterialToggle>().Toggle = PointIsInPolygon(points, goPoint);
        }

        // Pop magic water
        float radius = GetRadius(points);
        Vector2 meanPos = GetMean(points);
        GameObject magicWater = Instantiate(_magicWater, new Vector3(meanPos.x, 0f, meanPos.y), Quaternion.identity);
        magicWater.GetComponent<MagicWater>().Appear(radius);

        // Make rats die
        GameObject.FindGameObjectsWithTag("Rat")
            .Where(go => PointIsInPolygon(points, go.transform.position.XZ()))
            .ForEach(go => { 
                go.transform
                    .DOMoveY(-0.5f, 1f)
                    .SetRelative(true)
                    .SetDelay(1.5f);
            })
            .Select(go => go.GetComponent<Animator>())
            .ForEach(animator => animator.SetTrigger("Die"));
        // quick and dirty - TODO remove it later
        Invoke("ResetInterestPoints", 1.0f);
    }

    public void ResetInterestPoints()
    {
        foreach(MaterialToggle materialToggle in GameObject.FindGameObjectsWithTag("Interest").Select(go => go.GetComponent<MaterialToggle>()))
        {
            materialToggle.Toggle = false;
        }
    }

    /// <summary>
    /// Check if <paramref name="pointToTest"/> is in <paramref name="polygon"/>.
    /// </summary>
    /// <param name="polygon"></param>
    /// <param name="pointToTest"></param>
    /// <returns></returns>
    public static bool PointIsInPolygon(List<Vector2> polygon, Vector2 pointToTest)
    {
        bool oddPoint = false;
        int j = polygon.Count - 1;

        for(int i = 0; i < polygon.Count; ++i)
        {
            Vector2 pointI = polygon[i];
            Vector2 pointJ = polygon[j];
            if (pointI.y < pointToTest.y && pointJ.y >= pointToTest.y || pointJ.y < pointToTest.y && pointI.y >= pointToTest.y)
            {
                if (pointI.x + (pointToTest.y - pointI.y) / (pointJ.y - pointI.y) * (pointJ.x - pointI.x) < pointToTest.x)
                {
                    oddPoint = !oddPoint;
                }
            }

            j = i;
        }

        return oddPoint;
    }
}
