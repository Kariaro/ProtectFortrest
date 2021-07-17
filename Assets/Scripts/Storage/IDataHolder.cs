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

		public static Vector3Int ReadVec3Int(this BinaryReader reader) {
			return new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}

		public static void WriteVector2(this BinaryWriter writer, Vector2 vector) {
			writer.Write(vector.x);
			writer.Write(vector.y);
		}

		public static void WriteVec3Int(this BinaryWriter writer, Vector3Int vector) {
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}
	}
}
