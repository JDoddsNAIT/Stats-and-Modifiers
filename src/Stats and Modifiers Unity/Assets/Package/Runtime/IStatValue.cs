using UnityEngine;

namespace JDodds.Stats
{
	public interface IStatValue<T> where T : struct
	{
		/// <summary>
		/// Gets the base value with all modifiers applied.
		/// </summary>
		/// <returns></returns>
		T GetModifiedValue();

		/// <summary>
		/// Sets the base value of this stat.
		/// </summary>
		/// <param name="value"></param>
		void SetBaseValue(in T value);

		/// <summary>
		/// Adds the given <paramref name="modifier"/> to this stat.
		/// </summary>
		/// <param name="modifier"></param>
		void AddModifier(in IStatModifier<T> modifier);

		/// <summary>
		/// Removes the given modifier from this stat.
		/// </summary>
		/// <param name="modifier"></param>
		void RemoveModifier(in IStatModifier<T> modifier);
	}
}
