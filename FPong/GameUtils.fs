module FPong.GameUtils

open SFML.Graphics
open System.Diagnostics

type GameState<'ScreenState> = {
    IsDone : bool
    TimeSinceLastUpdate : float32
    State : 'ScreenState
    Window : RenderWindow
}

let GameLoop update render (state : GameState<_>) =
    let timePerFrame = 1.0f / 60.0f

    let rec updateAndRenderFrame state =
        state.Window.DispatchEvents()
        match state.TimeSinceLastUpdate with
        | x when x < timePerFrame -> render state
        | _ -> { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate - timePerFrame }
               |> update timePerFrame
               |> updateAndRenderFrame

    let timer = Stopwatch()
    timer.Start()

    let rec gameLoop (state : GameState<_>) =
        if not state.IsDone then
            let dt = (float32 timer.ElapsedMilliseconds) / 1000.0f
            timer.Restart()
            { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate + dt }
            |> updateAndRenderFrame
            |> gameLoop
    gameLoop state

let InitGameState window state = { TimeSinceLastUpdate = 0.0f
                                   IsDone = false
                                   State = state
                                   Window = window }
