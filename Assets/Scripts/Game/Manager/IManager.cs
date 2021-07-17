using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFortrest.Game.Manager {
	public interface IManager {
		void Init(GameManager manager);
		void OnStart();
	}
}
