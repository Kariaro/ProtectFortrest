using ProjectFortrest.Game.Blocks;
using ProjectFortrest.Game.Blocks.Impl;
using ProjectFortrest.Game.Manager;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProjectFortrest.Game.Level {

	/*
	 * A block map pattern:
	 * 
	 * TileLevel:
	 *     // When adding a new block it should be put in this map.
	 *     // This block map should contain all blocks at all time.
	 *     BlockMap blocks;
	 *     
	 *     // This method should add the block to the block map and
	 *     // add the block to the tilemap.
	 *     void AddBlock(int x, int y, BlockObject block);
	 *     
	 *     // This method should serialize the blockmap object.
	 *     void Serialize(writer);
	 *     
	 *     // This method should deserialize the blockmap object.
	 *     void Deserialize(reader);
	 * 
	 * 
	 * BlockMap:
	 *     // Contains all the block objects
	 *     Dictionary<Vector3Int, BlockDataObject> _Blocks;
	 *     
	 *     // This method should write:
	 *     //   WRITE (_Blocks.Count)
	 *     //   FOR (Position) IN (_BLOCKS)
	 *     //     WRITE (Position);
	 *     //     SERIALIZE (_Blocks[Position]);
	 *     void Serialize(writer);
	 *     
	 *     // This method should read:
	 *     //   READ (_Count);
	 *     //   FOR (I) IN (_Count)
	 *     //     READ_Vec3I (_Position);
	 *     //     DESERIALIZE (BlockDataObject) -> (_bdo);
	 *     //     ADD (_Position, _bdo);
	 *     void Deserialize(reader);
	 * 
	 * 
	 * BlockDataObject:
	 *     // The block object.
	 *     BlockObject block;
	 *     
	 *     // The state of this block.
	 *     string state;
	 *     
	 *     // The script instance of this block.
	 *     BlockGameObject go;
	 *     
	 *     // This method should write:
	 *     //   WRITE (block.blockName);
	 *     //   WRITE (state);
	 *     //   IF (block.prefab != null)
	 *     //     SERIALIZE (go);
	 *     void Serialize(writer);
	 *     
	 *     // This method should read
	 *     //   READ_STRING (_blockName);
	 *     //   SET (block) = (BlockManager.GetBlock(_blockName));
	 *     //   READ_STRING (state);
	 *     //   IF (block.prefab != null)
	 *     //     INSTANTIATE (block.prefab) -> (_prefab);
	 *     //     DESERIALIZE (_prefab);
	 *     void Deserialize(reader);
	 */

	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
	public class TileLevel : MonoBehaviour, IDataHolder {
		public static int FLOOR => (int)IBlockGroup.Floor;
		public static int WALLS => (int)IBlockGroup.Wall;
		public static int INTER => (int)IBlockGroup.Interactable;

		public Tilemap tilemap;
		public TilemapRenderer tilemap_render;
		public TilemapCollider2D tilemap_collider;
		public Transform tilemap_objects;
		public BlockMap blockMap;

		public int index;

		internal void SetLevel(int index) {
			this.index = index;
			tilemap_render.sortingOrder = index;
		}

		public void SetBlock(int x, int y, BlockObject block) {
			SetTile(x, y, (int)block.blockGroup, block);
		}

		public void RemoveBlock(int x, int y, int layer) {
			SetTile(x, y, layer, null);
		}


		public BlockObject GetFloor(int x, int y) {
			return GetTile(x, y, FLOOR);
		}

		public BlockObject GetWall(int x, int y) {
			return GetTile(x, y, WALLS);
		}

		public BlockObject GetInteractable(int x, int y) {
			return GetTile(x, y, INTER);
		}

		private void SetTile(int x, int y, int level, BlockObject block) {
			Vector3Int pos = new Vector3Int(x, y, level);

			blockMap.Set(pos, block, () => {
				GameObject obj = GameObject.Instantiate(block.prefab, pos, Quaternion.identity, tilemap_objects);
				BlockGameObject bgo = obj.GetComponent<BlockGameObject>();
				bgo.Initialize(this, pos, block.defaultState);
				return bgo;
			});

			SetOnlyTile(pos, block);
		}

		private void SetOnlyTile(Vector3Int position, BlockObject block) {
			if(block == null) {
				tilemap.SetTile(position, null);
			} else {
				tilemap.SetTile(position, block.tile);
				if(position.z == FLOOR) {
					tilemap.SetColliderType(position, Tile.ColliderType.None);
				}
			}
		}

		private BlockObject GetTile(int x, int y, int level) {
			if(blockMap.Get(new Vector3Int(x, y, level), out BlockMapEntry entry)) {
				return entry.block;
			}

			return null;
		}

		public void SetCollision(bool enable) {
			tilemap_collider.enabled = enable;
		}

		public void Deserialize(BinaryReader reader) {
			// Clear the tilemap
			tilemap.ClearAllTiles();

			// This will read all blocks but they will not be initialized.
			blockMap.Deserialize(reader);

			// Initialize all blocks
			foreach(Vector3Int position in blockMap.Keys) {
				if(blockMap.Get(position, out var entry)) {
					if(entry.go != null) {
						entry.go.Initialize(this, position, entry.state);
						entry.go.transform.parent = tilemap_objects;
					}
					
					SetOnlyTile(position, entry.block);
				}
			}
		}

		public void Serialize(BinaryWriter writer) {
			// We dont really need to do anything really here
			blockMap.Serialize(writer);
		}


		#if UNITY_EDITOR
		[ToggleButtonAttribute("Deserialize Level")]
		public bool toggle_Deserialize;
		[ToggleButtonAttribute("Serialize Level")]
		public bool toggle_Serialize;

		void OnRenderObject() {
			// This is called inside the editor
			
			if(toggle_Serialize) {
				toggle_Serialize = false;
				
				string path = Application.dataPath + $"/Temp/level_{index}.dat";

				try {
					FileStream writeStream = new FileStream(path, FileMode.Create);
					BinaryWriter writer = new BinaryWriter(writeStream);
					Serialize(writer);
					writeStream.Dispose();
					writeStream.Close();
				} catch(Exception e) {
					Debug.LogException(e);
				}
			}

			if(toggle_Deserialize) {
				toggle_Deserialize = false;
				
				string path = Application.dataPath + $"/Temp/level_{index}.dat";

				if(File.Exists(path)) {
					try {
						FileStream readerStream = new FileStream(path, FileMode.Open);
						BinaryReader reader = new BinaryReader(readerStream);
						Deserialize(reader);
						readerStream.Dispose();
						readerStream.Close();
					} catch(Exception e) {
						Debug.LogException(e);
					}
				}
			}

		}

		#endif
	}

	[Serializable]
	public class BlockMapEntry : IDataHolder {
		public BlockObject block;
		public string state;
		public BlockGameObject go;

		public string GetSerializedName() {
			string result = block.blockName;

			if(block.hasStates) {
				if(block.prefab != null) {
					result += $":{go.State}";
				} else {
					result += $":{state}";
				}
			}

			return result;
		}

		public void Deserialize(BinaryReader reader) {
			string _serializedName = reader.ReadString();
			string _blockName = BlockManager.GetNamespaceName(_serializedName);
			string _state = BlockManager.GetNamespaceKey(_serializedName);
			block = BlockManager.GetBlock(_blockName);
			state = _state;
			
			if(block.prefab != null) {
				GameObject obj = GameObject.Instantiate(block.prefab);
				go = obj.GetComponent<BlockGameObject>();
				go.Deserialize(reader);
			}
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(GetSerializedName());

			if(block.prefab != null)
				go.Serialize(writer);
		}
	}
	
	[Serializable]
	public class BlockMap : IDataHolder, ISerializationCallbackReceiver {
		private readonly Dictionary<Vector3Int, BlockMapEntry> _Blocks = new Dictionary<Vector3Int, BlockMapEntry>();
		[SerializeField] List<Vector3Int> _Keys;
		[SerializeField] List<BlockMapEntry> _Values;
		
		public int Count => _Blocks.Count;
		public IEnumerable<Vector3Int> Keys => _Blocks.Keys;
		public IEnumerable<BlockMapEntry> Values => _Blocks.Values;

		public void OnBeforeSerialize() {
			_Keys = _Blocks.Keys.ToList();
			_Values = _Blocks.Values.ToList();
		}

		public void OnAfterDeserialize() {
			// Clear all residual data in the Dictionary
			_Blocks.Clear();

			for(int i = 0, len = _Keys.Count; i < len; i++) {
				var key = _Keys[i];
				var entry = _Values[i];
				if(key == null || entry == null) continue;

				_Blocks.Add(_Keys[i], _Values[i]);
			}
			_Keys.Clear();
			_Values.Clear();
		}

		public void Clear() {
			foreach(var position in _Keys) {
				if(Get(position, out BlockMapEntry entry)) {
					if(entry.go != null)
						entry.go.Remove();
				}
			}

			_Blocks.Clear();
		}

		public void Remove(Vector3Int position) {
			if(Get(position, out BlockMapEntry entry)) {
				if(entry.go != null)
					entry.go.Remove();
				_Blocks.Remove(position);
			}
		}

		public void Set(Vector3Int position, BlockObject block, Func<BlockGameObject> prefab) {
			Remove(position);
			if(block == null) return;

			BlockMapEntry entry = new BlockMapEntry() {
				block = block,
				state = block.defaultState
			};

			if(block.prefab != null) {
				entry.go = prefab.Invoke();
			}

			_Blocks.Add(position, entry);
		}

		public bool Get(Vector3Int position, out BlockMapEntry value) {
			return _Blocks.TryGetValue(position, out value);
		}

		public void Deserialize(BinaryReader reader) {
			// Remove prior data
			Clear();
			int count = reader.ReadInt32();

			for(int i = 0; i < count; i++) {
				Vector3Int position = reader.ReadVec3Int();
				BlockMapEntry entry = new BlockMapEntry();
				entry.Deserialize(reader);
				_Blocks.Add(position, entry);
			}
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(_Blocks.Count);
			foreach(KeyValuePair<Vector3Int, BlockMapEntry> entry in _Blocks) {
				writer.WriteVec3Int(entry.Key);
				entry.Value.Serialize(writer);
			}
		}
	}

	/*
	[Serializable]
	public class BlockMap : ISerializationCallbackReceiver {
		private readonly Dictionary<Vector3Int, BlockGameObject> _Dictionary = new Dictionary<Vector3Int, BlockGameObject>();
		[SerializeField] List<Vector3Int> _Keys;
		[SerializeField] List<BlockGameObject> _Values;

		public int Count => _Dictionary.Count;
		public IEnumerable<Vector3Int> Keys => _Dictionary.Keys;
		public IEnumerable<BlockGameObject> Values => _Dictionary.Values;

		public void OnBeforeSerialize() {
			_Keys = _Dictionary.Keys.ToList();
			_Values = _Dictionary.Values.ToList();
		}

		public void OnAfterDeserialize() {
			for(int i = 0, len = _Keys.Count; i < len; i++) {
				_Dictionary.Add(_Keys[i], _Values[i]);
			}
			_Keys.Clear();
			_Values.Clear();
		}

		public BlockGameObject Get(Vector3Int position) {
			_Dictionary.TryGetValue(position, out BlockGameObject result);
			return result;
		}

		public void Set(Vector3Int position, BlockGameObject bgo) {
			_Dictionary.TryGetValue(position, out BlockGameObject value);
			if(value != null) value.Remove();
			_Dictionary.Remove(position);
			
			if(bgo != null) {
				_Dictionary.Add(position, bgo);
			}
		}
	}
	*/
}
