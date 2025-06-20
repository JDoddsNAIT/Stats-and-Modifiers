using System;
using Unity.Properties;
using UnityEngine;

namespace JDodds.Stats.Modifiers
{
	[Serializable]
	public class Multiplier : INumericModifier
	{
		public Guid Id { get; }

		private int _order;

		public int Order {
			get => _order;
			set {
				if (_order != value)
				{
					_order = value;
					BroadcastChange();
				}
			}
		}

		private bool _enabled;

		[CreateProperty]
		public bool Enabled {
			get => _enabled;
			set {
				if (_enabled != value)
				{
					_enabled = value;
					BroadcastChange();
				}
			}
		}

		private float _amount;

		[CreateProperty]
		public float Amount {
			get => _amount;
			set {
				if (_amount != value)
				{
					_amount = value;
					BroadcastChange();
				}
			}
		}

		public event Action OnValueChanged;

		public Multiplier()
		{
			Id = Guid.NewGuid();
		}

		public Multiplier(float amount, bool enabled = true, int order = 0) : this()
		{
			_amount = amount;
			_enabled = enabled;
			_order = order;
		}

		/// <summary>
		/// Alerts any stats with this modifier that the value must be recalculated.
		/// </summary>
		public void BroadcastChange()
		{
			if (Application.isPlaying)
			{
				OnValueChanged?.Invoke();
			}
		}

		public void HandleQuery(ref StatQuery<int> query)
		{
			if (Enabled)
			{
				query.Value = (int)(query.Value * Amount);
			}
		}

		public void HandleQuery(ref StatQuery<float> query)
		{
			if (Enabled)
			{
				query.Value *= Amount;
			}
		}
	}
}
