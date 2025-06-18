namespace JDodds.Stats
{
	public delegate void QueryHandler<T>(ref StatQuery<T> query) where T : struct;

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
