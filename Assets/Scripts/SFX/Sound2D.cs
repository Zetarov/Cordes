using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound2D : MonoBehaviour
{
    public float maxDistance = 50f;
    public float minDistance = 20f;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateSound();
    }

    public void UpdateSound()
    {
        Sound2D.Adapt2DSound(audioSource);

        audioSource.maxDistance = maxDistance;
        audioSource.minDistance = minDistance;
    }

    public static void Adapt2DSound(AudioSource audioSource) {
        audioSource.spatialBlend = 0f;  //Set spatial Blend in 3D
        audioSource.dopplerLevel = 0f;    //Remove doppler Level
        audioSource.spread = 0f;          //Remove Spread
        audioSource.rolloffMode = AudioRolloffMode.Linear; //Set constant fade by distance
    }
}
