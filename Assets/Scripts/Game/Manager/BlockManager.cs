using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProjectFortrest.Game.Manager {
	public class BlockManager : MonoBehaviour, IManager {
		public BlockDatabase database;

		public BlockObject GRASS;
		public BlockObject PIPE;
		public BlockObject BRICK_WALL;
		public BlockObject WOOD_WALL;
		public BlockObject CHEST;
		public UnityEngine.Object testObject;
		
		private Dictionary<string, BlockObject> BlockMap;
		private Dictionary<TileBase, BlockObject> TileToBlock;

		private void ReloadDatabase() {
			BlockMap = new Dictionary<string, BlockObject>();
			TileToBlock = new Dictionary<TileBase, BlockObject>();

			foreach(BlockObject block in database.elements) {
				BlockMap.Add(block.blockName, block);
				TileToBlock.Add(block.tile, block);
			}
		}

		public BlockObject GetFromTile(TileBase tileBase) {
			if(tileBase == null) return null;
			
			if(TileToBlock == null) ReloadDatabase();
			TileToBlock.TryGetValue(tileBase, out var result);
			return result;
		}

		public BlockObject GetFromName(string name) {
			if(name == null) return null;

			if(BlockMap == null) ReloadDatabase();
			BlockMap.TryGetValue(name, out var result);
			return result;
		}

		public void Init(GameManager manager) {
			
		}

		public void OnStart() {
			ReloadDatabase();

			// Hacky way to debug stuff
			Type type = typeof(BlockManager);
			FieldInfo[] fields = type.GetFields();
			foreach(FieldInfo field in fields) {
				if(field.FieldType == typeof(BlockObject)) {
					BlockMap.TryGetValue(field.Name, out var value);
					field.SetValue(this, value);
				}
			}
		}

		public static BlockObject GetBlock(string name) {
			return GameManager.Instance.BlockManager.GetFromName(name);
		}

		public static string GetNamespaceName(string key) {
			int idx = key.IndexOf(':');
			if(idx < 0) return key;

			return key.Substring(0, idx);
		}

		public static string GetNamespaceKey(string key) {
			int idx = key.IndexOf(':');
			if(idx < 0) return "";
			return key.Substring(idx + 1);
		}
	}
}
