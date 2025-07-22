namespace ECAnalytic.Server.Infrastructure.Contracts
{
	public interface ITableObject<TId> where TId : IEquatable<TId>
	{
		// Type of Id
		TId Id { get; set; }

	}
}
