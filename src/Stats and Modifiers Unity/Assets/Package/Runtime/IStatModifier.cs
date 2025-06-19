using UnityEngine;

namespace JDodds.Stats
{
	/// <summary>
	/// Used for interfacing with types that can modify <typeparamref name="T"/> stats.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IStatModifier<T> where T : struct
	{
		/// <summary>
		/// This event should be invoked whenever changes are made to this object that would affect the modified value.
		/// </summary>
		event System.Action OnValueChanged;

		/// <summary>
		/// This method is for handling the modification of the base value.
		/// </summary>
		/// <param name="query"></param>
		void HandleQuery(ref StatQuery<T> query);
	}
}
