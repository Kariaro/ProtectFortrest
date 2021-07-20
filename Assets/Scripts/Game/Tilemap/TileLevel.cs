using ProjectFortrest.Game.Blocks;
using ProjectFortrest.Game.Blocks.Impl;
using ProjectFortrest.Game.Level;
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
		private static readonly Vector3 PREFAB_OFFSET = new Vector3(0.5f, 0.5f);
		private static readonly Vector3 PREFAB_SCALE = new Vector3(1, 1, -1);

		public static int FLOOR => (int)IBlockGroup.Floor;
		public static int WALLS => (int)IBlockGroup.Wall;
		public static int INTER => (int)IBlockGroup.Interactable;

		public Tilemap tilemap;
		public TilemapRenderer tilemap_render;
		public TilemapCollider2D tilemap_collider;
		public Transform tilemap_objects;
		public int index;

		public BlockMap blockMap;

		internal void SetLevel(int index) {
			this.index = index;
			tilemap_render.sortingOrder = index;
		}

		public void RemoveBlock(int x, int y, int z) {
			Vector3Int pos = new Vector3Int(x, y, z);
			blockMap.Remove(pos);
			tilemap.SetTile(pos, null);
		}

		public void PutBlock(int x, int y, BlockObject block) {
			if(block == null) return;
			PutBlock(new Vector3Int(x, y, (int)block.blockGroup), block, block.states[0]);
		}

		public void PutBlock(int x, int y, BlockObject block, string state) {
			if(block == null) return;
			PutBlock(new Vector3Int(x, y, (int)block.blockGroup), block, block.GetState(state));
		}

		public void PutBlock(int x, int y, BlockObject block, int stateIndex) {
			if(block == null) return;
			PutBlock(new Vector3Int(x, y, (int)block.blockGroup), block, block.GetState(stateIndex));
		}

		public void PutBlock(Vector3Int pos, BlockObject block, BlockState state) {
			blockMap.Set(pos, block, () => {
				GameObject obj = GameObject.Instantiate(block.prefab, Vector3.Scale(pos + PREFAB_OFFSET, PREFAB_SCALE), Quaternion.identity, tilemap_objects);
				BlockGameObject bgo = obj.GetComponent<BlockGameObject>();
				bgo.Initialize(this, pos, state.name);
				return bgo;
			});

			SetOnlyTile(pos, block, state);
		}
		
		public void UpdateState(Vector3Int position, BlockObject block, string state) {
			if(blockMap.Get(position, out var entry)) {
				entry.state = state;
				SetOnlyTile(position, block, block.GetState(state));
			}
		}

		private void SetOnlyTile(Vector3Int position, BlockObject block, BlockState state) {
			tilemap.SetTile(position, state?.tile);
			if(block != null) {
				if(position.z == FLOOR) {
					tilemap.SetColliderType(position, Tile.ColliderType.None);
				}
			}
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
						entry.go.transform.position = Vector3.Scale(position + PREFAB_OFFSET, PREFAB_SCALE);
					}
					
					SetOnlyTile(position, entry.block, entry.block.GetState(entry.state));
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
			
			if(block.HasStates()) {
				string _state = (block.prefab != null) ? go.State:state;
				if(_state.Length > 0) {
					result += $":{_state}";
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
				state = block.states[0].name
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
}

/*
namespace ProjectFortrest.Game.Level.Storage {
	[Serializable]
	public class BlockChunk {
		public BlockSlice[] slice = new BlockSlice[16];

		public void PutBlock(int x, int y, int z) {
			GetSlice(z).PutBlock()
		}
	}

	[Serializable]
	public class BlockEntry {
		public BlockObject block;
		public string state;
		public BlockGameObject go;

		public string GetSerializedName() {
			string result = block.blockName;
			
			if(block.HasStates()) {
				string _state = (block.prefab != null) ? go.State:state;
				if(_state.Length > 0) {
					result += $":{_state}";
				}
			}

			return result;
		}
	}

	[Serializable]
	public class BlockSlice : IDataHolder {
		public static readonly int SLICE_SIDE = 16;
		public static readonly int SLICE_SIZE = SLICE_SIDE * SLICE_SIDE;
		public BlockEntry[] data = new BlockEntry[SLICE_SIZE];

		public void Deserialize(BinaryReader reader) {
			for(int i = 0; i < SLICE_SIZE; i++) {
				string _serializedName = reader.ReadString();
				if(_serializedName.Length > 0) {
					data[i] = DeserializeEntry(reader, _serializedName);
				}
			}
		}

		public void Serialize(BinaryWriter writer) {
			for(int i = 0; i < SLICE_SIZE; i++) {
				var entry = data[i];
				if(entry == null) {
					writer.Write((int)0);
				} else {
					writer.Write(entry.GetSerializedName());

					if(entry.block.prefab != null) {
						entry.go.Serialize(writer);
					}
				}
			}
		}

		public BlockEntry DeserializeEntry(BinaryReader reader, string _serializedName) {
			string _blockName = BlockManager.GetNamespaceName(_serializedName);
			string _state = BlockManager.GetNamespaceKey(_serializedName);
			BlockEntry entry = new BlockEntry() {
				block = BlockManager.GetBlock(_blockName),
				state = _state
			};

			if(entry.block.prefab != null) {
				entry.go = GameObject.Instantiate(entry.block.prefab).GetComponent<BlockGameObject>();
				entry.go.Deserialize(reader);
			}

			return entry;
		}
	}
}
*/