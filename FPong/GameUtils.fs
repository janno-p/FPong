module FPong.GameUtils

open SFML.Graphics
open SFML.Window
open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection

let (@@) path1 path2 = Path.Combine(path1, path2)

let ContentDir =
    let assembly = Assembly.GetExecutingAssembly()
    Path.GetDirectoryName(assembly.Location) @@ "Content"

let FontPath name = ContentDir @@ "Fonts" @@ name

type InputHandler<'Command>(window : RenderWindow, subscriber : (List<'Command> -> IDisposable list)) =
    let state = List<'Command>()
    let unsubscriber = subscriber state
    member this.GetInputState() =
        state.Clear()
        window.DispatchEvents()
        state |> Seq.toList
    interface IDisposable with
        member this.Dispose() =
            unsubscriber |> List.iter (fun x -> x.Dispose())

type GameState<'ScreenState> = {
    IsDone : bool
    TimeSinceLastUpdate : float32
    State : 'ScreenState
    Window : RenderWindow
}

type GameLoopArgs<'State, 'Command> = {
    ProcessCommands : (GameState<'State> -> 'Command list -> GameState<'State>)
    Update : (float32 -> GameState<'State> -> GameState<'State>)
    Render : (GameState<'State> -> GameState<'State>)
    SubscribeToEvents : (List<'Command> -> IDisposable list)
    State : 'State
    Window : RenderWindow
}

let GameLoop (args : GameLoopArgs<'State, 'Command>) =
    let timePerFrame = 1.0f / 60.0f

    use inputHandler = new InputHandler<'Command>(args.Window, args.SubscribeToEvents)

    let rec updateAndRenderFrame state =
        let state = inputHandler.GetInputState() |> args.ProcessCommands state
        match state.TimeSinceLastUpdate with
        | x when x < timePerFrame -> args.Render state
        | _ -> { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate - timePerFrame }
               |> args.Update timePerFrame
               |> updateAndRenderFrame

    let timer = Stopwatch()
    timer.Start()

    let rec gameLoop (state : GameState<'State>) =
        if not state.IsDone then
            let dt = (float32 timer.ElapsedMilliseconds) / 1000.0f
            timer.Restart()
            { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate + dt }
            |> updateAndRenderFrame
            |> gameLoop

    { TimeSinceLastUpdate = 0.0f
      IsDone = false
      State = args.State
      Window = args.Window }
    |> gameLoop

let CalculateViewport aspectRatio (size : Vector2u) =
    let width = float32 size.X
    let height = float32 size.Y
    let ratio = width / height
    if ratio > aspectRatio then
        let expectedWidth = height * aspectRatio
        let dw = expectedWidth / width
        FloatRect((1.0f - dw) / 2.0f, 0.0f, dw, 1.0f)
    else
        let expectedHeight = width / aspectRatio
        let dh = expectedHeight / height
        FloatRect(0.0f, (1.0f - dh) / 2.0f, 1.0f, dh)
