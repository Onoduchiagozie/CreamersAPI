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
        private readonly UserManager<GymBro> _userManager;
        private readonly ImageDownloadService _imageDownloadService;

        public FavouritesController(ApplicationDbContext context, UserManager<GymBro> userManager, ImageDownloadService imageDownloadService)
        {
            _context = context;
            _userManager = userManager;
            _imageDownloadService = imageDownloadService;
        }

        // POST: api/favourites/add
        [HttpPost("AddFavourite")]
        public async Task<IActionResult> AddFavourite([FromBody] Exercise newExercise)
        {
            var userId = _userManager.GetUserId(User);
  
            if (userId == null) return Unauthorized("User not authenticated.");

            var user = await _userManager.Users.Include(u => u.Favorites)
                                               .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("User not found.");

            // Check if the exercise already exists in the DB
            var exercise = await _context.Exercises
                                         .FirstOrDefaultAsync(e => e.Name == newExercise.Name && e.Target == newExercise.Target);

            if (exercise == null)
            {
                // If exercise doesn't exist, save it to the database
                exercise = new Exercise
                {
                    ExerciseId = newExercise.ExerciseId,
                    Name = newExercise.Name,
                    BodyPart = newExercise.BodyPart,
                    Equipment = newExercise.Equipment,
                    Target = newExercise.Target,
                    GifUrl = newExercise.GifUrl,
                    Instructions = newExercise.Instructions,
                    SecondaryMuscles = newExercise.SecondaryMuscles
                };

                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();
            }

            // Check if user already favorited this exercise
            if (user.Favorites.Any(f => f.ExerciseId == exercise.Id))
                return BadRequest("Exercise is already in favorites.");

            // Download and save exercise image locally
            string localImagePath = await _imageDownloadService.DownloadAndSaveGifAsync(
                exercise.GifUrl, userId, exercise.ExerciseId
            );
            Console.Write(localImagePath);

            // Add exercise to user's favorites
            var favourite = new Favourite
            {
                UserId = userId,
                ExerciseId = exercise.Id,
                LocalImagePath = localImagePath
            };

            _context.Favorites.Add(favourite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Exercise added to favorites.", ImagePath = localImagePath });
        }

        
        
        
        // DELETE: api/favourites/delete/{exerciseId}
        [HttpDelete("delete/{exerciseId}")]
        public async Task<IActionResult> DeleteFavourite(int exerciseId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized("User not authenticated.");

            var favourite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ExerciseId == exerciseId);
            if (favourite == null)
                return NotFound("Favourite not found.");

            _context.Favorites.Remove(favourite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Exercise removed from favorites." });
        }

        // GET: api/favourites
        [HttpGet("GetUserFavourites")]
        public async Task<IActionResult> GetUserFavourites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User not authenticated.");

            var favouritesData = await _context.Favorites
                                           .Where(f => f.UserId == userId)
                                           .Include(f => f.Exercise)
                                           .Select(f => new
                                           {
                                           
                                               f.Exercise.Name,
                                               f.Exercise.BodyPart,
                                               f.Exercise.Equipment,
                                               f.Exercise.Target,
                                             f.LocalImagePath,
                                             //  LocalImagePath = _imageDownloadService.GetLocalImagePath(userId, f.Exercise.Id.ToString()),
                                               Instructions = f.Exercise.Instructions,
                                               SecondaryMuscles = f.Exercise.SecondaryMuscles
                                           })
                                           .ToListAsync();

            
            
            
            var favourites = favouritesData.Select(f => new
            {
                f.Name,
                f.BodyPart,
                f.Equipment,
                f.Target,
                LocalImagePath = $"http://192.168.100.67:5151/StoredImages/{f.LocalImagePath}",
                f.Instructions,
                f.SecondaryMuscles
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
