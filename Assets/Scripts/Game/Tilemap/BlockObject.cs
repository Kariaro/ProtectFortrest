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
		public string blockName;
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

		public static bool HasStates(this BlockObject block) {
			return block.hasStates && block.states.Length > 0;
		}

		[Obsolete("Old method do not use")]
		public static BlockState GetStateFromName(this BlockObject block, string name) {
			for(int i = 0, len = block.states.Length; i < len; i++) {
				if(block.states[i].name == name)
					return block.states[i];
			}

			// Default state
			return block.states[0];
		}

		public static BlockState GetState(this BlockObject block, string state) {
			for(int i = 0, len = block.states.Length; i < len; i++) {
				if(block.states[i].name == state)
					return block.states[i];
			}

			return block.states[0];
		}

		public static BlockState GetState(this BlockObject block, int index) {
			if(index < 0 || index >= block.states.Length) return block.states[0];
			return block.states[index];
		}

		public static TileBase GetDefaultTile(this BlockObject block) {
			return block.states[0].tile;
		}
	}

	[Serializable]
	public class BlockState {
		public TileBase tile;
		public string name;
	}
}
