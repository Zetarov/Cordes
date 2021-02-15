using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// Handles the points of the rope.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class RopeDispenser : MonoBehaviour
{
    [SerializeField, Tooltip("The minimum distance between two points.")]
    private float _pointStep;

    [SerializeField, Tooltip("The maximum duration the rope can reach.")]
    private float _maxDuration;

    [SerializeField, Tooltip("The minimum distance between this and a rope point to consider the shape closed.")]
    private float _minCloseSnapDist = 0.1f;

    private LineRenderer _lineRenderer;
    private float _pointStepSquared;
    private Queue<PointRecord> _points = new Queue<PointRecord>();
    private PointRecord _lastEnqueuedRecord = null;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _pointStepSquared = _pointStep * _pointStep;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_pointStep > _minCloseSnapDist);
    }

    // Update is called once per frame
    void Update()
    {
        // Search a point for closing the shape
        int? closePointIdx = FindAClosePointIndex();
        if(closePointIdx.HasValue)
        {
            TestInsidePoint(startIndex: closePointIdx.Value);
            // Destroy all points until this index
            _points.Clear();
            _lastEnqueuedRecord = null;
        }

        PutRopePointOrEditLastOne();
        RemoveOldPoints();
        UpdateLineRenderer();
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
        while(_points.Count > 0 && currentTime - _points.Peek().Time > _maxDuration)
        {
            _points.Dequeue();
        }
    }

    /// <summary>
    /// Updates the <see cref="_lineRenderer"/> to show the <see cref="_points"/>.
    /// </summary>
    void UpdateLineRenderer()
    {
        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.Select(pointRecord => pointRecord.Point).ToArray());
    }

    int? FindAClosePointIndex()
    {
        // Prevent for finding last put points as enclosing shape
        const int skipPointNb = 10;

        if (_points.Count <= skipPointNb)
            return null;

        Vector3 currentPos = transform.position;

        int bestCandidate = -1;
        float minSqrMagnitude = float.PositiveInfinity;

        int index = 1;

        Vector3 prevPoint = _points.First().Point;

        foreach(Vector3 point in _points.Select(pointRecord => pointRecord.Point).Skip(1).Take(_points.Count - skipPointNb))
        {
            float sqrMagnitude = (point - currentPos).sqrMagnitude;

            if(sqrMagnitude < minSqrMagnitude)
            {
                bestCandidate = index;
                minSqrMagnitude = sqrMagnitude;
            }

            prevPoint = point;
            ++index;
        }

        bool candidateRetained = minSqrMagnitude <= _minCloseSnapDist * _minCloseSnapDist;
        return candidateRetained ? bestCandidate : null as int?;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
    }

    public void TestInsidePoint(int startIndex = 0)
    {
        List<Vector2> points = _points.Skip(startIndex).Select(pointRecord => new Vector2(pointRecord.Point.x, pointRecord.Point.z)).ToList();
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Interest"))
        {
            Vector2 goPoint = new Vector2(go.transform.position.x, go.transform.position.z);
            go.GetComponent<MaterialToggle>().Toggle = PointIsInPolygon(points, goPoint);
        }
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
