module FPong.GameScreen

open FPong.GameUtils
open SFML.Graphics
open SFML.Window
open System.Diagnostics

let run (window : RenderWindow) = async {
    //window.Closed.Add(fun _ -> window.Close())
    //window.KeyPressed.Add(fun e -> if e.Code = Keyboard.Key.Escape then window.Close())

    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)
    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use text = new SFML.Graphics.Text("welcome", font, Color=Color.White, CharacterSize=20u)

    let update (dt : float32) (state : GameState<int option>) =
        if Keyboard.IsKeyPressed(Keyboard.Key.Q) then
            { state with IsDone = true }
        else
            state

    let render state =
        window.Clear()
        window.Draw(shape)
        window.Draw(text)
        window.Display()
        state

    let state = None : int option

    let gameState = {
        TimeSinceLastUpdate = 0.0f
        IsDone = false
        State = state
        Window = window
    }

    gameState |> GameLoop update render
}
