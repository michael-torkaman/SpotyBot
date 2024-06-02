using Microsoft.AspNetCore.Mvc;
using SpotyBot;
using System.Threading.Tasks;

namespace SpotifyAuthServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpotifyAuthController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;

        public SpotifyAuthController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            var clientId = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFYBOT_CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("SPOTIFYSERVICE_URI"); 

            await _spotifyService.InitializeClient(code, clientId, clientSecret, redirectUri);

            return Ok("Spotify authorization successful! You can close this window.");
        }
    }
}
