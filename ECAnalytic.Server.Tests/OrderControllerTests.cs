using AutoMapper;
using ECAnalytic.Server.Controllers;
using ECAnalytic.Server.Data;
using ECAnalytic.Server.Data.Repositories;
using ECAnalytic.Server.Infrastructure.Contracts;
using ECAnalytic.Server.Infrastructure.Entities;
using ECAnalytic.Server.Infrastructure.Profiles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.Configuration;
using System.Threading.Tasks;

namespace ECAnalytic.Server.Tests
{
	public class OrderControllerTests
	{
		private readonly Mock<IBaseEntityFrameworkRepository<Order, OrderDto, Guid>> mockRepo;
		private readonly List<OrderDto> testOrders;
		private readonly OrderController controller;
		private readonly Guid nonexistantGuid;

		public OrderControllerTests()
		{
			// Test Objects
			testOrders = new List<OrderDto>()
			{
			new (new Guid("89974872-bfc5-4881-bff5-119633d3eb13"), new DateTime(2025, 01, 27),100710,1004,4,9.89M,39.56M,"Brazil","Salvador"),
			new(new Guid("89974872-bfc5-4881-bff5-119633d3eb13"), new DateTime(2025, 01, 27) ,100710,1004,4,9.89M,39.56M,"Brazil","Salvador"),
			new(new Guid("62f059f2-d9d0-4b55-a032-4999dc19eb1c"), new DateTime(2025, 06, 14) ,251220,1001,2,21.72M,43.44M,"India","Bangalore"),
			new(new Guid("ce249b11-6a0b-47c5-a9e6-57a7c271db19"), new DateTime(2025, 03, 03) ,967166,1004,5,8.99M,44.95M,"Brazil","Brasilia"),
			new(new Guid("2d0ccec5-14fd-4543-a94b-0f5a9a19c7a6"), new DateTime(2025, 02, 17) ,574261,1009,3,17.08M,51.24M,"France","Paris")		
			};

			nonexistantGuid = new Guid("89974872-bfc5-4881-bff5-d3eb13119633");

			// Initialize mock repository
			mockRepo = new();

			// Initialize controller
			controller = new OrderController(mockRepo.Object);
		}

		[Fact]
		[Trait("GetMany","Valid output")]
		public async Task GetOrders_MaxItemsOnOnePage_ReturnsIEnumerableOrderDto()
		{
			mockRepo.Setup(repo => repo.GetMultipleAsync(1, 10).Result)
				   .Returns(testOrders);
			var expectedObject = testOrders;

			var response = await controller.GetOrders(1, 10);
			var result = response.Result;

		
			var returnedObj = (result as ObjectResult).Value;
			Assert.IsAssignableFrom<IEnumerable<OrderDto>>(returnedObj);
		}

		[Fact]
		[Trait("GetMany", "Valid output")]

		public async Task GetOrders_GivenPage_ReturnsIEnumerableOrderDto()
		{
			mockRepo.Setup(repo => repo.GetMultipleAsync(2, 2).Result)
				   .Returns(testOrders);
			var expectedObject = testOrders.Skip(2).Take(2).AsEnumerable<OrderDto>;

			var response = await controller.GetOrders(2, 2);
			var result = response.Result;

			Assert.IsType<OkObjectResult>(result);
			var returnedObj = (result as ObjectResult).Value;
			Assert.IsAssignableFrom<IEnumerable<OrderDto>>(returnedObj);
		}

		[Fact]
		[Trait("Get", "Valid output")]

		public async Task GetOrder_GivenId_ReturnsOKOrderDto()
		{
			mockRepo.Setup(repo => repo.GetAsync(testOrders[0].Id).Result)
				   .Returns(testOrders[0]);

			var response = await controller.GetOrder(testOrders[0].Id);
			var result = response.Result;

		
			Assert.IsType<OkObjectResult>(result);
			var returnedObj = (result as ObjectResult).Value;
			Assert.IsType<OrderDto>(returnedObj);
		}

