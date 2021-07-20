using UnityEngine;


namespace ProjectFortrest.Game.Sound {
	[CreateAssetMenu(fileName = "Sound", menuName = "Database/Sound", order = 1)]
	public class SoundObject : ScriptableObject {
		public string soundName;
		public AudioClip clip;
	}

	public static class SoundObjectExtension {
		public static void Play(this SoundObject sound) {
			
		}
	}
}
