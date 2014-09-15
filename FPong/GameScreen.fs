module FPong.GameScreen

open SFML.Graphics
open SFML.Window
open System.Diagnostics

let run (window : RenderWindow) = async {
    window.Closed.Add(fun _ -> window.Close())
    window.KeyPressed.Add(fun e -> if e.Code = Keyboard.Key.Escape then window.Close())

    use shape = new RectangleShape(Vector2f(75.0f, 50.0f), Position=Vector2f(100.0f, 100.0f), FillColor=Color.Green)
    use font = new Font("/usr/share/fonts/truetype/freefont/FreeSans.ttf")
    use text = new SFML.Graphics.Text("welcome", font, Color=Color.White, CharacterSize=20u)

    let timePerFrame = 1.0f / 60.0f

    let timer = Stopwatch()
    let timeSinceLastUpdate = ref (float32 timer.ElapsedMilliseconds)
    timer.Start()

    while window.IsOpen() do
        window.DispatchEvents()

        timeSinceLastUpdate := !timeSinceLastUpdate + ((float32 timer.ElapsedMilliseconds) / 1000.0f)
        timer.Restart()

        while !timeSinceLastUpdate > timePerFrame do
            timeSinceLastUpdate := !timeSinceLastUpdate - timePerFrame
            window.DispatchEvents()

        window.Clear()
        window.Draw(shape)
        window.Draw(text)
        window.Display()
}
