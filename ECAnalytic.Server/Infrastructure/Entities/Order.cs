using ECAnalytic.Server.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECAnalytic.Server.Infrastructure.Entities
{
	public class Order : ITableObject<Guid>
	{
		// sample
		// b02179a7-5ed3-4e8f-8532-fb7600fb089d,2025-05-31,816651,1004,5,9.04,45.2,Australia,Perth
		public Guid Id { get; set; }
		public DateTime OrderDate { get; set; }
		public int UserId { get; set; }
		public int ProductId { get; set; }

		public int Quantity { get; set; }

		[Precision(14, 2)]
		public decimal Price { get; set; }

		public decimal TotalAmount { get; set; }

		public string Country { get; set; }

		public string City {  get; set; }

		public Order()
		{
		}

		//public Order(Guid id, DateTime orderDate, int userId, int productId, int quantity, decimal price, decimal totalAmount, string country, string city)
		//{
		//	Id = id;
		//	OrderDate = orderDate;
		//	UserId = userId;
		//	ProductId = productId;
		//	Quantity = quantity;
		//	Price = price;
		//	TotalAmount = totalAmount;
		//	Country = country;
		//	City = city;
		//}
	}
}
