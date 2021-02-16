using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDispenser : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _prefabs = new List<GameObject>();

    [SerializeField]
    private float _distance = 1.5f;

    private Vector3 _lastKnownPosition;
    private Quaternion _lastKnownRotation;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_prefabs.Count > 0);
    }

    // Update is called once per frame
    void Update()
    {
        if((_lastKnownPosition - transform.position).sqrMagnitude > _distance * _distance)
        {
            Instantiate(_prefabs[Random.Range(0, _prefabs.Count)], _lastKnownPosition, transform.rotation);
            _lastKnownPosition = transform.position;
            _lastKnownRotation = transform.rotation;
        }
    }

    private void OnDisable()
    {
        
    }

    private void OnEnable()
    {
        _lastKnownPosition = transform.position;
        _lastKnownRotation = transform.rotation;
    }
}
