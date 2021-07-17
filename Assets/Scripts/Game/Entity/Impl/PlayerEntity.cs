using ProjectFortrest.Game.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFortrest.Game.Entity.Impl {
	public class PlayerEntity : Entity {
		
		public override void Deserialize(BinaryReader reader) {
			base.Deserialize(reader);
		}

		public override void Serialize(BinaryWriter writer) {
			base.Serialize(writer);
		}

		public override void Tick() {
			
		}
	}
}
