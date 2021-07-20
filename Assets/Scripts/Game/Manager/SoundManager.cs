using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProjectFortrest.Game.Manager {
	public class SoundManager : MonoBehaviour, IManager {
		public SoundDatabase database;
		
		private Dictionary<string, SoundObject> SoundMap;
		private Dictionary<string, AudioSource> SoundAudioMap;
		
		[SerializeField]
		private GameObject AudioGameObject;

		private void ReloadDatabase() {
			SoundMap = new Dictionary<string, SoundObject>();
			SoundAudioMap = new Dictionary<string, AudioSource>();

			AudioSource[] sources = AudioGameObject.GetComponents<AudioSource>();
			if(sources.Length > 0) {
				foreach(AudioSource source in sources) {
					GameObject.Destroy(source);
				}
			}

			foreach(SoundObject element in database.elements) {
				if(element == null) continue;
				SoundMap.Add(element.soundName, element);

				AudioSource source = AudioGameObject.AddComponent<AudioSource>();
				source.clip = element.clip;
				SoundAudioMap.Add(element.soundName, source);
			}
		}


		public SoundObject GetFromName(string name) {
			if(name == null) return null;

			if(SoundMap == null) ReloadDatabase();
			SoundMap.TryGetValue(name, out var result);
			return result;
		}

		public void Play(string name) {
			if(name == null) return;
			
			if(SoundAudioMap == null) ReloadDatabase();
			if(SoundAudioMap.TryGetValue(name, out AudioSource source)) {
				source.Play();
			} else {
				Debug.LogWarning($"Could not find sound '{name}'");
			}
		}

		public void Init(GameManager manager) {
			
		}

		public void OnStart() {
			AudioGameObject = new GameObject("Audio Output");
			AudioGameObject.transform.parent = transform;

			ReloadDatabase();
		}

		public static SoundObject GetSound(string name) {
			return GameManager.Instance.SoundManager.GetFromName(name);
		}

		public static void PlaySound(string name) {
			GameManager.Instance.SoundManager.Play(name);
		}
	}
}
