using System;
using Unity.Properties;
using UnityEngine;

namespace JDodds.Stats.Modifiers
{
	[Serializable]
	public class Multiplier : INumericModifier
	{
		public int Order { get; }

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
			throw new NotImplementedException();
		}

		public void HandleQuery(ref StatQuery<float> query)
		{
			throw new NotImplementedException();
		}
	}
}
