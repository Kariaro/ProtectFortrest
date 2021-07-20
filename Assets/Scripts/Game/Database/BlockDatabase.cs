using ProjectFortrest.Game.Level;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFortrest.Game.Database {
	[CreateAssetMenu(fileName = "BlockDatabase", menuName = "Database/BlockDatabase", order = 1)]
	public class BlockDatabase : ScriptableObject {
		public List<BlockObject> elements;
	}

	public static class BlockDatabaseExtension {
		public static BlockObject GetFromName(this BlockDatabase database, string name) {
			foreach(BlockObject element in database.elements) {
				if(element.name.Equals(name))
					return element;
			}

			return null;
		}
	}
}
