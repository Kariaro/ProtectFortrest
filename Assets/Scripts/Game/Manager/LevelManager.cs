using ProjectFortrest.Game.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Manager {
    public class LevelManager : IManager {
        private GameObject _worldGrid; 
        private GameObject WorldGrid {
            get {
                if(_worldGrid != null) return _worldGrid;
                return _worldGrid = GameObject.FindGameObjectWithTag("WorldGrid");
            }
        }

        [SerializeField] private TileLevel[] levels = new TileLevel[32];

        public TileLevel GetLevel(int index) {
            TileLevel level = levels[index];
            if(level == null) {
                #if UNITY_EDITOR
                    // If we are inside the unity editor we want to grab the previous
                    // Level instance if it exists. This is because when we reload scritps
                    // that reference is lost and we want to keep it.
                    GameObject test = GameObject.Find($"Level({index})");
                    if(test != null) {
                        level = test.GetComponent<TileLevel>();
                        level.name = $"Level({index})";
                        level.SetLevel(index);
                        levels[index] = level;
                        return level;
                    }
                #endif
                
                GameObject clone = GameObject.Instantiate(GameManager.Instance.TileLevelPrefab);
                clone.transform.parent = WorldGrid.transform;

                level = clone.GetComponent<TileLevel>();
                level.name = $"Level({index})";
                level.SetLevel(index);
                levels[index] = level;
            }
            return level;
        }

        public void Init(GameManager manager) {
            
        }

        public void OnStart() {
            GetLevel(1);
            GetLevel(2);
            GetLevel(3);
        }
    }
}
