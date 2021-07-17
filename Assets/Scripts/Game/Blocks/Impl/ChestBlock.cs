using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Manager;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks.Impl {
	public class ChestBlock : Block {
	public override BlockObject Tile => BlockManager.GetBlock("CHEST");

		public List<IItem> Inventory;

		public override void Deserialize(BinaryReader reader) {
			
		}

		public override void Serialize(BinaryWriter writer) {
			
		}

		public override void OnBlockStart() {
			
		}
		
		public override void Tick() {
			
		}
	}
}
