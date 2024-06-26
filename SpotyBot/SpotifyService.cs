using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace SpotyBot;
/// <summary>
/// Serves as a service giving the ability to create and manipulate data on a
/// spotify account. requires Oauth to work with private data.
/// does not implement Oauth
/// </summary>
public class SpotifyService{
    
    //spotify user session
    private SpotifyClient _spotifyClient;

    private const string _playlistName = "Seattle Satellites";

    public SpotifyService(string authCode)
    {
        _spotifyClient = new SpotifyClient(authCode);
    }

    /// <summary>
    /// async task that Initializes the SpotifyClient object 
    /// </summary>
    /// <param name="authCode"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="redirectUri"></param>
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

    /// <summary>
    /// No arg overloaded method that check if the default playlist name
    /// Seattle Sattelites exist in user's spotify profile
    /// </summary>
    /// <returns></returns>
    public async Task<bool> UserHasPlaylist(){
        var result = await UserHasPlaylist(_playlistName);
        return result;
    }

    /// <summary>
    /// Checks if a User has a playlist by the name of param playlistName 
    /// </summary>
    /// <param name="playlistName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
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


    /// <summary>
    /// add song to default playlist by track id 
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AddSongToPlaylistByName(string trackId)
    {
        try
        {
            //get user profile info 
            var currentUser = await _spotifyClient.UserProfile.Current();

            // Check if the user has the playlist
            if (await UserHasPlaylist(_playlistName) == false)
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


    /// <summary>
    /// Creates public playlist by the name of field _playlistName
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> CreateNewPublicPlaylist()
    {
        //description field of the palylist
        var description = "your songs from fellow friends";
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
            throw new Exception("Unauthorized access. Please check your credentials.");
        }
        catch (APIException apiEx)
        {
            throw new Exception($"Spotify API error: {apiEx.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }


}