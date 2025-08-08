using AutoMapper;
using ECAnalytic.Server.Data;
using ECAnalytic.Server.Infrastructure.Contracts;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace ECAnalytic.Server.Data.Repositories
{
	public abstract class BaseEntityFrameworkRepository<TEntity, TModel, TId>: IBaseEntityFrameworkRepository<TEntity, TModel, TId>
		where TId : IEquatable<TId>
		where TEntity : class, ITableObject<TId>
		where TModel : class, ITableObject<TId>
	{
		// Keep these fields as `protected` in case they need to be accessed by derived classes
		protected readonly ECADbContext _context;
		protected readonly IMapper _mapper;

		public BaseEntityFrameworkRepository(ECADbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<TModel> CreateAsync(TModel obj)
		{
			var entity = _mapper.Map<TEntity>(obj);
			await _context.AddAsync(entity);
			obj.Id = entity.Id;
			return obj;
		}

		public async Task RemoveAsync(TModel obj)
		{
			var entity = _mapper.Map<TEntity>(obj);
			await _context.FindAsync<TEntity>(entity);
			_context.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<TModel>> GetMultipleAsync(int page, int countPerPage)
		{
			return await _context
							.Set<TEntity>()
							.Skip(page * countPerPage)
							.Take(countPerPage)
							.Select(
								entity => _mapper.Map<TModel>(entity)
								)
							.ToListAsync();
		}

		public async Task<TModel?> GetAsync(TId id)
		{
			var entity = await _context.FindAsync<TEntity>(id);
			if (entity == null) return null;
			var model = _mapper.Map<TModel>(entity);
			await _context.SaveChangesAsync();

			return model;

		}

		public async Task<TModel?> UpdateAsync(TId id, TModel obj)
		{
			var entity = await _context.FindAsync<TEntity>(id);
			if (entity == null)
			{
				return null;
			} else
			{
				_mapper.Map<TEntity>(obj);
				await _context.SaveChangesAsync();
				return obj;
			}
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