		[Fact]
		[Trait("Get", "Not found")]
		public async Task GetOrder_GivenNonExistantObject_ReturnsNotFound()
		{
			mockRepo.Setup(repo => repo.GetAsync(nonexistantGuid).Result)
				   .Returns((OrderDto)null);

			var response = await controller.GetOrder(nonexistantGuid);
			var result = response.Result;

		
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		[Trait("Put", "Valid return")]
		public async Task PutOrder_GivenObject_ReturnsOkOrderDto()
		{
			var updatedObject = new OrderDto(testOrders[0].Id, new DateTime(2025, 01, 27), 100710, 1004, 10, 9.89M, 39.56M, "Brazil", "Salvador");
			mockRepo.Setup(r => r.UpdateAsync(testOrders[0].Id, updatedObject).Result)
					.Returns(updatedObject);

			var response = await controller.PutOrder(testOrders[0].Id, updatedObject);

			Assert.NotNull(response);
			Assert.IsType<NoContentResult>(response);
		}

		[Fact]
		[Trait("Put", "Not Found")]
		public async Task PutOrder_GivenNonExistantObject_ReturnsNotFound()
		{
			var updatedObject = new OrderDto(testOrders[0].Id, new DateTime(2025, 01, 27), 100710, 1004, 10, 9.89M, 39.56M, "Brazil", "Salvador");
			mockRepo.Setup(r => r.UpdateAsync(testOrders[0].Id, updatedObject).Result)
					.Returns((OrderDto)null);

			var response = await controller.PutOrder(testOrders[0].Id, updatedObject);

			Assert.NotNull(response);
			Assert.IsType<NotFoundResult>(response);
		}

		[Fact]
		[Trait("Patch", "Not Found")]
		public async Task PatchOrder_GivenNonExistantObject_ReturnsNotFound()
		{
			var updatedObject = new OrderDto(testOrders[0].Id, new DateTime(2025, 01, 27), 100710, 1004, 10, 9.89M, 39.56M, "Brazil", "Salvador");
			mockRepo.Setup(repo => repo.GetAsync(nonexistantGuid).Result)
				   .Returns((OrderDto)null);
			mockRepo.Setup(r => r.UpdateAsync(nonexistantGuid, updatedObject).Result)
					.Returns(updatedObject);

			var response = await controller.PatchOrder(nonexistantGuid, new JsonPatchDocument<OrderDto> { });

			Assert.IsType<NotFoundResult>(response);
		}

		[Fact]
		[Trait("Patch", "Bad Result")]
		public async Task PatchOrder_WhenModelStateInvalid_ReturnsBadResult()
		{
			var updatedObject = new OrderDto(testOrders[0].Id, new DateTime(2025, 01, 27), 100710, 1004, 10, 9.89M, 39.56M, "Brazil", "Salvador");
			mockRepo.Setup(repo => repo.GetAsync(testOrders[0].Id).Result)
				   .Returns(testOrders[0]);
			controller.ModelState.AddModelError("", ""); // Force ModelState to be invalid

			var response = await controller.PatchOrder(testOrders[0].Id, new JsonPatchDocument<OrderDto> { });

			Assert.NotNull(response);
			Assert.IsType<BadRequestResult>(response);
		}

		[Fact]
		[Trait("Post", "Valid result")]
		public async Task PostOrder_GivenObject_ReturnNoContent()
		{
			var newObject = new OrderDto(new Guid("b45c9983-d97a-403e-87ae-742c11493a40"), new DateTime(2024, 11, 02), 117917, 1005, 2, 41.6M, 83.2M, "Brazil", "Sao Paulo");
			mockRepo.Setup(repo => repo.CreateAsync(newObject).Result)
					.Returns(newObject);

			var response = await controller.PostOrder(newObject);

			Assert.NotNull(response);
			Assert.IsType<ActionResult<OrderDto>>(response);
			Assert.IsType<CreatedAtActionResult>(response.Result);
		}

		[Fact]
		[Trait("Delete", "Valid result")]
		public async Task DeleteOrder_GivenId_ReturnsNoContent()
		{
			var obj = testOrders[0];
			mockRepo.Setup(repo => repo.GetAsync(obj.Id).Result).Returns(obj);
			mockRepo.Setup(repo => repo.RemoveAsync(obj)).Verifiable();

			var response = await controller.DeleteOrder(obj.Id);

			Assert.NotNull(response);
			Assert.IsType<NoContentResult>(response);
		}

		[Fact]
		[Trait("Delete", "NotFound")]
		public async Task DeleteOrder_GivenNonExistantId_ReturnsNoContent()
		{
			mockRepo.Setup(repo => repo.GetAsync(nonexistantGuid).Result).Returns((OrderDto)null);

			var response = await controller.DeleteOrder(nonexistantGuid);

			Assert.NotNull(response);
			Assert.IsType<NotFoundResult>(response);
		}
	}
}