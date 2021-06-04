#r "nuget:SpotifyAPI-NET"
#r "nuget:SpotifyAPI.Web.Auth"

open SpotifyAPI.Web

let client = SpotifyClient "" // Put your Spotify token here...
let results = client.Search.Item(SearchRequest(SearchRequest.Types.Album, "Clapton")).Result
let first = results.Albums

first.Items.[0].Images