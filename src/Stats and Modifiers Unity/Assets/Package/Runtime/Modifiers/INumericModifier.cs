using UnityEngine;

namespace JDodds.Stats.Modifiers
{
	/// <summary>
	/// Template interface for modifiers of <see langword="int"/> or <see langword="float"/> values.
	/// </summary>
	public interface INumericModifier :
		IStatModifier<int>,
		IStatModifier<float>
	{

	}

	/// <summary>
	/// Template interface for modifiers of <see cref="Vector2"/> or <see cref="Vector3"/> values.
	/// </summary>
	public interface IVectorModifier :
		IStatModifier<Vector2>,
		IStatModifier<Vector3>
	{

	}
}
