using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    struct AudioElement {
        public AudioSource source;
        public float volume;
        public bool mute;
    }

    [SerializeField] protected AudioClip environnement;
    [SerializeField] protected AudioClip infatuation1;
    [SerializeField] protected AudioClip infatuation2;

    [SerializeField] protected AudioClip theme1;
    [SerializeField] protected AudioClip theme2;

    AudioElement audio_environnement;
    AudioElement audio_infatuation1;
    AudioElement audio_infatuation2;
    AudioElement audio_theme1;
    AudioElement audio_theme2;

    [Range(0f,1.0f)]
    [SerializeField] float volume = 1.0f;

    [SerializeField] protected bool playTheme1 = false;
    [SerializeField] protected bool playTheme2 = false;

    // Start is called before the first frame update
    void Start()
    {
        audio_environnement.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audio_infatuation1.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audio_infatuation2.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audio_theme1.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audio_theme2.source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        audio_environnement.source.clip = environnement;
        audio_infatuation1.source.clip = infatuation1;
        audio_infatuation2.source.clip = infatuation2;
        audio_theme1.source.clip = theme1;
        audio_theme2.source.clip = theme2;

        InitializeSound(ref audio_environnement);
        InitializeSound(ref audio_infatuation1);
        InitializeSound(ref audio_infatuation2);
        InitializeSound(ref audio_theme1);
        InitializeSound(ref audio_theme2);

        audio_environnement.volume = 1.0f;
        audio_environnement.mute = false;
    }

    // Update is called once per frame
    void Update() {
        audio_theme1.mute = !playTheme1;
        audio_theme2.mute = !playTheme2;

        audio_infatuation1.mute = !(playTheme1 || playTheme2);
        audio_infatuation2.mute = !(playTheme1 && playTheme2);

        UpdateSoundElement(ref audio_environnement);
        UpdateSoundElement(ref audio_infatuation1);
        UpdateSoundElement(ref audio_infatuation2);
        UpdateSoundElement(ref audio_theme1);
        UpdateSoundElement(ref audio_theme2);
    }

    void InitializeSound(ref AudioElement element){
        element.source.loop = true;
        element.mute = true;
        element.volume = 0.0f;
        element.source.volume = 0.0f;
        element.source.Play();
    }

    void UpdateSoundElement(ref AudioElement element){
        if(volume == 0f) element.source.volume = 0f;
        else element.source.volume = element.volume * volume;

        if(element.mute){
            element.volume -= 1f * Time.deltaTime;
            element.volume = element.volume < 0 ? 0 : element.volume;
        }
        else {
            element.volume += 1f * Time.deltaTime;
            element.volume = element.volume > 1 ? 1 : element.volume;
        } 
    }
}
