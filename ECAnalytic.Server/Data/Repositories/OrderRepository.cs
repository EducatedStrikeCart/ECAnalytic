using AutoMapper;
using ECAnalytic.Server.Infrastructure.Entities;

namespace ECAnalytic.Server.Data.Repositories
{
	public class OrderRepository : BaseEntityFrameworkRepository<Order, OrderDto, Guid>
	{
		public OrderRepository(ECADbContext context, IMapper mapper) : base(context, mapper)
		{
		}
	}
}
