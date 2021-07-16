using ProjectFortrest.Game.Blocks;
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
        public Sprite sprite;
        public string itemName;
        public string defaultState;
        public IItemGroup itemGroup;
        public GameObject prefab;
        public bool interactable;
        public bool hasStates;
        public ItemState[] states;
    }

    [Serializable]
    public class ItemState {
        public string name;
        public Sprite sprite;
    }
}
