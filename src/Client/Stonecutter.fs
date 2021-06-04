/// See https://github.com/dantrain/react-stonecutter
module Stonecutter

open Fable.Core.JsInterop
open System
open Feliz.Bulma
open Feliz

let private springGrid : obj = import "SpringGrid" "react-stonecutter"
let private cssGrid : obj = import "CSSGrid" "react-stonecutter"
let private layout : obj = import "layout" "react-stonecutter"
let private easings : obj = import "easings" "react-stonecutter"
let private enterExitStyle : obj = import "enterExitStyle" "react-stonecutter"

let private r = Random()
let private pickOne items = items |> List.item (r.Next(0, List.length items))

let private transition () = pickOne [
    enterExitStyle?foldUp; enterExitStyle?fromBottom; enterExitStyle?fromCenter
    enterExitStyle?fromLeftToRight; enterExitStyle?fromTop; enterExitStyle?newspaper
    enterExitStyle?simple; enterExitStyle?skew
]

/// Creates a pinterest-style grid of albums.
let imageGrid albums =
    Feliz.Interop.reactApi.createElement(
        cssGrid,
        createObj [
            let t = transition()
            "columns" ==> 4
            "columnWidth" ==> 250
            "gutterWidth" ==> 10
            "gutterHeight" ==> 10
            "layout" ==> layout?pinterest
            "duration" ==> 800
            "enter" ==> t?enter
            "entered" ==> t?entered
            "exit" ==> t?exit
            "easing" ==> pickOne [ easings?quadIn; easings?quadOut; easings?quartIn; easings?quartInOut; easings?quartOut ]
            "children" ==> [|
                for album in albums do
                    match album.Image with
                    | Some image ->
                        Bulma.card [
                            let lines = (float album.Name.Length / 25.) |> Math.Ceiling |> int
                            prop.custom("itemHeight", image.CalculateHeight 250 + 50 + (lines * 20))
                            prop.key album.Name
                            prop.style [
                                style.maxWidth (length.px 250)
                            ]
                            prop.children [
                                Bulma.cardImage [
                                    Bulma.image [
                                        prop.children [
                                            Html.img [
                                                prop.src image.Uri
                                            ]
                                        ]
                                    ]
                                ]
                                Bulma.cardContent [
                                    prop.style [ style.backgroundColor "silver" ]
                                    prop.children [
                                        Bulma.media [
                                            Bulma.mediaContent [
                                                Bulma.title.p [
                                                    title.is6
                                                    prop.text album.Name
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    | None ->
                        ()

            |]
        ]
    )

