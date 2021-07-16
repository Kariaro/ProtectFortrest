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
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/ItemDatabase", order = 1)]
    public class ItemDatabase : ScriptableObject {
        public List<ItemObject> elements;
    }

    public static class ItemDatabaseExtension {
        public static ItemObject GetFromName(this ItemDatabase database, string name) {
            foreach(ItemObject item in database.elements) {
                if(item.name.Equals(name))
                    return item;
            }

            return null;
        }
    }
}
