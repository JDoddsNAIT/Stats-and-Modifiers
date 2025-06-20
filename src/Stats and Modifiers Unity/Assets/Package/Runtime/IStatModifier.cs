using System;

namespace JDodds.Stats
{
	/// <summary>
	/// Used for interfacing with types that can modify <typeparamref name="T"/> stats.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IStatModifier<T> :
		IEquatable<IStatModifier<T>>,
		IComparable<IStatModifier<T>>
		where T : struct
	{
		/// <summary>
		/// Globally unique identifier for this modifier.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Modifiers with a lower order will be applied first.
		/// </summary>
		int Order { get; }

		int IComparable<IStatModifier<T>>.CompareTo(IStatModifier<T> other)
		{
			return other switch {
				null => 1,
				_ => this.Order.CompareTo(other.Order),
			};
		}

		bool IEquatable<IStatModifier<T>>.Equals(IStatModifier<T> other)
		{
			return other is IStatModifier<T> modifier && this.Id.Equals(modifier.Id);
		}

		/// <summary>
		/// This event should be invoked whenever changes are made to this object that would affect the modified value.
		/// </summary>
		event Action OnValueChanged;

		/// <summary>
		/// This method is for handling the modification of the base value.
		/// </summary>
		/// <param name="query"></param>
		void HandleQuery(ref StatQuery<T> query);
	}
}
