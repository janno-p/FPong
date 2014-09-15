module FPong.Program

open SFML.Graphics
open SFML.Window
open FPong

[<EntryPoint>]
let main argv =
    let videoMode = VideoMode(600u, 480u)
    let titleText = sprintf "FPong v.%s" AssemblyInfo.assemblyVersion

    use window = new RenderWindow(videoMode, titleText)

    window.SetVerticalSyncEnabled(true)

    async {
        do! GameScreen.run window
    } |> Async.StartImmediate

    0
