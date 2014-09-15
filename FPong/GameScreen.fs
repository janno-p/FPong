module FPong.GameScreen

open SFML.Graphics
open SFML.Window

let run (window : RenderWindow) = async {
    window.Closed.Add(fun _ -> window.Close())
    window.KeyPressed.Add(fun e -> if e.Code = Keyboard.Key.Escape then window.Close())

    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)

    while window.IsOpen() do
        window.DispatchEvents()
        window.Clear()
        window.Draw(shape)
        window.Display()
}
