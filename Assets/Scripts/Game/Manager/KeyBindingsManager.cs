using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectFortrest.Game.Manager {
	public class KeyBindingsManager : IManager {
		public static KeyCode Up => GameManager.Instance.KeyBindings.KEY_UP;
		public static KeyCode Down => GameManager.Instance.KeyBindings.KEY_DOWN;
		public static KeyCode Left => GameManager.Instance.KeyBindings.KEY_LEFT;
		public static KeyCode Right => GameManager.Instance.KeyBindings.KEY_RIGHT;
		public static KeyCode Shift => GameManager.Instance.KeyBindings.KEY_SHIFT;

		public KeyCode KEY_UP = KeyCode.W;
		public KeyCode KEY_DOWN = KeyCode.S;
		public KeyCode KEY_LEFT = KeyCode.D;
		public KeyCode KEY_RIGHT = KeyCode.A;
		public KeyCode KEY_SHIFT = KeyCode.LeftShift;

		public void Init(GameManager manager) {
			
		}

		public void OnStart() {
			
		}
	}
}
