module FPong.GameScreen

open FPong.GameUtils
open SFML.Graphics
open SFML.Window
open System.Collections.Generic
open System.Diagnostics

type State = { Score : uint32 * uint32 }

type Command =
    | Quit

let run (window : RenderWindow) = async {
    let CalculateViewport' = CalculateViewport (4.0f / 3.0f)

    let halfSize = Vector2f(200.0f, 150.0f)

    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use playerScore = new Text("0", font, Color=Color.White, CharacterSize=20u, Position=Vector2f(-50.0f, 20.0f - halfSize.Y))
    use computerScore = new Text("0", font, Color=Color.White, CharacterSize=20u, Position=Vector2f(50.0f, 20.0f - halfSize.Y))

    let size1 = playerScore.GetLocalBounds()
    playerScore.Origin <- Vector2f(size1.Width / 2.0f, size1.Height / 2.0f)

    let size2 = computerScore.GetLocalBounds()
    computerScore.Origin <- Vector2f(size2.Width / 2.0f, size2.Height / 2.0f)

    let paddleSize = Vector2f(10.0f, 50.0f)

    use view = new View(Vector2f(), halfSize * 2.0f)
    view.Viewport <- CalculateViewport' window.Size

    let lineWidth = 2.0f

    use background = new RectangleShape(Vector2f(halfSize.X - lineWidth, halfSize.Y - lineWidth) * 2.0f)
    background.Position <- Vector2f(lineWidth - halfSize.X, lineWidth - halfSize.Y)
    background.FillColor <- Color.Transparent
    background.OutlineColor <- Color.White
    background.OutlineThickness <- lineWidth

    use playerPaddle = new RectangleShape(paddleSize, Position=Vector2f(-halfSize.X + 10.0f, 0.0f), FillColor=Color.Green, Origin=(paddleSize / 2.0f))
    use computerPaddle = new RectangleShape(paddleSize, Position=Vector2f(halfSize.X - 10.0f, 0.0f), FillColor=Color.Green, Origin=(paddleSize / 2.0f))

    let middleLine = [| Vertex(Vector2f(0.0f, -halfSize.Y)); Vertex(Vector2f(0.0f, halfSize.Y)) |]

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
        window.Draw(middleLine, PrimitiveType.Lines)
        window.Draw(playerScore)
        window.Draw(computerScore)
        window.Draw(playerPaddle)
        window.Draw(computerPaddle)
        window.Display()
        state

    { ProcessCommands = processCommands
      Update = update
      Render = render
      SubscribeToEvents = subscribeToEvents
      State = { Score = (0u, 0u) }
      Window = window }
    |> GameLoop
}
