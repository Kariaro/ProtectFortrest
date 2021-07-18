using ProjectFortrest.Game.Blocks.Impl;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;
using System;
using System.IO;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks {
	public class BlockGameObject : MonoBehaviour, IDataHolder {
		[SerializeField] private Vector3Int _position;
		[SerializeField] private TileLevel _level;
		[SerializeField] private Block _block;
		
		public string State {
			get {
				if(_block == null)
					_block = GetComponent<Block>();

				return _block.State;
			}
		}

		public void Initialize(TileLevel level, Vector3Int position, string state) {
			_level = level;
			_position = position;
			_block = GetComponent<Block>();
			_block.Initialize((Vector2Int)position, state);
		}

		public void UpdateState(string state) {
			_level.UpdateState(_position, _block.Data, state);
		}

		// Called when removed from the tilemap
		public void Remove() {
			#if UNITY_EDITOR
			if(!Application.isPlaying) {
				GameObject.DestroyImmediate(gameObject);
				return;
			}
			#endif

			GameObject.Destroy(gameObject);
		}

		public void Deserialize(BinaryReader reader) {
			int length = reader.ReadInt32();
			_block = GetComponent<Block>();

			try {
				MemoryStream stream = new MemoryStream(reader.ReadBytes(length));
				_block.Deserialize(new BinaryReader(stream));
				stream.Dispose();
				stream.Close();
			} catch(Exception e) {
				Debug.LogException(e);
			}
		}

		public void Serialize(BinaryWriter writer) {
			MemoryStream stream = new MemoryStream();
			_block.Serialize(new BinaryWriter(stream));
			writer.Write((int)stream.Length);
			writer.Write(stream.ToArray());
			stream.Dispose();
			stream.Close();
		}

		void FixedUpdate() {
			if(_block != null)
				_block.Tick();
		}
	}
}
