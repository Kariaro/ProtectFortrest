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
    public interface IItem : IDataHolder {
        ItemObject Item { get; }
        int MaxStack { get; }

        void Tick();
    }

    public enum IItemGroup {
        Default
    }
}
