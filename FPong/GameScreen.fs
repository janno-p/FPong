module FPong.GameScreen

open FPong.GameUtils
open SFML.Graphics
open SFML.Window
open System.Collections.Generic
open System.Diagnostics

type State =
    | State

type Command =
    | Quit

let run (window : RenderWindow) = async {
    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)
    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use text = new Text("welcome", font, Color=Color.White, CharacterSize=20u)

    let subscribeToEvents (state : List<Command>) = [
        window.KeyPressed
        |> Observable.subscribe (fun e -> if e.Code = Keyboard.Key.Escape then Quit |> state.Add)

        window.Closed
        |> Observable.subscribe (fun _ -> Quit |> state.Add)
    ]

    let rec processCommands (state : GameState<State>) commands =
        match commands with
        | [] -> state
        | Quit :: xs -> processCommands { state with IsDone = true } xs

    let update (dt : float32) state =
        state

    let render state =
        window.Clear()
        window.Draw(shape)
        window.Draw(text)
        window.Display()
        state

    { ProcessCommands = processCommands
      Update = update
      Render = render
      SubscribeToEvents = subscribeToEvents
      State = State
      Window = window }
    |> GameLoop
}
