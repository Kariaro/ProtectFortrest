using ProjectFortrest.Game.Level;
using ProjectFortrest.Storage;

namespace ProjectFortrest.Game.Items {
	public interface IItem : IDataHolder {
		/// <summary>
		/// The item data object
		/// </summary>
		ItemObject Data { get; }
		
		/// <summary>
		/// The max stack of this item
		/// </summary>
		int MaxStack { get; }

		/// <summary>
		/// Called each tick
		/// </summary>
		void Tick();
	}

	public enum IItemGroup {
		Default
	}
}
