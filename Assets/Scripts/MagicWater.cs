using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWater : MonoBehaviour
{
    [SerializeField]
    Light _spotlight = null;

    // Start is called before the first frame update
    void Start()
    {
        //Appear(2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Appear(float radius)
    {
        const float duration = 2f;

        transform.DOScale(new Vector3(radius * 2f, transform.localScale.y, radius * 2f), duration);
        float height = _spotlight.transform.localPosition.y;
        float alpha = Mathf.Atan2(radius * 2f, height) * Mathf.Rad2Deg;

        DOTween.To(SetLightAngle, 0f, alpha, duration);
        DOTween.To(SetLightIntensity, 0f, 3.5f, duration);

        const float durationBeforeDisappear = 2f;
        const float disappearDuration = 1f;

        DOTween.To(SetLightIntensity, 0f, 0f, disappearDuration).SetDelay(durationBeforeDisappear);
        transform.DOScale(new Vector3(0f, transform.localScale.y, 0f), disappearDuration).SetDelay(durationBeforeDisappear);

        Invoke("DestroySelf", durationBeforeDisappear + disappearDuration + 0.1f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void SetLightAngle(float angle)
    {
        _spotlight.spotAngle = angle + 25f;
        _spotlight.innerSpotAngle = angle;
    }

    void SetLightIntensity(float intensity)
    {
        _spotlight.intensity = intensity;
    }
}
