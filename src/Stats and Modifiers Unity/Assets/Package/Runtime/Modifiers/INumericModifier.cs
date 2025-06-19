using UnityEngine;

namespace JDodds.Stats.Modifiers
{
	public interface INumericModifier :
		IStatModifier<int>,
		IStatModifier<float>
	{

	}

	public interface IVectorModifier :
		IStatModifier<Vector2>,
		IStatModifier<Vector3>
	{

	}
}
