using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace SpotyBot;
public class SpotifyService{

    private SpotifyClient _spotifyClient;

    private const string _playlistName = "Seattle Satellites";

    public SpotifyService(string authCode)
    {
        _spotifyClient = new SpotifyClient(authCode);
    }


    public async Task InitializeClient(string authCode, string clientId, string clientSecret, string redirectUri)
    {
        var config = SpotifyClientConfig.CreateDefault();
        var request = new OAuthClient(config);
        var response = await request.RequestToken(
            new AuthorizationCodeTokenRequest(clientId, clientSecret, authCode, new Uri(redirectUri))
        );

        _spotifyClient = new SpotifyClient(response.AccessToken);
    }

    /// <summary>
    /// takes a spotfiy track id an returns the name of track 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SpotifyAPI.Web.FullTrack> GetTrackByID(string id)
    {
        var track = await _spotifyClient.Tracks.Get(id);
        return track;
    }

    public async Task<bool> UserHasPlaylist(){
        var result = await UserHasPlaylist(_playlistName);
        return result;
    }

    public async Task<bool> UserHasPlaylist(string playlistName)
    {
    try
    {
        var currentUser = await _spotifyClient.UserProfile.Current();

        // Retrieve all playlists with pagination
        var offset = 0;
        const int limit = 50;
        Paging<FullPlaylist> currentPage;
        
        do
        {
            currentPage = await _spotifyClient.Playlists.CurrentUsers(new PlaylistCurrentUsersRequest { Limit = limit, Offset = offset });
            var playlist = currentPage.Items.FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

            if (playlist != null)
            {
                return true;
            }

            offset += limit;
        } while (currentPage.Items.Count == limit);

        return false;
    }
    catch (APIUnauthorizedException)
    {
        // Handle unauthorized error (e.g., refresh token)
        throw new Exception("Unauthorized access. Please check your credentials.");
    }
    catch (APIException apiEx)
    {
        // Handle API-related errors
        throw new Exception($"Spotify API error: {apiEx.Message}");
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }


    // public async Task<bool> AddToPlaylist(string trackId)
    // {
    //     var playlistId = await EnsurePlaylistExists();
    //     var addItemsRequest = new PlaylistAddItemsRequest(new List<string> { $"spotify:track:{trackId}" });
    //     var response = await _spotifyClient.Playlists.AddItems(playlistId, addItemsRequest);

    //     return response.SnapshotId != null; 
    // }

    /// <summary>
    /// takes a link to a spotify song and returns the track id from the url using regex
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public string ExtractIDFromURL(string url)
    {
        var regex = new Regex(@"https:\/\/open\.spotify\.com\/track\/([a-zA-Z0-9]+)");
        var match = regex.Match(url);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public async Task<bool> AddSongToPlaylistByName(string trackId)
    {
        try
        {
            var currentUser = await _spotifyClient.UserProfile.Current();

            // Check if the user has the playlist
            var hasPlaylist = await UserHasPlaylist(_playlistName);

            if (!hasPlaylist)
            {
                throw new Exception($"Playlist '{_playlistName}' not found.");
            }

            // Retrieve all playlists with pagination to find the playlist ID
            var offset = 0;
            const int limit = 50;
            Paging<FullPlaylist> currentPage;
            FullPlaylist targetPlaylist = null;

            do
            {
                currentPage = await _spotifyClient.Playlists.CurrentUsers(new PlaylistCurrentUsersRequest { Limit = limit, Offset = offset });
                targetPlaylist = currentPage.Items.FirstOrDefault(p => p.Name.Equals(_playlistName, StringComparison.OrdinalIgnoreCase));

                if (targetPlaylist != null)
                {
                    break;
                }

                offset += limit;
            } while (currentPage.Items.Count == limit);

            if (targetPlaylist == null)
            {
                throw new Exception($"Playlist '{_playlistName}' not found.");
            }

            // Add the track to the playlist
            var addItemsRequest = new PlaylistAddItemsRequest(new List<string> { $"spotify:track:{trackId}" });
            var response = await _spotifyClient.Playlists.AddItems(targetPlaylist.Id, addItemsRequest);

            return response.SnapshotId != null;
        }
        catch (APIUnauthorizedException)
        {
            // Handle unauthorized error (e.g., refresh token)
            throw new Exception("Unauthorized access. Please check your credentials.");
        }
        catch (APIException apiEx)
        {
            // Handle API-related errors
            throw new Exception($"Spotify API error: {apiEx.Message}");
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }


    public async Task<string> CreateNewPublicPlaylist()
    {
        var description = "your songs from fellow freinds";
        try
        {
            var currentUser = await _spotifyClient.UserProfile.Current();

            var newPlaylist = await _spotifyClient.Playlists.Create(currentUser.Id, new PlaylistCreateRequest(_playlistName)
            {
                Description = description,
                Public = true
            });

            return newPlaylist.Id;
        }
        catch (APIUnauthorizedException)
        {
            // Handle unauthorized error (e.g., refresh token)
            throw new Exception("Unauthorized access. Please check your credentials.");
        }
        catch (APIException apiEx)
        {
            // Handle API-related errors
            throw new Exception($"Spotify API error: {apiEx.Message}");
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }


}