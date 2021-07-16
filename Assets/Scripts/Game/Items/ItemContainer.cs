using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks {
    public class ItemContainer : IDataHolder {
        public IItem[] items;

        public void Deserialize(BinaryReader reader) {
            int len = reader.ReadInt32();
            for(int i = 0; i < len; i++) {
                // ReadItem
            }
        }

        public void Serialize(BinaryWriter writer) {
            writer.Write(items.Length);

            for(int i = 0, len = items.Length; i < len; i++) {
                IItem item = items[i];
                item.Serialize(writer);
            }
        }
    }
}
