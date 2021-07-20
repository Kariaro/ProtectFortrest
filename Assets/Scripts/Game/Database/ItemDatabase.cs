using ProjectFortrest.Game.Level;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFortrest.Game.Database {
	[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/ItemDatabase", order = 1)]
	public class ItemDatabase : ScriptableObject {
		public List<ItemObject> elements;
	}

	public static class ItemDatabaseExtension {
		public static ItemObject GetFromName(this ItemDatabase database, string name) {
			foreach(ItemObject element in database.elements) {
				if(element.name.Equals(name))
					return element;
			}

			return null;
		}
	}
}
