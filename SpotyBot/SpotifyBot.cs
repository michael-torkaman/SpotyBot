using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;

class SpotifyBot{

    SpotifyClientConfig spotifyClientConfig;

    public SpotifyClient spotifyClient;

    public SpotifyBot(string clientId, string clientSecret){
        spotifyClient = new SpotifyClient(SpotifyClientConfig.
            CreateDefault().
            WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret))
            );
    }

    /// <summary>
    /// takes a spotfiy track id an returns the name of track 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<string> GetTrackByID(string id)
    {
        var track = await spotifyClient.Tracks.Get(id);
        await Task.CompletedTask;
        return track.Name.ToString();
    }

    /// <summary>
    /// takes a link to a spotify song and returns the track id from the url using regex
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string ExtractIDFromURL(string url)
    {
        var regex = new Regex(@"https:\/\/open\.spotify\.com\/track\/([a-zA-Z0-9]+)");
        var match = regex.Match(url);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}