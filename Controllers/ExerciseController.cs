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
        [HttpPost("AddFavourite")]
        public async Task<IActionResult> AddFavourite([FromBody] Product newExercise)
        {
            var userId = _userManager.GetUserId(User);
  
            if (userId == null) return Unauthorized("User not authenticated.");

           var user = await _userManager.Users.Include(u => u.Favorites)
                                               .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null) return NotFound("User not found.");

            // Check if the exercise already exists in the DB
            var product = await _context.Products
                                         .FirstOrDefaultAsync(e => e.Name == newExercise.Name
                                                                   // && e.Target == newExercise.Target
                                                                   )
                ;

            if (product == null)
            {
                // If exercise doesn't exist, save it to the database
                product = new Product
                {
                    Id = newExercise.Id,
                    Name = newExercise.Name,
                    Cost = newExercise.Cost,
                    // Equipment = newExercise.Equipment,
                    // Target = newExercise.Target,
             
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            // Check if user already favorited this exercise
            if (user.Favorites.Any(f => f.ProductId.ToString() == product.Id))
                return BadRequest("Product is already in favorites.");

            // Download and save exercise image locally
            string localImagePath = await _imageDownloadService.DownloadAndSaveGifAsync(
                product.ProductImageBase64, userId, product.Id
            );
            Console.Write(localImagePath);

            // Add exercise to user's favorites
            var favourite = new Favourite
            {
                CreamUserId = userId,
                ProductId = product.Id,
                ImagePath = localImagePath
            };

            _context.Favorites.Add(favourite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Product  added to favorites.", ImagePath = localImagePath });
        }

        
        
        
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
        [HttpGet("GetUserFavourites")]
        public async Task<IActionResult> GetUserFavourites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User not authenticated.");

            var favouritesData = await _context.Favorites
                                           .Where(f => f.CreamUserId == userId)
                                           .Include(f => f.Product)
                                           .Select(f => new
                                           {
                                           
                                               f.Product.Name,
                                               f.Product.Cost,
                                               f.Product.ProductImageBase64,
                              
                                             //  LocalImagePath = _imageDownloadService.GetLocalImagePath(userId, f.Exercise.Id.ToString()),
                                               // Instructions = f.Exercise.Instructions,
                                               // SecondaryMuscles = f.Exercise.SecondaryMuscles
                                           })
                                           .ToListAsync();

            
            
            
            var favourites = favouritesData.Select(f => new
            {
                f.Name,
                f.Cost
                // f.BodyPart,
                // f.Equipment,
                // f.Target,
                // LocalImagePath = $"http://192.168.100.67:5151/StoredImages/{f.LocalImagePath}",
                // f.Instructions,
                // f.SecondaryMuscles
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
