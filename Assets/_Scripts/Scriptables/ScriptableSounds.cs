using UnityEngine;

namespace RoboRiftRush.Audio {

	[CreateAssetMenu(fileName = "Sounds", menuName = "Resources/Sounds")]
	public class ScriptableSounds : ScriptableObject {

        [SerializeField] private AudioClip[] musicEnv1;
        public AudioClip[] MusicEnv1 => musicEnv1;

        [SerializeField] private AudioClip[] steps;
		public AudioClip[] Steps => steps;

		[SerializeField] private AudioClip jump;
        public AudioClip Jump => jump;

        [SerializeField] private AudioClip land;
        public AudioClip Land => land;

        [SerializeField] private AudioClip startLevel;
        public AudioClip StartLevel => startLevel;

        [SerializeField] private AudioClip completeLevel;
        public AudioClip CompleteLevel => completeLevel;

        [SerializeField] private AudioClip fallVoid;
        public AudioClip FallVoid => fallVoid;

        [SerializeField] private AudioClip trapDeath;
        public AudioClip TrapDeath => trapDeath;

        [SerializeField] private AudioClip trapSpike;
        public AudioClip TrapSpike => trapSpike;

        [SerializeField] private AudioClip wind;
        public AudioClip Wind => wind;

        [SerializeField] private AudioClip grapplePickup;
        public AudioClip GrapplePickup => grapplePickup;

        [SerializeField] private AudioClip grappleLaunch;
        public AudioClip GrappleLaunch => grappleLaunch;

        [SerializeField] private AudioClip grappleBreak;
        public AudioClip GrappleBreak => grappleBreak;

        [SerializeField] private AudioClip grappleAttach;
        public AudioClip GrappleAttach => grappleAttach;

        [SerializeField] private AudioClip platformDisappear;
        public AudioClip PlatformDisappear => platformDisappear;

        [SerializeField] private AudioClip islandDissolve;
        public AudioClip IslandDissolve => islandDissolve;

        [Header("UI")]
        [SerializeField] private AudioClip mouseOn;
        public AudioClip MouseOn => mouseOn;

        [SerializeField] private AudioClip mouseOff;
        public AudioClip MouseOff => mouseOff;

        [SerializeField] private AudioClip buttonClick;
        public AudioClip ButtonClick => buttonClick;

        [SerializeField] private AudioClip clickLocked;
        public AudioClip ClickLocked => clickLocked;

        [SerializeField] private AudioClip _UISlide;
        public AudioClip UISlide => _UISlide;
    }
}