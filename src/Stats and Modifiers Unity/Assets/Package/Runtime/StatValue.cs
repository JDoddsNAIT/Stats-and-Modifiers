using System;
using System.Collections.Generic;
using UnityEngine;

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

		// This field is not serialized so sub-classes can override the property to add formatting attributes to a field with the same name.
		[NonSerialized] private T _baseValue;

		protected List<IStatModifier<T>> _modifiers;

		protected QueryHandler<T> _query;

		public T Value => _dirty ? GetModifiedValue() : _cachedValue;
		public virtual T BaseValue { get => _baseValue; set { _baseValue = value; this.SetDirty(); } }
		public IReadOnlyList<IStatModifier<T>> Modifiers => _modifiers;

		public StatValue() : this(baseValue: default(T)) { }

		public StatValue(T baseValue)
		{
			this.BaseValue = baseValue;
			_modifiers = new();
		}

		public StatValue(T baseValue, params IStatModifier<T>[] modifiers) : this(baseValue)
		{
			AddModifier(modifiers);
		}

		public StatValue(T baseValue, IEnumerable<IStatModifier<T>> modifiers) : this(baseValue)
		{
			AddModifier(modifiers);
		}

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

		public void AddModifier(in IStatModifier<T> modifier)
		{
			if (!_modifiers.Contains(modifier))
			{
				SetDirty();
				_modifiers.Add(modifier);
				_query += modifier.HandleQuery;
				modifier.OnValueChanged += SetDirty;
			}
		}

		public void AddModifier(params IStatModifier<T>[] modifiers)
		{
			for (int i = 0; i < modifiers.Length; i++)
			{
				this.AddModifier(modifiers[i]);
			}
		}

		public void AddModifier(IEnumerable<IStatModifier<T>> modifiers)
		{
			foreach (var modifier in modifiers)
			{
				this.AddModifier(modifier);
			}
		}

		public void RemoveModifier(in IStatModifier<T> modifier)
		{
			if (_modifiers.Contains(modifier))
			{
				SetDirty();
				_modifiers.Remove(modifier);
				_query -= modifier.HandleQuery;
				modifier.OnValueChanged -= SetDirty;
			}
		}

		public void RemoveModifier(params IStatModifier<T>[] modifiers)
		{
			for (int i = 0; i < modifiers.Length; i++)
			{
				this.RemoveModifier(modifiers[i]);
			}
		}

		public void RemoveModifier(IEnumerable<IStatModifier<T>> modifiers)
		{
			foreach (var modifier in modifiers)
			{
				this.RemoveModifier(modifier);
			}
		}

		public void ClearModifiers()
		{
			RemoveModifier(_modifiers);
		}
	}
}
