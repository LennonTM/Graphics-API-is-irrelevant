# Graphics API is irrelevant (C# / WinForms)

A simple WinForms app that renders an animated, shader-like plasma and wave effect purely on the CPU, pixel by pixel, with no graphics library (no OpenGL/DirectX/SkiaSharp/etc.) — just `System.Drawing`, `System.Numerics`, and math.

This project started as a **tutorial** to learn how graphics/shaders work without relying on a graphics library, inspired by this video:

- https://www.youtube.com/watch?v=xNX9H_ZkfNE

The original tutorial builds the effect in **C++**. Here it has been adapted to **C# and WinForms**, rendering each frame to a `Bitmap` that is displayed in a `PictureBox` on a timer.

## About the shader code

The per-pixel color logic is a C# port of golfed GLSL shader code (originally written for shader compos like Twigl/Shadertoy), converted line-by-line into equivalent `Vector2`/`Vector4` math in `Program.cs`. Credit for the original shader code goes to:

- https://x.com/XorDev/status/1894123951401378051
- https://x.com/XorDev/status/2021258388038943162?s=20

Since GLSL isn't directly usable in C#, the vector/trig operations (`sin`, `cos`, `tanh`, `exp`, swizzles like `.yx`/`.xyyx`, etc.) were reimplemented as extension methods on `Vector2`/`Vector4`, and the shader's `mainImage`-style loop was rewritten as a per-pixel loop that writes into a `Bitmap`.

## How it works

- `Program.cs` computes the color for every pixel of the output image each frame (`render_image`), mimicking a fragment shader running per-pixel on the CPU.
- `Form1.cs` uses a `Timer` (ticking every ~16ms, ~60 FPS) to advance a `_time` value and repaint a `PictureBox` with the newly rendered `Bitmap` each tick.
- A button on the form toggles between effect variants (`ToggleSlide`).

## Requirements

- Windows OS (WinForms)
- .NET 10 SDK (project targets `net10.0-windows`)
- Visual Studio 2022 (or later) with the **.NET desktop development** workload, or the `dotnet` CLI

## Running the project

### Visual Studio
1. Open the solution/project in Visual Studio.
2. Set `Black Hole` as the startup project (if not already).
3. Press `F5` (or Ctrl+F5) to build and run.

### CLI
```powershell
cd "Black Hole"
dotnet run
```

Note: since the entire image is rendered on the CPU every frame, performance will depend on the window/PictureBox size and your machine's CPU speed.
