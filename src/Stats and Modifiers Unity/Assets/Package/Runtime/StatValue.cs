using System;
using System.Collections.Generic;
using System.Linq;

namespace JDodds.Stats
{
	/// <summary>
	/// Base class for <typeparamref name="T"/> variables that can use modifiers.
	/// </summary>
	/// <remarks>
	/// This class uses a cache for the <see cref="Value"/> to reduce event calls. Use the <see cref="SetDirty"/> method to update the cache, or <see cref="GetModifiedValue"/> to bypass the dirty check.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class StatValue<T> : IStatValue<T> where T : struct
	{
		protected bool _dirty;
		protected T _cachedValue;

		// This field is not serialized so sub-classes can override the property to add formatting attributes
		// such as [Range] or [Min] to a field with the same name.
		[NonSerialized] private T _baseValue;

		protected List<IStatModifier<T>> _modifiers;

		protected QueryHandler<T> _query;

		public T Value => _dirty ? GetModifiedValue() : _cachedValue;
		public virtual T BaseValue { get => _baseValue; set { _baseValue = value; this.SetDirty(); } }
		public IReadOnlyList<IStatModifier<T>> Modifiers => _modifiers;

		#region Constructors
		/// <summary>
		/// Creates a new Stat Value of type <typeparamref name="T"/> with the default base value.
		/// </summary>
		public StatValue() : this(baseValue: default) { }

		/// <summary>
		/// Creates a new Stat Value of type <typeparamref name="T"/> with the given <paramref name="baseValue"/>.
		/// </summary>
		/// <param name="baseValue"></param>
		public StatValue(T baseValue)
		{
			this.BaseValue = baseValue;
			_modifiers = new();
		}

		/// <summary>
		/// Creates a new Stat Value of type <typeparamref name="T"/> with the given <paramref name="baseValue"/> and <paramref name="modifiers"/>.
		/// </summary>
		/// <param name="baseValue"></param>
		/// <param name="modifiers"></param>
		public StatValue(T baseValue, params IStatModifier<T>[] modifiers) : this(baseValue)
		{
			AddModifier(modifiers);
		}

		/// <summary>
		/// Creates a new Stat Value of type <typeparamref name="T"/> with the given <paramref name="baseValue"/> and <paramref name="modifiers"/>.
		/// </summary>
		/// <param name="baseValue"></param>
		/// <param name="modifiers"></param>
		public StatValue(T baseValue, IEnumerable<IStatModifier<T>> modifiers) : this(baseValue)
		{
			AddModifier(modifiers);
		}

		~StatValue()
		{
			this.ClearModifiers();
		}
		#endregion

		public void SetDirty() => _dirty |= true;

		public T GetModifiedValue()
		{
			var query = new StatQuery<T>(BaseValue);
			_query?.Invoke(ref query);
			_cachedValue = query.Value;
			_dirty = false;
			return _cachedValue;
		}

		public void SetBaseValue(in T value) => BaseValue = value;

		#region Add Modifier
		public void AddModifier(in IStatModifier<T> modifier)
		{
			if (this.AddModifierNoSort(modifier))
			{
				this.SortModifiers();
			}
		}

		/// <summary>
		/// Adds the given <paramref name="modifiers"/> to this <see cref="StatValue{T}"/>.
		/// </summary>
		/// <param name="modifiers"></param>
		public void AddModifier(params IStatModifier<T>[] modifiers)
		{
			bool requireSort = false;
			for (int i = 0; i < modifiers.Length; i++)
			{
				requireSort |= AddModifierNoSort(modifiers[i]);
			}

			if (requireSort)
			{
				this.SortModifiers();
			}
		}

		/// <summary>
		/// <inheritdoc cref="AddModifier(IStatModifier{T}[])"/>
		/// </summary>
		/// <param name="modifiers"></param>
		public void AddModifier(IEnumerable<IStatModifier<T>> modifiers)
		{
			bool requireSort = false;
			foreach (var modifier in modifiers)
			{
				requireSort |= AddModifierNoSort(modifier);
			}

			if (requireSort)
			{
				this.SortModifiers();
			}
		}

		private bool AddModifierNoSort(in IStatModifier<T> modifier)
		{
			bool canAdd = !_modifiers.Contains(modifier);
			if (canAdd)
			{
				_modifiers.Add(modifier);
				_query += modifier.HandleQuery;
				modifier.OnValueChanged += SetDirty;
			}
			return canAdd;
		}
		#endregion

		#region Remove Modifier
		public void RemoveModifier(in IStatModifier<T> modifier)
		{
			if (this.RemoveModifierNoSort(modifier))
			{
				this.SortModifiers();
			}
		}

		/// <summary>
		/// Removes the given <paramref name="modifiers"/> from this <see cref="StatValue{T}"/>.
		/// </summary>
		/// <param name="modifiers"></param>
		public void RemoveModifier(params IStatModifier<T>[] modifiers)
		{
			bool requireSort = false;
			for (int i = 0; i < modifiers.Length; i++)
			{
				requireSort |= RemoveModifierNoSort(modifiers[i]);
			}

			if (requireSort)
			{
				this.SortModifiers();
			}
		}

		/// <summary>
		/// <inheritdoc cref="RemoveModifier(IStatModifier{T}[])"/>
		/// </summary>
		/// <param name="modifiers"></param>
		public void RemoveModifier(IEnumerable<IStatModifier<T>> modifiers)
		{
			bool requireSort = false;
			foreach (var modifier in modifiers)
			{
				requireSort |= RemoveModifierNoSort(modifier);
			}

			if (requireSort)
			{
				this.SortModifiers();
			}
		}

		private bool RemoveModifierNoSort(in IStatModifier<T> modifier)
		{
			bool canRemove = _modifiers.Contains(modifier);
			if (canRemove)
			{
				_modifiers.Remove(modifier);
				_query -= modifier.HandleQuery;
				modifier.OnValueChanged -= SetDirty;
			}
			return canRemove;
		}
		#endregion

		/// <summary>
		/// Removes all modifiers from this <see cref="StatValue{T}"/>.
		/// </summary>
		public void ClearModifiers()
		{
			RemoveModifier(_modifiers);
		}

		private void SortModifiers()
		{
			for (int i = _modifiers.Count - 1; i >= 0; i--)
			{
				if (_modifiers[i] is null)
				{
					_modifiers.RemoveAt(i);
				}
			}
			_modifiers.Sort();
			this.SetDirty();
		}

		public static implicit operator T(StatValue<T> stat) => stat.Value;

		public static explicit operator StatValue<T>(T value) => new(value);
	}
}
