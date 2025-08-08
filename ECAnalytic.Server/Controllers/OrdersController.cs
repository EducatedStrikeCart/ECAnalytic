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
using ECAnalytic.Server.Infrastructure.Contracts;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace ECAnalytic.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController(IBaseEntityFrameworkRepository<Order, OrderDto, Guid> repository) : ControllerBase
	{

		// GET: api/Orders
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int pageNumber, int countPerPage)
		{
			var result = await repository.GetMultipleAsync(pageNumber - 1, countPerPage);

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

		[HttpPatch("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> PatchOrder(Guid id, JsonPatchDocument<OrderDto> updatePatch)
		{
			// Prevent [DoS via memory amplification](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-9.0)
			if (updatePatch.Operations.Where(op => op.OperationType == OperationType.Copy).Count()> 5) return BadRequest();

			var order = await repository.GetAsync(id);
			if (order == null) return NotFound();

			updatePatch.ApplyTo(order, ModelState);
			if (!ModelState.IsValid) return BadRequest();
			if (!TryValidateModel(order)) return BadRequest();

			await repository.UpdateAsync(id, order);

			return NoContent();
		}

		// POST: api/Orders
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<OrderDto>> PostOrder(OrderDto order)
		{
			await repository.CreateAsync(order);
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
