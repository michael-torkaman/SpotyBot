using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace SpotyBot;
class SpotifyService{

    private readonly SpotifyClient _spotifyClient;

    private string _playlistName;

    public SpotifyService(string clientId, string clientSecret){
        _spotifyClient = new SpotifyClient(SpotifyClientConfig.
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
        var track = await _spotifyClient.Tracks.Get(id);
        await Task.CompletedTask;
        return track.Name.ToString();
    }

    public async Task<string> EnsurePlaylistExists(){
        var currentUser = await _spotifyClient.UserProfile.Current();
        var playlists = await _spotifyClient.Playlists.CurrentUsers();
        var playlist = playlists.Items.Find(p => p.Name == _playlistName);
        
        if(playlist != null){
            return playlist.Id;
        }

        var newPlaylist = await _spotifyClient.Playlists.Create(currentUser.Id, new PlaylistCreateRequest(_playlistName){
            Description = "song suggestions from friends",
            Public = false
        });

        return newPlaylist.Id;
    }

    

    public async Task<bool> AddToPlaylist(string trackId)
    {
        var playlistId = await EnsurePlaylistExists();
        var addItemsRequest = new PlaylistAddItemsRequest(new List<string> { $"spotify:track:{trackId}" });
        var response = await _spotifyClient.Playlists.AddItems(playlistId, addItemsRequest);

        return response.SnapshotId != null; 
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