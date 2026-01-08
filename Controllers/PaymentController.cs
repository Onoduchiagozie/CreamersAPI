using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdonisAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaystackController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _paystackSecretKey;

        public PaystackController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _paystackSecretKey = _configuration["Paystack:SecretKey"] 
                ?? throw new InvalidOperationException("Paystack secret key not configured");
        }

        [HttpPost("Initialize")]
        public async Task<IActionResult> Initialize([FromBody] InitializeRequest request)
        {
            if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Invalid amount or email" });
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_paystackSecretKey}");

            // Generate unique reference
            var reference = $"txn_{Guid.NewGuid():N}";

            var payload = new
            {
                email = request.Email,
                amount = Math.Round((int)request.Amount * 0.0001),  
                reference = reference,
                 callback_url = "myapp://payment/callback" ,
                channels = new[] { "card", "ussd", "qr" }

            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );
            //paystack 

            try
            {
                var response = await client.PostAsync("https://api.paystack.co/transaction/initialize", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Paystack initialization failed", details = responseBody });
                }
//transaction is successfull save it to Db Here
                var paystackResponse = JsonSerializer.Deserialize<PaystackInitializeResponse>(responseBody);

                if (paystackResponse?.Status != true || paystackResponse.Data == null)
                {
                    return BadRequest(new { message = "Invalid response from Paystack" });
                }

                return Ok(new
                {
                    authorization_url = paystackResponse.Data.AuthorizationUrl,
                    reference = paystackResponse.Data.Reference,
                    access_code = paystackResponse.Data.AccessCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error initializing payment", error = ex.Message });
            }
        }

        [HttpGet("verify/{reference}")]
        public async Task<IActionResult> VerifyTransaction(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest(new { message = "Reference is required" });
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_paystackSecretKey}");

            try
            {
                var response = await client.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Verification failed", details = responseBody });
                }

                var verifyResponse = JsonSerializer.Deserialize<PaystackVerifyResponse>(responseBody);

                if (verifyResponse?.Status != true || verifyResponse.Data == null)
                {
                    return BadRequest(new { message = "Invalid verification response" });
                }

                var isSuccessful = verifyResponse.Data.Status == "success";
                var amount = verifyResponse.Data.Amount / 100m; // Convert from kobo back to naira

                return Ok(new
                {
                    success = isSuccessful,
                    reference = verifyResponse.Data.Reference,
                    amount = amount,
                    status = verifyResponse.Data.Status,
                    paid_at = verifyResponse.Data.PaidAt,
                    message = isSuccessful ? "Payment successful" : "Payment failed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error verifying payment", error = ex.Message });
            }
        }
    }

    // Request/Response Models
    public class InitializeRequest
    {
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PaystackInitializeResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public PaystackInitializeData? Data { get; set; }
    }

    public class PaystackInitializeData
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; } = string.Empty;
        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; } = string.Empty;
        [JsonPropertyName("reference")]

        public string Reference { get; set; } = string.Empty;
    }

    public class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackVerifyData? Data { get; set; }
    }

    public class PaystackVerifyData
    {
        public string Status { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string? PaidAt { get; set; }
    }
}