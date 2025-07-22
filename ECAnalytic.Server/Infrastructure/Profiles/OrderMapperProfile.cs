using AutoMapper;
using ECAnalytic.Server.Infrastructure.Entities;

namespace ECAnalytic.Server.Infrastructure.Profiles
{
	public class OrderMapperProfile:Profile
	{
		public OrderMapperProfile()
		{
			CreateMap<Order, OrderDto>().ReverseMap();
		}
	}
}
