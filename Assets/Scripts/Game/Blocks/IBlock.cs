using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Blocks {
	public interface IBlock : IDataHolder {
		/// <summary>
		/// The tile object of this block
		/// </summary>
		BlockObject Tile { get; }

		/// <summary>
		/// The current state of this block
		/// </summary>
		string State { get; }

		/// <summary>
		/// The position of this block
		/// </summary>
		Vector2 Position { get; }

		/// <summary>
		/// Called when the block is started
		/// </summary>
		void OnBlockStart();

		/// <summary>
		/// Called each tick
		/// </summary>
		void Tick();
	}

	public enum IBlockGroup {
		Floor,
		Wall,
		Interactable
	}
}
