using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer.Audio {
    public class AudioSystem : Singleton<AudioSystem> {

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private GameObject sfxSourcesObject;

        [SerializeField] private ScriptableSounds soundClips;
        public static ScriptableSounds SoundClips => Instance.soundClips;

        private float musicVolume = 0.5f;
        private float musicVolumeMult = 0.15f; // cause music too loud

        private float sfxVolume = 0.5f;

        // Pool for regular SFX sources
        [SerializeField] private int sfxSourcePoolSize = 2;
        private List<AudioSource> sfxSources;

        #region Get Methods

        public float GetMusicVolume() {
            return musicVolume / musicVolumeMult;
        }

        public float GetSFXVolume() {
            return sfxVolume;
        }

        #endregion

        #region Set Methods

        public void SetMusicVolume(float volume) {
            musicVolume = volume * musicVolumeMult;
            musicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float volume) {
            sfxVolume = volume;
        }

        public void SetWalking(bool walking) {
            this.walking = walking;
            stepTimer = float.MaxValue; // to play walk sound right when start walking
        }

        #endregion

        private void Start() {
            InitializeSFXSources();
        }

        private void InitializeSFXSources() {
            sfxSources = new List<AudioSource>();
            for (int i = 0; i < sfxSourcePoolSize; i++) {
                AudioSource source = sfxSourcesObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxSources.Add(source);
            }
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += StopWalkingSound;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= StopWalkingSound;
        }

        private void StopWalkingSound(Scene arg0, LoadSceneMode arg1) {
            SetWalking(false);
        }

        private void Update() {
            HandleStepAudio();
            HandleMusic();
        }

        public void PlayMusic(AudioClip clip) {
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlaySound(AudioClip clip, float pitchRandomize = 0.25f, float vol = 1) {
            AudioSource sfxSource = GetAvailableSFXSource();
            if (sfxSource == null) {
                return;
            }

            sfxSource.pitch = Random.Range(1f - pitchRandomize, 1f + pitchRandomize);

            sfxSource.PlayOneShot(clip, sfxVolume * vol);
        }

        public AudioSource PlayLoopingSound(AudioClip clip, float vol = 1) {
            AudioSource sfxSource = GetAvailableSFXSource();
            if (sfxSource == null) {
                return null;
            }

            sfxSource.loop = true;
            sfxSource.clip = clip;
            sfxSource.volume = sfxVolume * vol;
            sfxSource.Play();

            return sfxSource;
        }

        public void StopLoopingSound(AudioSource sfxSource) {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }

        private AudioSource GetAvailableSFXSource() {
            foreach (var source in sfxSources) {
                if (!source.loop) {
                    return source;
                }
            }
            Debug.LogWarning("All sources are looping");
            return null; // All sources are currently playing
        }

        private void HandleMusic() {
            if (!musicSource.isPlaying) {
                musicSource.PlayOneShot(SoundClips.MusicEnv1.RandomClip());
            }
        }

        #region Walking Sound Effect

        private bool walking;
        private float stepTimer;

        private void HandleStepAudio() {
            if (walking) {
                float stepFrequency = 0.15f;
                stepTimer += Time.deltaTime;
                if (stepTimer > stepFrequency) {
                    stepTimer = 0;
                    PlaySound(soundClips.Steps.RandomClip(), 0);
                }
            }
        }

        #endregion
    }
}
