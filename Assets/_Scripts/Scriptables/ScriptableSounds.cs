using UnityEngine;

namespace SpeedPlatformer.Audio {

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

        [SerializeField] private AudioClip grappleLaunch;
        public AudioClip GrappleLaunch => grappleLaunch;

        [SerializeField] private AudioClip grappleBreak;
        public AudioClip GrappleBreak => grappleBreak;

        [SerializeField] private AudioClip grappleRetract;
        public AudioClip GrappleRetract => grappleRetract;

        [SerializeField] private AudioClip grappleAttach;
        public AudioClip GrappleAttach => grappleAttach;
    }
}