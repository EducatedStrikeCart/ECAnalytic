using ECAnalytic.Server.Infrastructure.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ECAnalytic.Server.Infrastructure.Entities
{
	public class OrderDto : ITableObject<Guid>
	{
		public Guid Id { get; set; }
		public DateTime OrderDate { get; set; }
		[Required]
		public int UserId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal TotalAmount { get; set; }
		public string Country { get; set; }
		public string City {  get; set; }

		public OrderDto(Guid id, DateTime orderDate, int userId, int productId, int quantity, decimal price, decimal totalAmount, string country, string city)
		{
			Id = id;
			OrderDate = orderDate;
			UserId = userId;
			ProductId = productId;
			Quantity = quantity;
			Price = price;
			TotalAmount = totalAmount;
			Country = country;
			City = city;
		}
	}
}
