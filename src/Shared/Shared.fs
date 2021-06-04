namespace global

open System

type Image =
    { Uri: string
      Height: int
      Width: int }
    member this.Ratio = float this.Height / float this.Width
    member this.CalculateHeight width = this.Ratio * float width |> int

type Album =
    { Artist: string option
      Name: string
      Image: Image option
      ReleaseDate: DateTime option
      Tracks: int
      Uri: string }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ISearchApi =
    { findAlbums: string -> Album list Async }
