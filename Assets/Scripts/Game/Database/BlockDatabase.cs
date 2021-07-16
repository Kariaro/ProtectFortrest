using ProjectFortrest.Game.Blocks;
using ProjectFortrest.Game.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProjectFortrest.Game.Database {
    [CreateAssetMenu(fileName = "BlockDatabase", menuName = "Database/BlockDatabase", order = 1)]
    public class BlockDatabase : ScriptableObject {
        public List<BlockObject> elements;
    }

    public static class BlockDatabaseExtension {
        public static BlockObject GetFromName(this BlockDatabase database, string name) {
            foreach(BlockObject block in database.elements) {
                if(block.name.Equals(name))
                    return block;
            }

            return null;
        }
    }
}
