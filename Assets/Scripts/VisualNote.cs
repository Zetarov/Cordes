using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisualNote : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _meshRenderer = null;

    [SerializeField]
    NotesParametersScriptableObject _parameters = null;

    public float Duration = 4f;

    [NonSerialized]
    public UnityEvent AboutToBeDestroyed = new UnityEvent();
    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> waveMoveTween;
    private const float _appearDurationProp = 0.25f;
    private const float _disappearDuration = 1.0f;
    private Coroutine _lifeCoroutine = null;
    private Coroutine _fadeCoroutine = null;

    private void Start()
    {
        _lifeCoroutine = StartCoroutine(Life_Coroutine());
    }

    private IEnumerator Life_Coroutine()
    {
        #region Initialization
        _meshRenderer.material = new Material(_meshRenderer.material);
        _meshRenderer.material.color = Color.Lerp(_parameters.BaseColor, Color.white, 0.5f);
        _meshRenderer.material.SetColor("_EmissionColor", _parameters.BaseColor);
        #endregion

        // Appear
        _meshRenderer.material.DOFade(0f, _appearDurationProp * Duration).From();

        // Main loop
        waveMoveTween = transform.DOMoveY(0.25f, 1.0f);
        waveMoveTween.SetLoops(-1, LoopType.Yoyo);
        waveMoveTween.SetEase(Ease.InOutSine);
        waveMoveTween.SetRelative(true);

        yield return new WaitForSeconds(Duration);

        #region Fade away
        _fadeCoroutine = StartCoroutine(Fade_Coroutine());
        #endregion
    }

    public void Fade()
    {
        if (_fadeCoroutine != null)
            return;
        if(_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);
        _fadeCoroutine = StartCoroutine(Fade_Coroutine());
    }

    private IEnumerator Fade_Coroutine()
    {
        waveMoveTween.Kill(false);

        var tweening = transform.DOMoveY(2.0f, _disappearDuration);
        tweening.SetEase(Ease.InBack);
        tweening.SetRelative(true);

        _meshRenderer.material.DOFade(0f, _disappearDuration);

        yield return new WaitForSeconds(_disappearDuration);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        AboutToBeDestroyed.Invoke();
    }
}
