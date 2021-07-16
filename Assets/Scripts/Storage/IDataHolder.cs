using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Storage {
    public interface IDataHolder {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }

    public static class BinaryIOExtension {
        public static Vector2 ReadVector2(this BinaryReader reader) {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static void WriteVector2(this BinaryWriter writer, Vector2 vector) {
            writer.Write(vector.x);
            writer.Write(vector.y);
        }
    }
}
