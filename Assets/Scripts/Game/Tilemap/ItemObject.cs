using ProjectFortrest.Game.Blocks;
using ProjectFortrest.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProjectFortrest.Game.Level {
	[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
	public class ItemObject : ScriptableObject {
		// This is the identifying name of the item.
		public string itemName;
		public IItemGroup itemGroup;
		public GameObject prefab;
		public bool interactable;
		public bool hasStates;

		// Default state always first
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
