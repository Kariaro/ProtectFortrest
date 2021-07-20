using ProjectFortrest.Game.Sound;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFortrest.Game.Database {
	[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Database/SoundDatabase", order = 1)]
	public class SoundDatabase : ScriptableObject {
		public List<SoundObject> elements;
	}

	public static class SoundDatabaseExtension {
		public static SoundObject GetFromName(this SoundDatabase database, string name) {
			foreach(SoundObject element in database.elements) {
				if(element.name.Equals(name))
					return element;
			}

			return null;
		}
	}
}
