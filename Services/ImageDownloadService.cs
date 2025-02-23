 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdonisAPI.Services
{
    public class ImageDownloadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseImageFolder = "StoredImages"; // Base folder for all images

        public ImageDownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Downloads an image from a URL and saves it under a user-specific folder.
        /// Returns a relative path like "userId/imageId.gif".
        /// </summary>
        public async Task<string> DownloadAndSaveGifAsync(string imageUrl, string userId, string imageId)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL must be provided.", nameof(imageUrl));

            try
            {
                // Build the base folder path based on the current directory.
                string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), _baseImageFolder);
                if (!Directory.Exists(baseFolder))
                    Directory.CreateDirectory(baseFolder);

                // Create a user-specific folder inside the base folder.
                string userFolder = Path.Combine(baseFolder, userId);
                if (!Directory.Exists(userFolder))
                    Directory.CreateDirectory(userFolder);

                // Create the full file path (with .gif extension).
                string fileName = $"{imageId}.gif";
                string fullPath = Path.Combine(userFolder, fileName);

                // If the file already exists, return the relative path.
                if (File.Exists(fullPath))
                    return $"{userId}/{fileName}";

                // Download image bytes.
                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                // Return the relative path.
                return $"{userId}/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the relative path of an image for a given user and image ID.
        /// </summary>
        public string GetLocalImagePath(string userId, string imageId)
        {
            string fileName = $"{imageId}.gif";
            string relativePath = $"{userId}/{fileName}";
            string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), _baseImageFolder);
            string fullPath = Path.Combine(baseFolder, userId, fileName);
            return File.Exists(fullPath) ? relativePath : null;
        }

        /// <summary>
        /// Lists all users who have stored images.
        /// </summary>
        public List<string> GetAllUsersWithStoredImages()
        {
            string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), _baseImageFolder);
            if (!Directory.Exists(baseFolder))
                return new List<string>();

            return Directory.GetDirectories(baseFolder)
                            .Select(Path.GetFileName) // Extracts user ID from folder name.
                            .ToList();
        }

        /// <summary>
        /// Lists all stored images for a given user as relative paths.
        /// </summary>
        public List<string> GetUserImages(string userId)
        {
            string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), _baseImageFolder);
            string userFolder = Path.Combine(baseFolder, userId);
            if (!Directory.Exists(userFolder))
                return new List<string>();

            return Directory.GetFiles(userFolder, "*.gif")
                            .Select(file => $"{userId}/{Path.GetFileName(file)}")
                            .ToList();
        }
    }
}
