# Quick Start Guide (with C# examples!)
## Overview
This guide has some "quick and dirty" example usages for this project. For a detailed walk through of how to use the library to its fullest, see the [Detailed Usage Guide](detailed_usage.md)

Before coding with the examples below, you'll want to either import the `DreamCheekyLED` project into your Visual Studio solution outright, or reference the `DreamCheekyLED.dll` instead. This library relies on the Apache 2.0-licensed  nuget package [HidSharp](https://software.seekye.com/hidsharp) ([GitHub](https://github.com/IntergatedCircuits/HidSharp), [Nuget](https://www.nuget.org/packages/HidSharp))

If you need to load images from a file, you'll have to add either the `DreamCheekyLEDExtras` project to your solution or a reference to `DreamCheekyLEDExtras.dll`, which relies on the MIT-licensed nuget package [SkiaSharp](https://mono.github.io/SkiaSharp/) ([GitHub](https://github.com/mono/skiasharp), [Nuget](https://www.nuget.org/packages/SkiaSharp/)).

For the examples on this page, we will be doing both.

## Loading an animated image from a file path

[!code-csharp[](example_consoleapp_full_multithread.cs)]

## Loading a static image from a file path

[!code-csharp[](example_consoleapp_full_singlethread.cs)]

## Storing a single screen for later use

[!code-csharp[](example_quickframebuffer.cs)]