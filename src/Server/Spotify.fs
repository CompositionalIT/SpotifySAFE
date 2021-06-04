module Spotify

open FSharp.Control.Tasks
open SpotifyAPI.Web
open System

let getAlbums searchTerm = task {
    let client = SpotifyClient "" // Put your Spotify Token here!
    let! results = client.Search.Item(SearchRequest(SearchRequest.Types.Album, searchTerm))

    return [
        for album in results.Albums.Items do
            { Name = album.Name
              Artist =
                  album.Artists
                  |> Seq.tryHead
                  |> Option.map (fun a -> a.Name)
              Image =
                  album.Images
                  |> Seq.tryHead
                  |> Option.map
                      (fun a ->
                          { Uri = a.Url
                            Height = a.Height
                            Width = a.Width })
              ReleaseDate =
                  album.ReleaseDate
                  |> DateTime.TryParse
                  |> function
                  | true, v -> Some v
                  | false, _ -> None
              Tracks = album.TotalTracks
              Uri = album.Uri }
    ]
    |> List.distinctBy (fun r -> r.Name)
    |> List.sortByDescending(fun r -> r.ReleaseDate)
}

