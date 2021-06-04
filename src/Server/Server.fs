module Server

open Fable.Remoting.Giraffe
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Saturn

let searchApi =
    { findAlbums = Spotify.getAlbums >> Async.AwaitTask }

let errorHandler (ex:exn) (routeInfo:RouteInfo<HttpContext>) =
    let logger = routeInfo.httpContext.GetService<ILogger<ISearchApi>>()
    logger.LogError (string ex)
    match ex.InnerException with
    | :? SpotifyAPI.Web.APIException as ex -> Propagate {| Description = ex.Message |}
    | _ -> Propagate {| Description = ex.Message |}

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withErrorHandler errorHandler
    |> Remoting.withBinarySerialization
    |> Remoting.fromValue searchApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        use_static "public"
    }

run app
