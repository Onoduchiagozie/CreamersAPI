using System.Security.Claims;
using AdonisAPI.Migrations;
using AdonisAPI.Models.Order;
using AdonisAPI.Models.Order.DTOs;
using AdonisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdonisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ApplicationDbContext _context;
    public OrderController(OrderService orderService, ApplicationDbContext context)
    {
        _orderService = orderService;
        _context = context;
    }
    // GET
    [HttpGet("GetOrders")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Select(o => new OrderSummaryDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                Total = o.Total,
                Status = o.Status,
                TotalItems = o.Items.Sum(i => i.Quantity)
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders);
    }

    
    
    [HttpPost]
    public async Task<IActionResult> Order([FromBody]PlaceOrderRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (request == null)
            return BadRequest("Request is null");
        var order = await _orderService.PlaceOrderAsync(request, userId);
        return Ok(new
        {
            order.Id,
            order.Total,
            order.Status
        });
    }
}