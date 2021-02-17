using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidMove : MonoBehaviour
{
    [SerializeField, Range(0.1f, 50f)]
    private float _length;

    [SerializeField, Range(0.01f, 10f)]
    private float _heightSemiRange;

    private Vector2 _lastPosition;
    private float _distance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = transform.position.XZ();

        _distance += Vector2.Distance(_lastPosition, newPosition);
        while(_distance > _length)
        {
            _distance -= _length;
        }

        float x = (_distance / _length) * 2f * Mathf.PI;
        float y = Mathf.Sin(x);

        transform.localPosition = new Vector3(transform.localPosition.x, y * _heightSemiRange, transform.localPosition.z);

        _lastPosition = newPosition;
    }

    void OnValidate()
    {
        if(Application.isPlaying)
        {
            while(_distance > _length)
            {
                _distance -= _length;
            }
        }
    }

    private void OnEnable()
    {
        _lastPosition = transform.position.XZ();
    }
}
