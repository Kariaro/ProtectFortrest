using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Items.Impl {
	public class Item : MonoBehaviour, IItem {
		public virtual ItemObject Data { get; }
		public virtual int MaxStack { get; }
		
		public virtual void Deserialize(BinaryReader reader) {}
		public virtual void Serialize(BinaryWriter writer) {}

		public virtual void Tick() {
			
		}
	}
}
