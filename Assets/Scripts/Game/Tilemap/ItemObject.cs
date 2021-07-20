using ProjectFortrest.Game.Items;
using System;
using UnityEngine;


namespace ProjectFortrest.Game.Level {
	[CreateAssetMenu(fileName = "Item", menuName = "Database/Item", order = 1)]
	public class ItemObject : ScriptableObject {
		public string itemName;
		public IItemGroup itemGroup;
		public GameObject prefab;
		public bool interactable;
		public bool hasStates;
		public ItemState[] states;
	}

	[Serializable]
	public class ItemState {
		public Sprite sprite;
		public string name;
	}

	public static class ItemObjectExtension {
		
	}
}
