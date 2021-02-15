using DelaunayVoronoi;
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

    [SerializeField, Tooltip("The maximum length the rope can reach.")]
    private float _maxLength;

    [SerializeField]
    private float _minCloseSnapDist = 0.1f;

    private LineRenderer _lineRenderer;
    private float _pointStepSquared;
    private Queue<Vector3> _points = new Queue<Vector3>();
    private Vector3? _lastEnqueuedPoint = null;
    private float _totalLength = 0f;

    private IEnumerable<Triangle> _triangles = new List<Triangle>();

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _pointStepSquared = _pointStep * _pointStep;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckCanPutRopePoint())
        {
            int? closePointIdx = FindAClosePointIndex();
            if(closePointIdx.HasValue)
            {
                TestInsidePoint(startIndex: closePointIdx.Value);
            }

            PutRopePoint(transform.position);
            RemoveFarPoints();
            UpdateLineRenderer();
        }
    }

    /// <summary>
    /// Put a new rope point at the current position of the object
    /// </summary>
    void PutRopePoint(Vector3 position)
    {
        Vector3 newPoint = transform.position;

        _points.Enqueue(newPoint);

        if(_lastEnqueuedPoint.HasValue)
        {
            _totalLength += (_lastEnqueuedPoint.Value - newPoint).magnitude;
        }

        _lastEnqueuedPoint = newPoint;

    }

    /// <summary>
    /// Remove the farthest points while the allowed length of the rope is exceeded.
    /// </summary>
    void RemoveFarPoints()
    {
        while(_totalLength > _maxLength && _points.Count > 0)
        {
            RemoveFarPoint();
        }
    }

    /// <summary>
    /// Remove the farthest points of the rope (aka the oldest).
    /// </summary>
    void RemoveFarPoint()
    {
        Vector3 removed = _points.Dequeue();
        Vector3 last = _points.Peek();

        _totalLength -= (last - removed).magnitude;
    }

    /// <summary>
    /// Updates the <see cref="_lineRenderer"/> to show the <see cref="_points"/>.
    /// </summary>
    void UpdateLineRenderer()
    {
        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.ToArray());
    }

    /// <summary>
    /// Returns <c>true</c> if the condition are met to put a new point in the rope.
    /// </summary>
    /// <returns></returns>
    bool CheckCanPutRopePoint()
    {
        if(_lineRenderer.positionCount == 0)
        {
            return true;
        }

        Vector3 _lastPointPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
        return (transform.position - _lastPointPosition).sqrMagnitude > _pointStepSquared;
    }

    int? FindAClosePointIndex()
    {
        if (_points.Count == 0)
            return null;

        Vector3 currentPos = transform.position;

        int bestCandidate = -1;
        float minSqrMagnitude = float.PositiveInfinity;

        int index = 0;
        foreach(Vector3 point in _points)
        {
            float sqrMagnitude = (currentPos - point).sqrMagnitude;

            if(sqrMagnitude < minSqrMagnitude)
            {
                bestCandidate = index;
                minSqrMagnitude = sqrMagnitude;
            }

            ++index;
        }

        bool candidateRetained = minSqrMagnitude <= _minCloseSnapDist * _minCloseSnapDist;
        return candidateRetained ? bestCandidate : null as int?;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(Triangle triangle in _triangles)
        {
            Gizmos.DrawLine(triangle.Vertices[0].XYtoVector3XZ(), triangle.Vertices[1].XYtoVector3XZ());
            Gizmos.DrawLine(triangle.Vertices[1].XYtoVector3XZ(), triangle.Vertices[2].XYtoVector3XZ());
            Gizmos.DrawLine(triangle.Vertices[2].XYtoVector3XZ(), triangle.Vertices[0].XYtoVector3XZ());
        }
    }

    public void TestInsidePoint(int startIndex = 0)
    {
        List<Vector2> points = _points.Skip(startIndex).Select(point => new Vector2(point.x, point.z)).ToList();
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

    public static bool PointIsInPolygon(List<Vector2> points, Vector2 pointToTest)
    {
        bool oddPoint = false;
        int j = points.Count - 1;

        for(int i = 0; i < points.Count; ++i)
        {
            Vector2 pointI = points[i];
            Vector2 pointJ = points[j];
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
