using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks.Impl {
	[RequireComponent(typeof(BlockGameObject))]
	public class Block : MonoBehaviour, IBlock {
		public virtual BlockObject Tile { get; }
		public Vector2 Position => _position;
		public string State => _state;

		[SerializeField] private Vector2 _position;
		[SerializeField] private string _state;
		
		public void Initialize(Vector2 position, string state) {
			_position = position;
			_state = state;

			OnBlockStart();
		}

		public virtual void OnBlockStart() {
			
		}

		public void SetState(string state) {
			if(!Tile.hasStates) return;
			
			// Cache information about the block object to make state lookup faster.
			// Check if states is a valid state of this tile.
			_state = state;
		}

		public virtual void Deserialize(BinaryReader reader) {}
		public virtual void Serialize(BinaryWriter writer) {}

		public virtual void Tick() {
			
		}
	}
}
