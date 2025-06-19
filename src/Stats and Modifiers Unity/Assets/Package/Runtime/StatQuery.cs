namespace JDodds.Stats
{
	/// <summary>
	/// Delegate used by modifiers to modify a <typeparamref name="T"/> value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="query"></param>
	public delegate void QueryHandler<T>(ref StatQuery<T> query) where T : struct;

	/// <summary>
	/// Represents the query for the value of a <typeparamref name="T"/> stat.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct StatQuery<T> where T : struct
	{
		/// <summary>
		/// The base value. (Read only)
		/// </summary>
		public T BaseValue { get; }
		/// <summary>
		/// The current value being modified.
		/// </summary>
		public T Value { get; set; }

		public StatQuery(T baseValue)
		{
			this.BaseValue = baseValue;
			this.Value = baseValue;
		}
	}
}
