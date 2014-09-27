module FPong.GameScreen

open FPong.GameUtils
open SFML.Graphics
open SFML.Window
open System.Collections.Generic
open System.Diagnostics

type State = { Position : float32
               Score : uint32 * uint32 }

type Command =
    | MoveToPosition of float32
    | Quit

let run (window : RenderWindow) = async {
    let aspectRatio = 4.0f / 3.0f

    let CalculateViewport' = CalculateViewport aspectRatio

    let halfSize = Vector2f(aspectRatio * 150.0f, 150.0f)

    use font = new Font(FontPath "VCR_OSD_MONO.ttf")
    use playerScore = new Text("0", font, Color=Color.White, CharacterSize=20u, Position=Vector2f(-50.0f, 20.0f - halfSize.Y))
    use computerScore = new Text("0", font, Color=Color.White, CharacterSize=20u, Position=Vector2f(50.0f, 20.0f - halfSize.Y))

    let size1 = playerScore.GetLocalBounds()
    playerScore.Origin <- Vector2f(size1.Width / 2.0f, size1.Height / 2.0f)

    let size2 = computerScore.GetLocalBounds()
    computerScore.Origin <- Vector2f(size2.Width / 2.0f, size2.Height / 2.0f)

    let paddleSize = Vector2f(10.0f, 50.0f)

    use view = new View(Vector2f(), halfSize * 2.0f)
    view.Viewport <- CalculateViewport' window.Size

    let lineWidth = 4.0f

    use background = new RectangleShape(Vector2f(halfSize.X, halfSize.Y - lineWidth) * 2.0f)
    background.Position <- Vector2f(-halfSize.X, lineWidth - halfSize.Y)
    background.FillColor <- Color.Transparent
    background.OutlineColor <- Color.White
    background.OutlineThickness <- lineWidth

    use playerPaddle = new RectangleShape(paddleSize, Position=Vector2f(halfSize.X - 10.0f, 0.0f), FillColor=Color.Green, Origin=(paddleSize / 2.0f))
    use computerPaddle = new RectangleShape(paddleSize, Position=Vector2f(-halfSize.X + 10.0f, 0.0f), FillColor=Color.Green, Origin=(paddleSize / 2.0f))

    use middleLine = new VertexArray(PrimitiveType.Lines)
    middleLine.Append(Vertex(Vector2f(0.0f, -halfSize.Y)))
    middleLine.Append(Vertex(Vector2f(0.0f, halfSize.Y)))

    let windowCenter = window.Size / 2u
    let mouseOrigin = Vector2i(int windowCenter.X, int windowCenter.Y)
    Mouse.SetPosition(mouseOrigin, window)

    window.SetMouseCursorVisible(false)

    let subscribeToEvents (state : List<Command>) = [
        window.KeyPressed
        |> Observable.subscribe (fun e -> if e.Code = Keyboard.Key.Escape then Quit |> state.Add)

        window.Closed
        |> Observable.subscribe (fun _ -> Quit |> state.Add)

        window.Resized
        |> Observable.subscribe (fun e -> view.Viewport <- CalculateViewport' (Vector2u(e.Width, e.Height)))

        window.MouseMoved
        |> Observable.subscribe (fun e -> let v = window.MapPixelToCoords(Vector2i(e.X, e.Y), view)
                                          MoveToPosition v.Y |> state.Add)
    ]

    let rec processCommands (state : GameState<State>) commands =
        match commands with
        | [] -> state
        | MoveToPosition pos :: xs -> processCommands { state with State = { state.State with Position = pos } } xs
        | Quit :: xs -> processCommands { state with IsDone = true } xs

    let update (dt : float32) (state : GameState<State>) =
        let offset = lineWidth + playerPaddle.Size.Y / 2.0f
        let pos = max (min state.State.Position (halfSize.Y - offset)) (-halfSize.Y + offset)
        playerPaddle.Position <- Vector2f(playerPaddle.Position.X, pos)
        state

    let render state =
        window.Clear()
        window.SetView(view)
        window.Draw(background)
        window.Draw(middleLine)
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
      State = { Position = 0.0f; Score = (0u, 0u) }
      Window = window }
    |> GameLoop
}
