using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;

class SpotifyBot{

    SpotifyClientConfig spotifyClientConfig;

    public SpotifyClient spotifyClient;

    public SpotifyBot(string clientId, string clientSecret){
        spotifyClientConfig = new SpotifyClientConfig

    }


    public async void GetTrackByID(string id){
        var track = await spotifyClient.Tracks.Get(id);
        await Task.CompletedTask;
        Console.WriteLine(track.Name);
    }
}