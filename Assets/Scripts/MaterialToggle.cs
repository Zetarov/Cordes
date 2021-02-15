using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialToggle : MonoBehaviour
{
    [SerializeField]
    private bool _toggle = false;

    [SerializeField]
    Material _matOff = null;

    [SerializeField]
    Material _matOn = null;

    public bool Toggle
    {
        get => _toggle;
        set
        {
            if (_toggle == value)
                return;
            _toggle = value;
            Refresh();
        }
    }

    MeshRenderer _meshRenderer = null;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnValidate()
    {
        if(_matOff != null && _matOn != null)
        {
            Refresh(GetComponent<MeshRenderer>());
        }
    }

    private void Refresh()
    {
        Refresh(_meshRenderer);
    }

    private void Refresh(MeshRenderer meshRenderer)
    {
        meshRenderer.material = _toggle ? _matOn : _matOff;
    }
}
