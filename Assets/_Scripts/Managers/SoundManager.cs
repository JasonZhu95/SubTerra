using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Sound Manager in scene.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoadChangeMusic;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadChangeMusic;
    }

    public void OnSceneLoadChangeMusic(Scene scene, LoadSceneMode mode)
    {
        StopPlay("MusicTheme" + (scene.buildIndex - 1));
        StopPlay("UIGameStart");
        switch(scene.buildIndex)
        {
            case 0:
                Play("MusicTheme0");
                break;
            case 1:
                Play("MusicTheme1");
                break;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
                return;
        }
        s.source.Play();
    }

    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
            return;
        }
        s.source.Stop();
    }

    public void ChangeVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Name Was Not Found.");
            return;
        }
        s.source.volume = volume;
    }
}