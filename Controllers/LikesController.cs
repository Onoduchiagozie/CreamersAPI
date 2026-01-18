using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdonisAPI.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using AdonisAPI.Services;

namespace AdonisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavouritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CreamUser> _userManager;
        private readonly ImageDownloadService _imageDownloadService;

        public FavouritesController(ApplicationDbContext context,
            UserManager<CreamUser> userManager,
            ImageDownloadService imageDownloadService)
        {
            _context = context;
            _userManager = userManager;
            _imageDownloadService = imageDownloadService;
        }

        // POST: api/favourites/add
  
        
        
        // DELETE: api/favourites/delete/{exerciseId}
        [HttpDelete("delete/{exerciseId}")]
        public async Task<IActionResult> DeleteFavourite(int exerciseId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized("User not authenticated.");
            var c = Guid.Parse(exerciseId.ToString());
            var favourite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.CreamUserId == userId && f.ProductId == c.ToString());
            if (favourite == null)
                return NotFound("Favourite not found.");

            _context.Favorites.Remove(favourite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Product removed from favorites." });
        }

        // GET: api/favourites
        [HttpGet]
        public async Task<IActionResult> GetUserFavourites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User not authenticated.");
            var request = HttpContext.Request; // Access via a controller property or injected IHttpContextAccessor
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            var favouritesData = await _context.Favorites
                                           .Where(f => f.CreamUserId == userId)
                                           .Include(f => f.Product)
                                           .Select(f => new
                                           {
                                               Name=f.Product.Name,
                                              Price= f.Product.Cost,
                                               Image = $"{baseUrl}{f.Product.ProductImageBase64}",
                               
                                           })
                                           .ToListAsync();

            
            
            
            var favourites = favouritesData.Select(f => new
            {
                f.Name,
                f.Price,
                f.Image
   
            }).ToList();
            return Ok(favourites);
        }
    }
}

























































/*
using AdonisAPI.Models;
using AdonisAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdonisAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
     
   private readonly ApplicationDbContext _dbContext;
   private readonly ImageDownloadService _imageDownloadService;

        public ExerciseController(ApplicationDbContext dbContext,
            ImageDownloadService imageDownloadService)
        {
            _dbContext = dbContext;
            _imageDownloadService = imageDownloadService;
        }

        
        [HttpGet]
        public async Task<IEnumerable<Exercise>> GetExercises()
        {
            var exercises = await _dbContext.Exercises.ToListAsync();

            
           
     //   public class ImageRequest
     //   {
      //      public string ImageUrl { get; set; }
      //      public string CategoryName { get; set; } // e.g., "barbell_workout"
      //      public string ImageId { get; set; } // e.g., "12345"
      //  }
      //      [FromBody] ImageRequest request
            //var filePath = await _imageDownloadService.DownloadAndSaveGifAsync(request.ImageUrl, request.CategoryName, request.ImageId);

            
            /*foreach (var exercise in exercises)
            {
                if (!string.IsNullOrEmpty(exercise.ImageUrl)) // If image exists
                {
                    string category = exercise.Category ?? "default"; // Category from DB
                    string imageId = exercise.Id.ToString(); // Use exercise ID as image ID
                    await _imageDownloadService.DownloadAndSaveGifAsync(exercise.ImageUrl, category, imageId);
                }
            }#1#
            return exercises;
        }
        
}
*/ 
