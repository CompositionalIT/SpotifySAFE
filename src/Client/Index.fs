module Index

open Elmish
open Elmish.SweetAlert
open Fable.FontAwesome
open Fable.Remoting.Client
open Fable.SimpleJson
open Feliz
open Feliz.Bulma
open System

type Model =
    { Albums: Album list
      IsLoading : bool
      Input: string }
    member this.CanSearch =
        String.IsNullOrWhiteSpace this.Input |> not

type Msg =
    | Searching of Album list AsyncOp
    | SearchUpdated of string
    | OnError of exn

let searchApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withBinarySerialization
    |> Remoting.buildProxy<ISearchApi>

let init () =
    { Albums = []
      Input = ""
      IsLoading = false },
    Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | SearchUpdated text ->
        { model with Input = text }, Cmd.none
    | Searching Started ->
        let cmd = Cmd.OfAsync.either searchApi.findAlbums model.Input (Completed >> Searching) OnError
        { model with IsLoading = true }, cmd
    | Searching (Completed albums) ->
        { model with IsLoading = false; Albums = albums }, Cmd.none
    | OnError ex ->
        let message =
            match ex with
            | :? ProxyRequestException as ex ->
                let x = Json.parseAs<{| error : {| Description : string |} |}> ex.ResponseText
                x.error.Description
            | ex ->
                ex.Message

        let cmd = SimpleAlert(message).Title("Oh, no!").Type(AlertType.Error)
        { model with IsLoading = false }, SweetAlert.Run cmd

let searchForm dispatch model =
    Bulma.level [
        Bulma.levelLeft [
            Bulma.levelItem [
                Bulma.field.div [
                    field.hasAddons
                    prop.children [
                        Bulma.control.p [
                            control.hasIconsLeft
                            prop.children [
                                Html.div [
                                    Bulma.input.search [
                                        prop.size 50
                                        prop.value model.Input
                                        prop.placeholder "Enter a search term here..."
                                        prop.onChange (fun t -> SearchUpdated t |> dispatch)
                                    ]
                                    Bulma.icon [
                                        icon.isLeft
                                        icon.isSmall
                                        prop.children [
                                            Html.i [ prop.className Free.Fa.Solid.Search.Value ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Bulma.control.p [
                            Bulma.button.a [
                                color.isPrimary
                                prop.disabled (not model.CanSearch)
                                if model.IsLoading then button.isLoading
                                prop.onClick (fun _ -> dispatch (Searching Started))
                                prop.text "Search"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.section [
        Bulma.container [
            Bulma.title [ prop.text "Spotif#y" ]
            searchForm dispatch model
            match model.Albums with
            | [] -> ()
            | albums -> Stonecutter.imageGrid albums
        ]
    ]
