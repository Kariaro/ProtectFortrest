using ProjectFortrest.Game.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProjectFortrest.Game.Level {
    public class TileLevel : MonoBehaviour {
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

        public void SetFloor(int x, int y, BlockObject block) {
            SetTile(x, y, FLOOR, block);
        }

        public void SetWall(int x, int y, BlockObject block) {
            SetTile(x, y, WALLS, block);
        }

        public void SetInteractable(int x, int y, BlockObject block) {
            SetTile(x, y, INTER, block);
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

        public GameObject GetObject(int x, int y, int layer) {
            return tilemap.GetInstantiatedObject(new Vector3Int(x, y, layer));
        }


        private void SetTile(int x, int y, int level, BlockObject block) {
            Vector3Int pos = new Vector3Int(x, y, level);
            if(block == null) {
                tilemap.SetTile(pos, null);
                blockMap.Set(pos, null);
            } else {
                tilemap.SetTile(pos, block.tile);
                if(level == FLOOR) {
                    tilemap.SetColliderType(pos, Tile.ColliderType.None);
                }
                
                if(block.prefab != null) {
                    GameObject obj = GameObject.Instantiate(block.prefab, pos, Quaternion.identity, tilemap_objects);
                    BlockGameObject bgo = obj.GetComponent<BlockGameObject>();
                    bgo.Initialize(this, pos);
                    blockMap.Set(pos, bgo);
                }
            }
        }

        private BlockObject GetTile(int x, int y, int level) {
            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, level));
            return GameManager.Instance.BlockManager.GetFromTile(tile);
        }

        public void SetCollision(bool enable) {
            tilemap_collider.enabled = enable;
        }
    }

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
}
