using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Call an FX one time at position
public class OneShotEffect : MonoBehaviour
{

    AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        if(audioSource && !audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            Sound2D.Adapt2DSound(audioSource);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = false;
            audioSource.maxDistance = 15f;
            audioSource.minDistance = 5f;
            audioSource.Play();
        }
    }
}
