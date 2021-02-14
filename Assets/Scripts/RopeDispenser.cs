using System.Collections;
using System.Collections.Generic;
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

    private LineRenderer _lineRenderer;
    private float _pointStepSquared;
    private Queue<Vector3> _points = new Queue<Vector3>();
    private Vector3? _lastEnqueuedPoint = null;
    private float _totalLength = 0f;

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
            PutRopePoint();
            RemoveFarPoints();
            UpdateLineRenderer();
        }
    }

    /// <summary>
    /// Put a new rope point at the current position of the object
    /// </summary>
    void PutRopePoint()
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
}
