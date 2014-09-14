module FPong.Program

open SFML.Graphics
open SFML.Window
open FPong.AssemblyInfo

[<EntryPoint>]
let main argv =
    use window = new RenderWindow(VideoMode(600u, 480u), (sprintf "FPong v.%s" assemblyVersion))
    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)

    window.Closed.Add(fun _ -> window.Close())
    window.KeyPressed.Add(fun e -> if e.Code = Keyboard.Key.Escape then window.Close())

    while window.IsOpen() do
        window.DispatchEvents()
        window.Clear()
        window.Draw(shape)
        window.Display()

    0
