using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class VisualNote : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _meshRenderer = null;

    [SerializeField]
    NotesParametersScriptableObject _parameters = null;

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> waveMoveTween;

    const float _appearDuration = 1.0f;
    const float _disappearDuration = 1.0f;

    private void Start()
    {
        StartCoroutine(LifeCoroutine());
    }

    private IEnumerator LifeCoroutine()
    {
        #region Initialization
        _meshRenderer.material = new Material(_meshRenderer.material);
        _meshRenderer.material.color = Color.Lerp(_parameters.BaseColor, Color.white, 0.5f);
        _meshRenderer.material.SetColor("_EmissionColor", _parameters.BaseColor);
        #endregion

        // Appear
        _meshRenderer.material.DOFade(0f, _appearDuration).From();

        // Main loop
        waveMoveTween = transform.DOMoveY(0.25f, 1.0f);
        waveMoveTween.SetLoops(-1, LoopType.Yoyo);
        waveMoveTween.SetEase(Ease.InOutSine);
        waveMoveTween.SetRelative(true);

        yield return new WaitForSeconds(_appearDuration + _parameters.DelayBeforeFadingAway);

        #region Fade away
        waveMoveTween.Kill(false);

        var tweening = transform.DOMoveY(2.0f, 1.0f);
        tweening.SetEase(Ease.InBack);
        tweening.SetRelative(true);

        _meshRenderer.material.DOFade(0f, 1.0f);
        #endregion

        yield return new WaitForSeconds(_disappearDuration);

        Destroy(gameObject);
    }
}
