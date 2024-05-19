using Unity.VisualScripting;
using UnityEngine;

namespace SpeedPlatformer.Audio {
    public class AudioSystem : StaticInstance<AudioSystem> {

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource soundsSource;

        [SerializeField] private ScriptableSounds soundClips;
        public static ScriptableSounds SoundClips => Instance.soundClips;

        private float musicVolume = 1f;
        private float soundEffectsVolume = 1f;

        private bool walking;

        #region Get Methods

        public float GetMusicVolume() {
            return musicVolume;
        }

        public float GetSoundEffectsVolume() {
            return soundEffectsVolume;
        }

        #endregion

        #region Set Methods

        public void SetMusicVolume(float volume) {
            musicVolume = volume;
            musicSource.volume = musicVolume;
        }

        public void SetSoundEffectsVolume(float volume) {
            soundEffectsVolume = volume;
        }

        public void SetWalking(bool walking) {
            this.walking = walking;
            stepTimer = float.MaxValue; // to play walk sound right when start walking
        }

        #endregion

        private void Update() {
            HandleStepAudio();
        }

        public void PlayMusic(AudioClip clip) {
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlaySound(AudioClip clip, bool randomizePitch = true, float vol = 1) {

            if (randomizePitch) {
                soundsSource.pitch = Random.Range(1f, 1.5f);
            }
            else {
                soundsSource.pitch = 1f;
            }

            soundsSource.PlayOneShot(clip, soundEffectsVolume * vol);
        }

        private float stepTimer;

        private void HandleStepAudio() {

            if (walking) {
                float stepFrequency = 0.15f;
                stepTimer += Time.deltaTime;
                if (stepTimer > stepFrequency) {
                    stepTimer = 0;
                    PlaySound(soundClips.Steps.RandomSound(), false);
                }
            }
        }
    }
}