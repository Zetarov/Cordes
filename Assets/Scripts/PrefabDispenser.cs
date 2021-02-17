using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDispenser : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _prefabs = new List<GameObject>();

    [SerializeField]
    private float _distance = 1.5f;

    protected Vector3 _lastKnownPosition;
    protected Quaternion _lastKnownRotation;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_prefabs.Count > 0);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnIfNecessary();
    }

    protected virtual GameObject SpawnIfNecessary()
    {
        if((_lastKnownPosition - transform.position).sqrMagnitude > _distance * _distance)
        {
            GameObject go = Instantiate(_prefabs[Random.Range(0, _prefabs.Count)], _lastKnownPosition, transform.rotation);
            _lastKnownPosition = transform.position;
            _lastKnownRotation = transform.rotation;
            return go;
        }
        return null;
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
