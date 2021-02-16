using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNote : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _meshRenderer = null;

    [SerializeField]
    List<DOTweenAnimation> _animationsForFade = new List<DOTweenAnimation>();

    [SerializeField]
    NotesParametersScriptableObject _parameters = null;

    int _finishedAnim = 0;

    private void Start()
    {
        _meshRenderer.material.color = Color.Lerp(_parameters.BaseColor, Color.white, 0.5f);
        _meshRenderer.material.SetColor("_EmissionColor", _parameters.BaseColor);
        Invoke("FadeAway", _parameters.DelayBeforeFadingAway);
    }

    private void FadeAway()
    {
        foreach(DOTweenAnimation anim in _animationsForFade)
        {
            anim.DOPlay();
            anim.onComplete.AddListener(AnimCompleted);
        }
    }

    void AnimCompleted()
    {
        ++_finishedAnim;
        if(_finishedAnim >= _animationsForFade.Count)
        {
            Destroy(gameObject);
        }
    }
}
