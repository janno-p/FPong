module FPong.GameScreen

open SFML.Graphics
open SFML.Window
open System.Diagnostics

type State = {
    TimeSinceLastUpdate : float32
    Running : bool
}

let run (window : RenderWindow) = async {
    //window.Closed.Add(fun _ -> window.Close())
    //window.KeyPressed.Add(fun e -> if e.Code = Keyboard.Key.Escape then window.Close())

    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)
    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use text = new SFML.Graphics.Text("welcome", font, Color=Color.White, CharacterSize=20u)

    let timePerFrame = 1.0f / 60.0f

    let timer = Stopwatch()
    let timeSinceLastUpdate = ref (float32 timer.ElapsedMilliseconds)
    timer.Start()

    let update (dt : float32) (state : State) =
        if Keyboard.IsKeyPressed(Keyboard.Key.Q) then
            { state with Running = false }
        else
            state

    let render (state : State) =
        window.Clear()
        window.Draw(shape)
        window.Draw(text)
        window.Display()
        state

    let rec updateAndRenderFrame (state : State) =
        window.DispatchEvents()
        match state.TimeSinceLastUpdate with
        | x when x < timePerFrame -> render state
        | _ -> { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate - timePerFrame }
               |> update timePerFrame
               |> updateAndRenderFrame

    let rec gameLoop (state : State) =
        if state.Running then
            let dt = (float32 timer.ElapsedMilliseconds) / 1000.0f
            timer.Restart()
            { state with TimeSinceLastUpdate = state.TimeSinceLastUpdate + dt }
            |> updateAndRenderFrame
            |> gameLoop

    let state = { TimeSinceLastUpdate = 0.0f
                  Running = true }

    gameLoop state
}
