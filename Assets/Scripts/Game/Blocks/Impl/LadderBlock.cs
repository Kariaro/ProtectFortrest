using ProjectFortrest.Game.Entity.Impl;
using ProjectFortrest.Game.Items;
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
	[RequireComponent(typeof(BoxCollider2D))]
	public class LadderBlock : Block {
		public override BlockObject Data => BlockManager.GetBlock("LADDER");

		public override void Deserialize(BinaryReader reader) {
			
		}

		public override void Serialize(BinaryWriter writer) {
			
		}

		public override void OnBlockStart() {
			
		}
		
		public override void Tick() {
			
		}


		void OnMouseDown() {
			Collider2D coll = GetComponent<Collider2D>();
			
			List<Collider2D> colliders = new List<Collider2D>();
			coll.GetContacts(colliders);
			foreach(Collider2D c in colliders) {
				ClientPlayerEntity cpe = c.GetComponent<ClientPlayerEntity>();
				if(cpe != null) {
					Debug.Log("Use ladder");
				}
			}
		}
	}
}
