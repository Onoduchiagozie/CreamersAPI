using System.Security.Claims;
using AdonisAPI.Models;
using AdonisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AdonisAPI.Controllers;


[Route("api/[controller]")]
[ApiController]

public class ProductController:ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<CreamUser> _userManager;
    public ProductController(ApplicationDbContext context, UserManager<CreamUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    
    
    
    [HttpPost("Like")]
    [Authorize]
    public async Task<IActionResult> Like([FromBody] string productId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized("User not authenticated.");

        // Check if it already exists
        var existingFav = await _context.Favorites
            .FirstOrDefaultAsync(f => f.CreamUserId == userId && f.ProductId == productId);

        // --- UNLIKE ---
        if (existingFav != null)
        {
            _context.Favorites.Remove(existingFav);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                liked = false,
                message = "Product removed from favourites."
            });
        }

        // --- LIKE ---
        var newFav = new Favourite
        {
            Id = Guid.NewGuid().ToString(),
            CreamUserId = userId,
            ProductId = productId
        };

        _context.Favorites.Add(newFav);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            liked = true,
            message = "Product added to favourites."
        });
    }


    // [HttpGet("GetAllProductswithoption")]
    // public async Task<IActionResult> GetAllProductsWithOptions(string id){
    //     var product = await _context.Products
    //         .Include(p => p.CustomizationGroups)
    //         .ThenInclude(g => g.Options)
    //         .FirstOrDefaultAsync(p => p.Id == id);
    //     return Ok(product);
    //
    // }

    [Authorize]  
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(new { message = "Product ID is required" });

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    
        if (product == null)
            return NotFound(new { message = "Product not found" });
    
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    
        return Ok(new { message = "Product deleted successfully" });
    }


    [HttpGet("GetAllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
        var request = HttpContext.Request; // Access via a controller property or injected IHttpContextAccessor
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        var products = await _context
            .Products
            .Select(p => new
            {
                p.Id,
                 p.Name,
                p.Cost,
                rating=23,
                p.Category,
                description=p.Description,
                 ImageUrl = $"{baseUrl}{p.ProductImageBase64}", // contains "/uploads/xxx.jpg"
                 Customizations = p.CustomizationGroups
                 .OrderBy(g => g.SortOrder)
                 .Select(g => new
                 {
                 g.Id,
                 g.Name,               // Sweetness, Flavour, Toppings
                 g.IsRequired,
                 g.MaxSelections,
                 Options = g.Options.Select(o => new
                 {
                     o.Id,
                     o.Name,
                     o.PriceIncrement
                 })
            })
            })
            .ToListAsync();

        return Ok(products);
    }
    

    [Authorize] 
    [HttpGet("GetAllSellerProducts")]// Add this attribute to require authentication
    public async Task<IActionResult> GetAllSellerProducts()
    {
        var request = HttpContext.Request; // Access via a controller property or injected IHttpContextAccessor
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        // Get user ID from token claims
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //
        // // Or get username
        // var username = User.FindFirst(ClaimTypes.Name)?.Value;
    
         var email = User.FindFirst(ClaimTypes.Email)?.Value;
    
        // Get custom claims (if you added any during token generation)
        // var customClaim = User.FindFirst("YourCustomClaimType")?.Value;
    
        // var request = HttpContext.Request;
        // var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
    
        // Use the userId/username to filter products
        var products = await 
            _context
                .Products
                 .Where(p 
                    =>
                    p.Seller.Email==email).Select(n=>new
                 {n.Id,
                     n.Name,
                     n.Cost,
                     n.Description,
                     ImageUrl = $"{baseUrl}{n.ProductImageBase64}", // contains "/uploads/xxx.jpg"

                      n.FavouritedBy
                 }) 
            .ToListAsync();
    
        return Ok(products);
    }
    
    [HttpPost("AddProduct")]
    [Authorize]
    public async Task<IActionResult> AddProduct([FromBody] ProductDTO newProduct)
    {
         var user = await _userManager.GetUserAsync(User);
        Product intake = new Product();

        if (newProduct == null)
            return BadRequest(new { message = "Invalid product data" });

        // Handle Base64 image
        if (!string.IsNullOrEmpty(newProduct.ProductImageBase64))
        {
            try
            {
                // Convert Base64 to bytes
                var bytes = Convert.FromBase64String(newProduct.ProductImageBase64);

                // Create folder if missing
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "StoredImages");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Generate file name
                string fileName = $"{Guid.NewGuid()}.jpg";
                string filePath = Path.Combine(folderPath, fileName);

                // Save the image to disk
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                // Replace Base64 with relative path
                newProduct.ProductImageBase64 = $"/StoredImages/{fileName}";

                intake.Id = Guid.NewGuid().ToString();
                intake.Name = newProduct.Name;
                intake.Cost = newProduct.Cost;
                intake.Location = newProduct.Location;
                intake.Description = newProduct.Description;
                intake.ProductImageBase64 = newProduct.ProductImageBase64;
                intake.SellerId = user.Id;
                intake.CustomizationGroups = new List<TreatCustomizationGroup>();
                
                
                
                if (newProduct.Customizations != null)
                {
                    foreach (var group in newProduct.Customizations)
                    {
                        var cg = new TreatCustomizationGroup
                        {
                            Name = group.Name,
                            IsRequired = group.IsRequired,
                            MaxSelections = group.MaxSelections,
                            Options = group.Options.Select(o => 
                                new TreatCustomizationOptions
                            {
                                Name = o.Name,
                                PriceIncrement = o.PriceIncrement
                            }).ToList()
                        };
                        intake.CustomizationGroups.Add(cg);
                    }
                }

            }
            catch(Exception e)
            {
                
                return BadRequest(new { message = $" error in adding product {e.Message}" });
            }
        }

         // Save product to DB
        _context.Products.Add(intake);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Product created successfully",
            product = newProduct
        });
    }


}