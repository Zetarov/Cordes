using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager main;

    [System.Serializable]
    public struct SoundElement {
        public string name;
        public AudioClip clip;
        public bool loop;
    }

    [System.Serializable]
    public struct EffectElement
    {
        public string name;
        public List<AudioClip> variants;
    }

    #region publicVariables
    public bool startOnPlay = true;
    public float maxDistance = 50f;
    public float minDistance = 20f;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float effectVolume = 1f;
    #endregion

    public List<SoundElement> allMusics = new List<SoundElement>();
    List<AudioSource> allSources = new List<AudioSource>();
    List<float> allSourcesBaseVolume = new List<float>();
    float currTimeTransition = 0f; //in ms
    float timeToTransition = 0f; //in ms
    int lastIndex = 0;

    public List<EffectElement> allSoundsEffects = new List<EffectElement>();
    List<AudioSource> allSourcesSoundsEffects = new List<AudioSource>();

    string currentName = ""; //Current Sound Play

    public static MusicManager GetMusic(){
        if(MusicManager.main == null){
            GameObject musicMain = Instantiate((GameObject)Resources.Load("prefabs/MUSIC_MANAGER"));
            MusicManager.main = musicMain.GetComponent<MusicManager>();
        }
        return MusicManager.main;
    }

    // Start is called before the first frame update
    void Awake() {
        if(MusicManager.main == null){
            MusicManager.main = this;
            DontDestroyOnLoad(this);
        }else{
            Destroy(this.gameObject);
        }
        //Run main sound
        if(startOnPlay && allMusics.Count > 0) {
            PlaySound(allMusics[0].name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allSources.Count > 0)
        {
            if (currTimeTransition > 0) currTimeTransition -= Time.deltaTime;
            else currTimeTransition = 0f;
            // Others
            for (int i = 0; i < allSources.Count - 1; i++) {
                allSources[i].volume = Mathf.SmoothStep(0f, allSourcesBaseVolume[i], timeToTransition == 0f ? 0f : currTimeTransition/timeToTransition);
                if(allSources[i].volume == 0f)
                {
                    //Remove AudioSource
                    Destroy(allSources[i]);
                    allSources.RemoveAt(i);
                    allSourcesBaseVolume.RemoveAt(i);
                    i--;
                }
            }
            //Last
            lastIndex = allSources.Count - 1;
            if(allSources[lastIndex]) allSources[lastIndex].volume = Mathf.SmoothStep(volume * musicVolume, allSourcesBaseVolume[lastIndex], timeToTransition == 0f ? 0f : currTimeTransition/timeToTransition);
        }
        for(int i = 0; i < allSourcesSoundsEffects.Count; i++)
        {
            if (!allSourcesSoundsEffects[i].isPlaying)
            {
                Destroy(allSourcesSoundsEffects[i]);
                allSourcesSoundsEffects.RemoveAt(i);
                i--;
            }
        }
    }

    public void SetVolume(float newVolume = 1f, float speedTime = 5f)
    {
        newVolume = newVolume > 1 ? 1 : newVolume;
        newVolume = newVolume < 0 ? 0 : newVolume;
        volume = newVolume;
        timeToTransition = speedTime;
        currTimeTransition = timeToTransition;

        InitVolumes();
    }

    public void SetMusicVolume(float newVolume){
        newVolume = newVolume > 1 ? 1 : newVolume;
        newVolume = newVolume < 0 ? 0 : newVolume;
        musicVolume = newVolume;
    }

    public void SetEffectVolume(float newVolume) {
        newVolume = newVolume > 1 ? 1 : newVolume;
        newVolume = newVolume < 0 ? 0 : newVolume;
        effectVolume = newVolume;
    }


    void InitVolumes()
    {
        allSourcesBaseVolume.Clear();
        for (int i = 0; i < allSources.Count; i++)
        {
            allSourcesBaseVolume.Add(allSources[i].volume);
        }
    }

    public void PlaySound(string musicName, float speedTime = 10f)
    {
        SoundElement found = foundSound(musicName);
        if(found.name != null && found.name != currentName)
        {
            //We have found a sound
            currentName = found.name;
            timeToTransition = speedTime;
            currTimeTransition = timeToTransition;
            AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            Sound2D.Adapt2DSound(newAudio);
            newAudio.maxDistance = maxDistance;
            newAudio.minDistance = minDistance;
            newAudio.clip = found.clip;
            newAudio.volume = 0f;
            newAudio.Play();
            newAudio.loop = found.loop;
            allSources.Add(newAudio);

            InitVolumes();
        }
    }

    public void PlayEffect(string effectName, float volumeSound = -1f, float variantionPitch = 0.1f)
    {
        volumeSound = volumeSound == -1 ? volume : volumeSound * volume * effectVolume;
        EffectElement found = foundEffect(effectName);
        if (found.name != null)
        {
            AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            Sound2D.Adapt2DSound(newAudio);
            newAudio.maxDistance = maxDistance;
            newAudio.minDistance = minDistance;
            newAudio.clip = found.variants[Random.Range(0,found.variants.Count)];
            newAudio.volume = volumeSound;
            newAudio.Play();
            newAudio.loop = false;
            newAudio.pitch = Random.Range(1f - variantionPitch, 1 + variantionPitch);
            allSourcesSoundsEffects.Add(newAudio);

        }
    }

    SoundElement foundSound(string musicName)
    {
        for(int i = 0; i < allMusics.Count; i++)
        {
            if (allMusics[i].name == musicName) return allMusics[i];
        }
        return default(SoundElement);
    }

    EffectElement foundEffect(string effectName)
    {
        for (int i = 0; i < allSoundsEffects.Count; i++)
        {
            if (allSoundsEffects[i].name == effectName) return allSoundsEffects[i];
        }
        return default(EffectElement);
    }
}
