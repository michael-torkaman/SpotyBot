using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;

class SpotifyBot{

    SpotifyClientConfig spotifyClientConfig;

    public SpotifyClient spotifyClient;

    public SpotifyBot(string clientId, string clientSecret){
        spotifyClient = new SpotifyClient(SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret)));
    }

    public async Task<string> GetTrackByID(string id){
        var track = await spotifyClient.Tracks.Get(id);
        await Task.CompletedTask;
        return track.Name.ToString();
    }
}