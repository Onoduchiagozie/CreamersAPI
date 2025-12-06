using AdonisAPI.Models;
using AdonisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AdonisAPI.Controllers;


[Route("api/[controller]")]
[ApiController]

public class ProductController:ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    [HttpGet("GetAllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
        var request = HttpContext.Request; // Access via a controller property or injected IHttpContextAccessor
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        var products = await _context.Products
            .Select(p => new
            {
                 p.Name,
                p.Cost,
                rating=23,
                description=p.Description,
                 ImageUrl = $"{baseUrl}{p.ProductImageBase64}" // contains "/uploads/xxx.jpg"
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost("AddProduct")]
    public async Task<IActionResult> AddProduct([FromBody] Product newProduct)
    {
        // In a Controller action, Razor Page, or service with injected IHttpContextAccessor
 
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
            }
            catch
            {
                return BadRequest(new { message = "Error processing image" });
            }
        }

        newProduct.Id = Guid.NewGuid().ToString();
        // Save product to DB
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Product created successfully",
            product = newProduct
        });
    }


}