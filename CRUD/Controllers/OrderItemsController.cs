using CRUD.DTO;
using CRUD.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [ApiController]
    [Route("api/order-items")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll()
        {
            return Ok(await _orderItemService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderItemDto>> GetById(Guid id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Позиция заказа с ID {id} не найдена.");
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto dto)
        {
            try
            {
                var item = await _orderItemService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderItemDto dto)
        {
            try
            {
                var updated = await _orderItemService.UpdateAsync(id, dto);
                if (!updated)
                {
                    return NotFound($"Позиция заказа с ID {id} не найдена.");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _orderItemService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Позиция заказа с ID {id} не найдена.");
            }

            return NoContent();
        }
    }
}
