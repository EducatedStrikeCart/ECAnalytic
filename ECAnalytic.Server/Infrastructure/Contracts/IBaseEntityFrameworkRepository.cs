namespace ECAnalytic.Server.Infrastructure.Contracts
{
	public interface IBaseEntityFrameworkRepository<TEntity,TModel, TId>
		where TId : IEquatable<TId>
		where TEntity : class, ITableObject<TId>
		where TModel : class, ITableObject<TId>
	{
		Task<TModel> CreateAsync(TModel obj);
		Task<IEnumerable<TModel>> GetMultipleAsync(int page, int countPerPage);
		Task<TModel?> GetAsync(TId id);
		Task RemoveAsync(TModel obj);
		Task SaveChangesAsync();
		Task<TModel?> UpdateAsync(TId id, TModel obj);
	}
}