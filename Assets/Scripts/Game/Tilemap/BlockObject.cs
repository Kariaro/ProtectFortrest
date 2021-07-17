using ProjectFortrest.Game.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProjectFortrest.Game.Level {
	[CreateAssetMenu(fileName = "Block", menuName = "Blocks/Block", order = 1)]
	public class BlockObject : ScriptableObject {
		public TileBase tile;
		public string blockName;
		public string defaultState;
		public IBlockGroup blockGroup;
		public GameObject prefab;
		public bool interactable;
		public bool hasStates;
		public BlockState[] states;
	}

	public static class BlockObjectExtension {
		public static string[] GetStateNames(this BlockObject block) {
			string[] array = new string[block.states.Length];
			for(int i = 0, len = array.Length; i < len; i++) {
				array[i] = block.states[i].name;
			}

			return array;
		}

		public static BlockState GetStateFromName(this BlockObject block, string name) {
			for(int i = 0, len = block.states.Length; i < len; i++) {
				if(block.states[i].name == name)
					return block.states[i];
			}

			Debug.LogError("Invalid fallback for GetStateFromName was called. (This should never happen)");
			return new BlockState() { name = "", tile = block.tile };
		}

		public static TileBase GetDefaultTile(this BlockObject block) {
			if(block.hasStates) {
				return block.GetStateFromName(block.defaultState).tile;
			} else {
				return block.tile;
			}
		}
	}

	[Serializable]
	public class BlockState {
		public string name;
		public TileBase tile;
	}
}
