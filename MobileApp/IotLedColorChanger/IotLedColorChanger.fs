namespace IotLedColorChanger

open Fabulous.Core
open Fabulous.DynamicViews
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open Xamarin.Forms

module App = 
    type Model = 
        {
            R : byte
            G : byte
            B : byte
            IsBusy : bool
        }

    type Msg = 
        | SetR of byte
        | SetG of byte
        | SetB of byte 
        | Submit
        | SubmitFinished

    let initModel = { R = 0uy; G = 0uy; B = 0uy; IsBusy = false }

    let init () = initModel, Cmd.none

    let updateRgb model =
        async { 
            let body = sprintf """ {"red": %d,"green": %d,"blue": %d} """ model.R model.G model.B
            let! resp = Http.AsyncRequest(ApiKeys.FunctionUri, 
                                          query=[ "code", ApiKeys.FunctionKey ],
                                          headers = [ ContentType HttpContentTypes.Json ],
                                          body = TextRequest body,
                                          httpMethod="POST" )
            return SubmitFinished
        }

    let update msg model =
        match msg with
        | SetR v -> { model with R = v }, Cmd.none
        | SetG v -> { model with G = v }, Cmd.none
        | SetB v -> { model with B = v }, Cmd.none
        | Submit -> { model with IsBusy = true }, (updateRgb model) |> Cmd.ofAsyncMsg
        | SubmitFinished -> { model with IsBusy = false },Cmd.none

    let createSliderLabel row text =
        View.Label(text = text, verticalOptions = LayoutOptions.Center)
            .GridRow(row).GridColumn(0)

    let createSlider row changed =
        View.Slider(
                maximum = 255.,
                minimum = 0., 
                verticalOptions = LayoutOptions.Center, 
                valueChanged = changed)
            .GridRow(row).GridColumn(1)
       
    let view (model: Model) dispatch =
        View.ContentPage(
            View.Grid(
                padding = 20.,
                verticalOptions = LayoutOptions.Center,
                rowdefs=[ "auto"; "auto"; "auto"; "*" ], 
                coldefs=[ "auto"; "*" ],
                children = [
                    createSliderLabel 0 "R"
                    createSliderLabel 1 "G"
                    createSliderLabel 2 "B"
                    createSlider 0 (fun args -> dispatch (SetR(byte args.NewValue)))
                    createSlider 1 (fun args -> dispatch (SetG(byte args.NewValue)))
                    createSlider 2 (fun args -> dispatch (SetB(byte args.NewValue)))
                    View.Button(
                        backgroundColor = Color.FromRgb(int model.R, int model.G, int model.B),
                        command = (fun () -> dispatch Submit),
                        margin = Thickness(0., 50.),
                        fontSize = "Large",
                        text = "Send",
                        textColor = Color.White,
                        isEnabled = not model.IsBusy
                    ).GridRow(3).GridColumnSpan(2)
                ]
            )
        )

    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    