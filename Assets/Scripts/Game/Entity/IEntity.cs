using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Entities {
	public interface IEntity : IDataHolder {
		void Tick();

		EntityStatus GetStatus();

		void SetPosition(Vector2 position);
		void SetPosition(Vector2 position, int layer);
		Vector2 GetPosition();

		void SetVelocity(Vector2 velocity);
		Vector2 GetVelocity();

		void SetLayer(int layer);
		int GetLayer();
	}
	
	[Serializable]
	public class EntityStatus : IDataHolder {
		public float Health { get => _health; set => _health = value; }
		public string Name { get => _name; set => _name = value; }

		[SerializeField] private float _health;
		[SerializeField] private string _name;
		
		public void Deserialize(BinaryReader reader) {
			_health = reader.ReadSingle();
			_name = reader.ReadString();
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(_health);
			writer.Write(_name);
		}
	}
}
