using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] RestingMusic;
    public AudioClip[] BuildUpMusic;
    public AudioClip[] BuildUpPreludes;
    public AudioClip[] BuildUpSounds;
    AudioSource[] audioSources;
    string[] fileNames;
    string soundDirectory;
    int idx;


    // Start is called before the first frame update
    void Start()
    {
        audioSources = gameObject.GetComponents<AudioSource>();
        InitializeRest();
        InitializeBuildUp();
        InitializeClimax();
        InitializeCooldown();
    }

    
    // plays music relevant to state that is passed in
    // 0 -> Resting State
    // 1 -> Build Up State
    // 2 -> Climax State
    // 3 -> Cooldown State
    public void PlayMusic(int state)
    {
        switch (state)
        {
            case 0:
                audioSources[0].Stop();
                audioSources[0].clip = RestingMusic[0];
                audioSources[0].Play();
                break;
        }
    }

    void InitializeRest()
    {
        soundDirectory = Path.Combine(Application.dataPath, "Sounds", "Rest", "Background");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        RestingMusic = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            RestingMusic[idx] = new WWW(file).GetAudioClip(false, true, AudioType.WAV);
            idx++;
        }
    }

    void InitializeBuildUp()
    {
        soundDirectory = Path.Combine(Application.dataPath, "Sounds", "BuildUp", "Prelude");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpPreludes = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpPreludes[idx] = new WWW(file).GetAudioClip(false, true, AudioType.WAV);
            idx++;
        }

        soundDirectory = Path.Combine(Application.dataPath, "Sounds", "BuildUp", "Background");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpMusic = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpMusic[idx] = new WWW(file).GetAudioClip(false, true, AudioType.WAV);
            idx++;
        }

        soundDirectory = Path.Combine(Application.dataPath, "Sounds", "BuildUp", "Effects");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpSounds = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpSounds[idx] = new WWW(file).GetAudioClip(false, true, AudioType.WAV);
            idx++;
        }
    }

    void InitializeClimax()
    {

    }

    void InitializeCooldown()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
