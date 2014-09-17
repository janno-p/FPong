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
    let CalculateViewport' = CalculateViewport (4.0f / 3.0f)

    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use text = new Text("welcome", font, Color=Color.White, CharacterSize=20u)

    use view = new View(Vector2f(0.0f, 0.0f), Vector2f(200.0f, 100.0f))
    view.Viewport <- CalculateViewport' window.Size

    use background = new RectangleShape(Vector2f(200.0f, 100.0f), Position=Vector2f(-100.0f, -50.0f), FillColor=Color.Magenta)
    use playerPaddle = new RectangleShape(Vector2f(50.0f, 50.0f), Position=Vector2f(-100.0f, 0.0f), FillColor=Color.Green)
    use computerPaddle = new RectangleShape(Vector2f(50.0f, 50.0f), Position=Vector2f(0.0f, 0.0f), FillColor=Color.Green)

    let subscribeToEvents (state : List<Command>) = [
        window.KeyPressed
        |> Observable.subscribe (fun e -> if e.Code = Keyboard.Key.Escape then Quit |> state.Add)

        window.Closed
        |> Observable.subscribe (fun _ -> Quit |> state.Add)

        window.Resized
        |> Observable.subscribe (fun e -> view.Viewport <- CalculateViewport' (Vector2u(e.Width, e.Height)))
    ]

    let rec processCommands (state : GameState<State>) commands =
        match commands with
        | [] -> state
        | Quit :: xs -> processCommands { state with IsDone = true } xs

    let update (dt : float32) state =
        state

    let render state =
        window.Clear()
        window.SetView(view)
        window.Draw(background)
        window.Draw(playerPaddle)
        window.Draw(computerPaddle)
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
