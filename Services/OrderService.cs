 using System.Data;
 using System.Security.Claims;
 using AdonisAPI.Models;
 using AdonisAPI.Models.Order;
using AdonisAPI.Models.Order.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
 
namespace AdonisAPI.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<CreamUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public OrderService(ApplicationDbContext context, 
        UserManager<CreamUser> userManager, 
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Order> PlaceOrderAsync(PlaceOrderRequest request,string userId)
    {
        /////
        // using var tx = await _context.Database
        //     .BeginTransactionAsync(IsolationLevel.Serializable);

        var order = new Order();

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId.ToString());

            if (product == null)
                throw new Exception("Product not found");
            

            var basePrice = product.Cost ?? 0;
            var lineTotal = (basePrice
                            ) * item.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                BasePrice = basePrice,
                Quantity = item.Quantity,
                LineTotal = lineTotal,
           
            };

            order.Items.Add(orderItem);
        }
 
        order.Subtotal = order.Items.Sum(i => i.LineTotal);
        order.Tax = (int)(order.Subtotal * 0.1);
        order.Total = order.Subtotal + order.Tax;
        order.UserId = userId;
        
        
        
        //get user object in service class 
       // var userPrincipal = _httpContextAccessor.HttpContext?.User;
      //  var currentUser = _userManager.GetUserAsync(userPrincipal);
        var currentUser = await _userManager.FindByIdAsync(userId);
        
        
        
       await _context.Orders.AddRangeAsync(order);
        try
        {
            var result =await _context.SaveChangesAsync();
            
            EmailSender.SendOrderConfirmation(
                order,
                currentUser.UserName,
                currentUser.Email
                );
            
            if (result > 0)
                Console.WriteLine("Order Placed successfully",result);
            return order;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        // await tx.CommitAsync();

    }
}
