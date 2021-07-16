using ProjectFortrest.Game.Blocks.Impl;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks {
    public class BlockGameObject : MonoBehaviour, IDataHolder {
        [SerializeField] private Vector3Int _position;
        [SerializeField] private TileLevel _level;
        [SerializeField] private Block _block;

        public void Initialize(TileLevel level, Vector3Int position) {
            _level = level;
            _position = position;

            Block block = GetComponent<Block>();
            block.Initialize((Vector2Int)position, "");
            _block = block;
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
            _block.Deserialize(reader);
        }

        public void Serialize(BinaryWriter writer) {
            _block.Serialize(writer);
        }

        void FixedUpdate() {
            if(_block != null)
                _block.Tick();
        }
    }
}
