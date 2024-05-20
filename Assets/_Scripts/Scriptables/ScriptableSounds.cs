using UnityEngine;

namespace SpeedPlatformer.Audio {

	[CreateAssetMenu(fileName = "Sounds", menuName = "Resources/Sounds")]
	public class ScriptableSounds : ScriptableObject {

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

        [SerializeField] private AudioClip wind;
        public AudioClip Wind => wind;
    }
}