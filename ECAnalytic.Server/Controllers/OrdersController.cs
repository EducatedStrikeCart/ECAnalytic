using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECAnalytic.Server.Data;
using ECAnalytic.Server.Infrastructure.Entities;
using ECAnalytic.Server.Data.Repositories;

namespace ECAnalytic.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrdersController(OrderRepository repository) : ControllerBase
	{

		// GET: api/Orders
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int page, int countPerPage)
		{
			var result = await repository.GetAllAsync(page, countPerPage);

			if (result == null) return NotFound();

			return Ok(result);
		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
		{
			var order = await repository.GetAsync(id);

			if (order == null) return NotFound();

			return Ok(order);
		}

		// PUT: api/Orders/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> PutOrder(Guid id, OrderDto order)
		{
			var result = await repository.UpdateAsync(id, order);

			if (result == null) return NotFound();

			return NoContent();
		}

		// POST: api/Orders
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<OrderDto>> PostOrder(OrderDto order)
		{
			await repository.CreateAsync(order);
			await repository.SaveChangesAsync();
			return CreatedAtAction("GetOrder", new { id = order.Id }, order);
		}

		// DELETE: api/Orders/5
		[HttpDelete("{id}")]
				[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteOrder(Guid id)
		{
			var order = await repository.GetAsync(id);

			if (order == null) return NotFound();

			await repository.RemoveAsync(order);
			await repository.SaveChangesAsync();
			return NoContent();
		}
	}
}
