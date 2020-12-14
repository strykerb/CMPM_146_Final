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
    // audioSources
    // 0 -> Main music
    // 1 -> Sound effects
    // 2 -> Music fade
    AudioSource[] audioSources;
    string[] fileNames;
    string soundDirectory;
    int idx;
    float fadePercent = 0.0f; // Percent completion of a crossfade operation
    bool isCrossfading = false;

    // Awake is called before the game starts. We need to setup everything before the controller references it
    void Awake()
    {
        audioSources = gameObject.GetComponents<AudioSource>();
        Debug.Log("Found " + audioSources.Length + " audio sources.");
        InitializeRest();
        InitializeBuildUp();
        InitializeClimax();
        InitializeCooldown();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    
    // plays music relevant to state that is passed in
    // 0 -> Resting State
    // 1 -> Build Up State
    // 2 -> Climax State
    // 3 -> Cooldown State
    public void PlayMusic(int state)
    {
        if (audioSources[0].isPlaying)
        {
            StartCrossfade(state);
            return;
        }

        int audioSelection = 0;
        switch (state)
        {
            case 0:
                audioSources[0].Stop();
                audioSelection = (int)Mathf.Floor(Random.Range(0.0f, RestingMusic.Length - 1));
                audioSources[0].clip = RestingMusic[audioSelection];
                audioSources[0].Play();
                break;
            case 1:
                audioSources[0].Stop();
                audioSelection = (int)Mathf.Floor(Random.Range(0.0f, BuildUpMusic.Length - 1));
                audioSources[0].clip = BuildUpMusic[audioSelection];
                audioSources[0].Play();
                break;
        }
    }

    // Fade from current main clip to new state clip
    void StartCrossfade(int state)
    {
        // Stop fade track if for some reason it was already playing
        audioSources[2].Stop();

        // Get new clip to play
        switch(state)
        {
            case 0:
                audioSources[2].clip = RestingMusic[0];
                break;
            case 1:
                audioSources[2].clip = BuildUpMusic[0];
                break;
        }

        // Start at 0 volume, and play. Update method will take care of fading.
        audioSources[2].volume = 0;
        audioSources[2].Play();
        isCrossfading = true;
    }

    // Swap the new, full volume clip over to main track
    void FinishCrossFade()
    {
        fadePercent = 0.0f;
        audioSources[0].Stop();
        audioSources[0].clip = audioSources[2].clip;
        audioSources[0].time = audioSources[2].time;
        audioSources[0].Play();
        isCrossfading = false;
    }


    void InitializeRest()
    {
        string targetDir = Path.Combine("Sounds", "Rest", "Background");
        soundDirectory = Path.Combine(Application.dataPath, "Resources", targetDir);
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        RestingMusic = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            string pathToLoad = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(file));
            Debug.Log(pathToLoad);
            RestingMusic[idx] = Resources.Load<AudioClip>(Path.Combine(targetDir, Path.GetFileNameWithoutExtension(file)));
            Debug.Log(RestingMusic[idx]);
            idx++;
        }
    }

    void InitializeBuildUp()
    {
        string targetDir = Path.Combine("Sounds", "BuildUp");
        soundDirectory = Path.Combine(Application.dataPath, "Resources", targetDir, "Prelude");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpPreludes = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpPreludes[idx] = Resources.Load<AudioClip>(Path.Combine(targetDir, "Prelude", Path.GetFileNameWithoutExtension(file)));
            idx++;
        }

        soundDirectory = Path.Combine(Application.dataPath, "Resources", targetDir, "Background");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpMusic = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpMusic[idx] = Resources.Load<AudioClip>(Path.Combine(targetDir, "Background", Path.GetFileNameWithoutExtension(file)));
            idx++;
        }

        soundDirectory = Path.Combine(Application.dataPath, "Resources", targetDir, "Effects");
        fileNames = Directory.GetFiles(soundDirectory, "*.wav");
        BuildUpSounds = new AudioClip[fileNames.Length];
        idx = 0;
        foreach (string file in fileNames)
        {
            BuildUpSounds[idx] = Resources.Load<AudioClip>(Path.Combine(targetDir, "Background", Path.GetFileNameWithoutExtension(file)));
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
        if (isCrossfading)
        {
            // Get current percent of time spent fading, and set
            // volume of sources accordingly.
            fadePercent += Time.deltaTime / 5.0f;
            //Debug.Log(fadePercent);
            audioSources[0].volume = 1.0f - fadePercent;
            audioSources[2].volume = fadePercent;

            // Shift new clip over to main when done
            if (audioSources[0].volume <= 0.0f) FinishCrossFade();
        }
    }
}
