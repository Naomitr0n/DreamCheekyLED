# Detailed Usage Guide - C#
## Overview

This library enables both easy, hands-off control of a DreamCheekyLED device with minimal effort, while also allowing a fine-grained level of in-depth control too. If all you really want is to have it "just work," the [Quick Start Guide](quickstart.md) will have all you need to get started. If, however, you want more fine-grain control, this guide is for you.

While there *is* a [Developer Manual](https://www.last-outpost.com/~malakai/dcled/USB%20LED%20Message%20Board%20-%20Developer%20Manual%20v1.0.pdf) (also available multiple times over on [Archive.org](https://web.archive.org/web/*/https://www.last-outpost.com/~malakai/dcled/USB%20LED%20Message%20Board%20-%20Developer%20Manual%20v1.0.pdf)), I found it to be at best only a somewhat good starting point. I found oddities that were outright undocumented, incorrect, literally *backwards* even. While the developer manual states that is for the `USB LED Message Board v1.0`, the *physical* device I have self-identifies as a `Dream Link USB LED Message Board v1.0.1`, with the same Vendor ID and Product ID as the developer manual contains (the developer manual specifies it in hexadecimal, while the C# code specifies it in decimal). It's hard to believe that a version so insignificantly different in version numbering would be so drastically different in USB protocol.

For example, the developer manual ***extremely clearly*** states that the ***first*** byte sent to it should be the brightness value, followed by which row group to update, the first of the two rows, then the second row, for a total of *eight* bytes. In reality, *it is **absolutely** required to send an additional byte **first***, making it a total of ***nine*** bytes. No explanation, no documentation, no rhyme or reason.

***This undocumented first byte might actually perform a function***, but `0xFF` works just fine, so I haven't tried it. The code that inserts the 0xFF always is at the bottom of `LEDArray.MakeFrameBuffer()` if anyone has any desire to play around with it. Maybe it's related to the screen timeout, or some unrelated function, maybe it's a feature they didn't expand on, who knows, maybe the thought was to allow multiple 21x7 displays to daisy-chain together and that first byte might've specified *which* of the displays to update. If anyone figures it out, let me know, and if I can confirm it, I'll update the library and the documentation accordingly.

But that's not the only problem I found in the documentation. It very clearly states "the value of brightness ranged from 0 to 2. 0 is the maximum brightness" when in *reality,* zero is the *minimum* brightness. 2 is the maximum. Not only that, but it gives a table under the header of `2.3.1 Row Data` for which LED's to specify where that's *completely* wrong. I had to "trial and error" the right approach and found out that sure enough, whoever wrote the documentation just never tested to see if it was right. Worse yet, since it's been so long since the company was around *at all*, good luck even *knowing* if there was ever a revision to the developer manual that corrected this. There's a `Change Notes` section that shows that the developer manual was only released, never revised, with v1.0 of it dating to November 12, 2008.

**If you come across a newer version of the Dream Cheeky LED USB Message Board Developer Manual, *please* open an issue ticket on the GitHub for this library**

## USBInterface

<xref:DreamCheekyLED.USBInterface> implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), meaning you can use it in a `using` statement to avoid having to call its <xref:DreamCheekyLED.USBInterface.Dispose> method directly.

For example:

[!code-csharp[](example_open.cs)]

If you do not need (or want) to worry about exception handling, you can use the `TryOpen()` method instead, like this:

[!code-csharp[](example_tryopen.cs)]

While you *could* call <xref:DreamCheekyLED.USBInterface.Close(System.Boolean)>, this is *not* intended behavior as <xref:DreamCheekyLED.USBInterface.Dispose> calls [Close(false)](xref:DreamCheekyLED.USBInterface.Close(System.Boolean)) within it. Disposing and re-initializing <xref:DreamCheekyLED.USBInterface> per connection by using a `using` block is the intended behavior. There is no benefit to keeping the <xref:DreamCheekyLED.USBInterface> object in memory while keeping it closed.

Even though the DreamCheekyLED device has 21 columns and 7 rows, the way the USB protocol works is by specifying data for two rows simultaneously. These are grouped in row pairs: rows 1 and 2, rows 3 and 4, rows 5 and 6, and row 7. When row 7 data is sent, the second row is simply ignored by the device itself.



In order to display data on the DreamCheekyLED device, we need to set the Frame Buffer with 
<xref:DreamCheekyLED.USBInterface>.SetFrameBuffer(), either [by row group](xref:DreamCheekyLED.USBInterface.SetFrameBuffer(System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})), or  [for the whole screen](xref:DreamCheekyLED.USBInterface.SetFrameBuffer(System.Collections.Generic.IEnumerable{System.Byte})). Once done, a running <xref:DreamCheekyLED.USBInterface> will continue to send the same frame packets to the device with 1ms pauses between each packet, 

The easiest way to do this is with the <xref:DreamCheekyLED.LEDArray> helper class. You can generate a frame buffer with either its [MakeFrameBuffer()](xref:DreamCheekyLED.LEDArray.MakeFrameBuffer(System.Byte,System.Byte,System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})) or [MakeFrameBufferArray()](xref:DreamCheekyLED.LEDArray.MakeFrameBufferArray(System.Byte,System.Byte,System.Byte[],System.Byte[])) method, depending on whether you want an [IEnumerable&lt;byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) (for example, to convert to a [List&lt;byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)) or an `array` (respectively), each with the same output (just in different collection types). You could choose to either cache the output from one of the <xref:DreamCheekyLED.LEDArray> MakeFrameBuffer methods and supply it each time to the respective `SetFrameBuffer()` method, or generate it each time on the fly.

To call the [MakeFrameBuffer()](xref:DreamCheekyLED.LEDArray.MakeFrameBuffer(System.Byte,System.Byte,System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})) or [MakeFrameBufferArray()](xref:DreamCheekyLED.LEDArray.MakeFrameBufferArray(System.Byte,System.Byte,System.Byte[],System.Byte[])) methods, you will need to supply the bytes of each row, which can be generated by [LEDArray.BytesFromBools()](xref:DreamCheekyLED.LEDArray.BytesFromBools(System.Collections.Generic.IEnumerable{System.Boolean})) or [LEDArray.ByteArrayFromBools()](xref:DreamCheekyLED.LEDArray.ByteArrayFromBools(System.Boolean[])) to generate an [IEnumerable&lt;byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) or an `array` (respectively), each also with consistent output that could be cached if desired. Each takes an array of exactly 21 `bool` values, where `true` indicated an illuminated LED, and `false` indicates one turned off.

For example, if you had the following [IEnumerable&lt;byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) collections:

[!code-csharp[](example_rowbools.cs)]

Once you fill them with exactly 21 values each, you can create the byte collections from their respective [LEDArray.BytesFromBools()](xref:DreamCheekyLED.LEDArray.BytesFromBools(System.Collections.Generic.IEnumerable{System.Boolean})) method calls like so:

[!code-csharp[](example_rowbytes.cs)]

## Updating Frame Buffers
### By Row Group

The Dream Cheeky LED unit receives updates by sending it two rows at a time. This is why if you generate byte collections per row, you will need to supply them per row *group*. Note that you cannot select two arbitrary rows to update, rather you **must** select rows 1-2, or 3-4, or 5-6, or simply row 7. Despite row 7 being only one row, the frame buffer still requires two rows of data to be sent to the device, though it seems to ignore the second row of data sent to it when updating row 7. This is why the <xref:DreamCheekyLED.LEDArray> MakeFrameBuffer methods accept their `LowerRow` parameter as optional, however will throw an [ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception) if any other row group is specified for an update.

When calling the <xref:DreamCheekyLED.LEDArray> MakeFrameBuffer methods, you need to specify the brightness, (the helper class <xref:DreamCheekyLED.LEDArray.Brightness> can assist with this), the row group identifier (the helper class <xref:DreamCheekyLED.LEDArray.RowGroups> exists to assist with this), and the row data generated by the relevant <xref:DreamCheekyLED.LEDArray> byte collection from bool collection method. For example:

[!code-csharp[](example_makeframebuffer_header.cs)]

Note the addition of `lstFrameBufferScreen` that lets you cache the results **per screen**. (This is entirely optional!!!) Once the row group frame data is ready to send, all that's left is to set the frame buffer with [USBInterface.SetFrameBuffer()](xref:DreamCheekyLED.USBInterface.SetFrameBuffer(System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})), call the <xref:DreamCheekyLED.USBInterface.Run> method to display the contents, then call the <xref:DreamCheekyLED.USBInterface.Stop> method to stop displaying the contents once you're done. While you *could* use the <xref:DreamCheekyLED.USBInterface.Run(System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})> overload to set the initial frame buffer while running at the same time, it's best practice to set the frame buffer ***before*** trying to call the <xref:DreamCheekyLED.USBInterface.Run> method to display anything, which is best done within a `using` block, like so:

[!code-csharp[](example_mini_skeleton_rowgroup.cs)]

Alternatively, if sending the frame buffer for the entire screen (whether due to cache or convenience), you would use the [USBInterface.SetFrameBuffer(IEnumerable<byte>)](xref:DreamCheekyLED.USBInterface.SetFrameBuffer(System.Collections.Generic.IEnumerable{System.Byte})) overload to send the frame data for the entire screen at once, like so:

[!code-csharp[](example_mini_skeleton_screen.cs)]

Now, as-is, this code will run and exit **immediately**, only flashing the contents on the screen once, if even at all. This is due to the <xref:DreamCheekyLED.USBInterface.Run> method being asynchronous; it creates a new processing thread to handle the display updates then returns pretty immediately (more on how that works later). The Dream Cheeky LED USB device *itself* meanwhile has an internal cache that stores frame data for about a quarter of a second (or 250ms), so if you want to display a single image for only about one second total, you could add `Thread.Sleep(750);` to the `"// Do stuff here!"` section as shown here:

[!code-csharp[](example_mini_sleep_rowgroup.cs)]

When done, it should look like this:

[!code-csharp[](example_all_rowgroup.cs)]

Or, if updating the entire screen at once, like this:


[!code-csharp[](example_all_screen.cs)]

Flexibility is always nice, but this code sure looks repetitive and hard on the eyes, doesn't it?

Enter the <xref:DreamCheekyLED.ScreenData> class

### By Screen (The Easy Way)
The <xref:DreamCheekyLED.ScreenData> class simplifies the process of populating and filling the entire screen all at the same time. Rather than rely on the developer to keep track of which rows and row groups are which, the ScreenData class **does all the work for you**. It can genuinely be this easy:

[!code-csharp[](example_screendata_minimal.cs)]

MUCH better, right? You can also specify the brightness in the constructor, or even only the brightness, or nothing at all and default to an empty screen by using a different <xref:DreamCheekyLED.ScreenData> constructor overload. You could also use the <xref:DreamCheekyLED.ScreenData.FillRowFromBools(System.Int32,System.Collections.Generic.IEnumerable{System.Boolean})> method to do it row-by-row, or the [ScreenData.FillFromBools()](xref:DreamCheekyLED.ScreenData.FillFromBools(System.Collections.Generic.IEnumerable{System.Boolean})) to do it for the whole screen at once.

Once done, you can generate the frame buffer either as an [IEnumerable&lt;byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) with [ScreenData.MakeFrameBuffer()](xref:DreamCheekyLED.ScreenData.MakeFrameBuffer(System.Nullable{System.Byte})) or as an `array` with [ScreenData.MakeFrameBufferArray()](xref:DreamCheekyLED.ScreenData.MakeFrameBufferArray(System.Nullable{System.Byte})), both taking optional parameter `Brightness` which will override the current setting in the produced Frame Buffer (but does **not** replace the <xref:DreamCheekyLED.ScreenData.Brightness> property; that must be done separately).

By default, if not otherwise specified anywhere, <xref:DreamCheekyLED.ScreenData> will **always** use the maximum brightness. It does not explicitly need to be set to "unlock" brighter options. Only medium or dimmest require setting the brightness at any point.

Say that you just want to start slowly. Here is an example of what that could look like:

[!code-csharp[](example_screendata_constructor_empty.cs)]

There are multiple easy ways to populate row data onto the screen using this method. But, what if you already know ahead of time and just want to make things cleaner? This is where the other constructors come into play. For example, if you have each row stored individually, you can fill the ScreenData per row like so:

[!code-csharp[](example_screendata_constructor_perrow.cs)]

Or if you have all (21 x 7 = 147) elements of whole screen in one go, you can do it like so:

[!code-csharp[](example_screendata_constructor_perscreen.cs)]

Say that you already have the Row Group frame buffers made and want to put those in a <xref:DreamCheekyLED.ScreenData> object for whatever reason. You can do that too with [Row Group Constructor Overload](xref:DreamCheekyLED.ScreenData.%23ctor(System.Byte,System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte},System.Collections.Generic.IEnumerable{System.Byte})). However, due to the hackish nature of such (it discards *all* data from the frame array that isn't directly the LED on/off states), there is no `FillFromBools()` vairant.

Once you've filled the <xref:DreamCheekyLED.ScreenData> with data, you can call the [ScreenData.MakeFrameBuffer()](xref:DreamCheekyLED.ScreenData.MakeFrameBuffer(System.Nullable{System.Byte})) method or it's `array`-based equivalent [ScreenData.MakeFrameBufferArray()](xref:DreamCheekyLED.ScreenData.MakeFrameBufferArray(System.Nullable{System.Byte})) to generate the **full** Frame Buffer for the entire screen to send to [ScreenData.SetFrameBuffer()](xref:DreamCheekyLED.USBInterface.SetFrameBuffer(System.Collections.Generic.IEnumerable{System.Byte})), which you could also store in a cache if desired. You can even compare <xref:DreamCheekyLED.ScreenData> objects against each other to see if they contain the same data with the <xref:DreamCheekyLED.ScreenData.Equals(System.Object)> override, which only checks to see if the Brightness and Row Data values are the same (i.e. if it will produce the same output from [ScreenData.MakeFrameBuffer()](xref:DreamCheekyLED.ScreenData.MakeFrameBuffer(System.Nullable{System.Byte})), simplifying cache duplicate checks.

Combine all of that, and you can get ***much*** more readable code, at the expense of less access to the data along the way:

[!code-csharp[](example_screendata_skeleton.cs)]

But what about the end user? Well, you could build a UI to control all this readily (I'm sure some using this library will), and even serialize the ScreenData object to allow for saving it to disk. Or you could avoid that step entirely and simply load an image to display, freeing up any code touching this to do a different task. Enter...

## The ImageLoader - DreamCheekyLEDExtras library

You may wonder why <xref:DreamCheekyLED.ImageLoader> sits in a different C# project, yet shares the same namespace. Ultimately, *it is an extra, not a requirement*, and it requires additional external dependencies to run. Specifically, it requires the MIT-licensed [SkiaSharp](https://mono.github.io/SkiaSharp/) ([GitHub](https://github.com/mono/skiasharp), [Nuget](https://www.nuget.org/packages/SkiaSharp/)). For those who do not need nor want this -- or who may be already using ImageMagick for example and would rather use that instead -- you can simply leave this project out of a .NET solution, and/or its DLL unused.

There are two methods to ImageLoader: [ImageLoader.AnimatedScreenDataFromPath()](xref:DreamCheekyLED.ImageLoader.AnimatedScreenDataFromPath(System.String,System.Boolean)) (which returns an [IEnumerable&lt;FrameData&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)), and [ImageLoader.StaticScreenDataFromPath()](xref:DreamCheekyLED.ImageLoader.StaticScreenDataFromPath(System.String,System.Boolean)) (which returns an <xref:DreamCheekyLED.ImageLoader.FrameData>), depending on whether you want an animated image, or a static one respectively. The animated variant loads static images as single-frame images and will return a [IEnumerable&lt;FrameData&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) with only one element in it, but the static variant is explicitly meant for static images and will ignore animated data. If either can successfully read an image, it will do so, top-left aligned. For each of these, **ANY** pixel that is read which is neither black (RGB `#000000`) nor fully transparent (with an RGBA Alpha channel value other than `00`) will be made illuminated. The **very** first pixel read (per frame for animated images) that indicates an illuminated LED will set the brightness value for the whole frame, based on the [SKColor.ToHSV()](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.skcolor.tohsv?view=skiasharp) Value percentage -- if the V is 75% or higher, full brightness will be used. If the V is 50% to 74.99%, medium brightness will be used. Anything below 50% will use the dimmest brightness available.

Images will also be read upscaled, though it only grabs the central pixel per upscaled "pixel" rather than trying to center it. For example, if a 4x-scaled image is supplied (with dimensions of 84 x 28), it will reference pixels at XY locaitons of (2, 2), (6, 2), (10, 2), (14, 2), etc. If a 5x-scaled image is supplied (with dimensions of 105 x 35), it will reference pixels at XY locations of (3, 3) (8, 3), (13, 3), (18, 3), etc. This allows more rough creation of images at higher resolutions in case someone's image editor struggles to be usable at small sizes on a high-DPI screen, or even for ease of seeing the image that will be displayed. For example, here is a 21x7 image that is provided with the library:

![21x7 test GIF for ImageLoader](img/tests/complex_test.gif)

Obviously, this image is pretty small, and hard to see the individual pixels of. For an easier "human readable" version, here is one upscaled by 10x to a 210x70 image that is also provided with the library:

![21x7 test GIF for ImageLoader](img/tests/complex_test_10x.gif)

*Much* easier to see at that scale, and both work just the same.

The above two images are the easiest format to understand -- WYSIWYG (or "What You See Is What You Get"), but, what about a more complex one? Take for example this input GIF:

![21x7 test GIF for ImageLoader](img/tests/extended_test_10x_input.gif)

The first three frames are differing dot patterns at dim, medium, and full brightness, followed by a **fully black** frame. After that are dot patterns where ***only the first pixel checked*** changes the HSV Value amount to change brightness from full brightness to medium then dimmest, followed by a **fully transparent** frame. As long as it is loaded with `ImageLoader.AnimatedScreenDataFromPath(pathToImage: "path_to_this_gif", autoSetBrightness: true)` (the `autoSetBrightness` parameter can be omitted also as it defaults to `true`), it should look like this on the device itself:

![21x7 test GIF for ImageLoader](img/tests/extended_test_10x_expected_red.gif)

These images are also included with the project itself. Here's a super simple example on how to load these images:

[!code-csharp[](example_imageloader_skeleton.cs)]

But, we still haven't addressed something obvious by now: instead of only displaying the whole animation exactly one time, how do we *keep doing it?* 

Enter asynchronous coding!

## The `async` and `await` keywords

This is what the DreamCheekyPathLoaderApp does. Instead of having to loop it over and over and tie up resources, we can simply create a new processing thread and only wait for it *only when we want to.* .NET does this by utilitizing two keywords -- `async` and `await`, and leveraging the [System.Threading.Tasks.Task](
https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) class to specify a new processing thread to use *asynchronously* (i.e. `async`), meaning, "running in the background on its own while the code I'm writing runs," unless you decide to sync it up by asynchronously waiting (i.e. `await`) for the processing thread that branched off to tidy up and complete. Yes, this is multithreading! You can read a much, ***much*** more technical and involved document on it in the Microsoft .NET documentation [here](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/) that goes ***super*** in-depth, but we'll walk through the basics and even provide a few skeleton sections of code you can readily copy-paste to various projects in this section, before circling back to how to integrate this with the DreamCheekyLED library.

To code asynchronously, you'll need to have a `static` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) variable. I've chosen to put it outside the main loop in the `DreamCheekyPathLoaderApp` and declare it as `private static`, but you could also put it within only one method if that is the entirety of the context it will be used in. Given this project could be reference material for others, I've chosen to make it available to the whole class, should someone want to create a new method to handle other tasks and still want access to the [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) itself. Here is an example of what that looks like: 

[!code-csharp[](example_multithread_start.cs)]

I **highly** recommend a [System.Threading.CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource) be used as well in case you need to kill the process early (such as when exiting). Where you define this follows the same reasoning as the [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) variable: while it can readily stay within whatever method is creating our [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) varilable, due to this being a potential educational resource, I'll make it global and `private static` to make it more intuitive to fork, placing it *just* under our [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) variable like so:

[!code-csharp[](example_multithread_canceltoken.cs)]

So now we have both a processing thread, and a variable that is meant to stop it. But what do we *do* about that? Enter the `async` method. Here's a rudimentary skeleton that you can use for various projects:

[!code-csharp[](example_multithread_asyncmethod_skeleton.cs)]

The beauty of using a [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource) is the ability to ***cleanly*** cancel a processing thread. When you call its [Cancel()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancel) method, it sets the [CancellationTokenSource.IsCancellationRequested](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.iscancellationrequested#system-threading-cancellationtokensource-iscancellationrequested) property to `true`, and can also throw a [OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception). That's why the `RunLoop(CancellationToken)` above not only has the error handling it does, but *also* checks the [IsCancellationRequested](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.iscancellationrequested#system-threading-cancellationtokensource-iscancellationrequested) property rather than relying on any other flag. Best of all, you can do so without having to touch any global variables, or even touch the loop itself, and the [Cancel()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancel) call has an overload capable of handling exceptions more delicately should the need arise.

Once you assign the [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) variable to an `async` method like we did in `RunLoop(CancellationToken)` above, stopping that [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) *cleanly* is as simple as calling the [Cancel()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancel) method of the [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource) that you passed to the `async` `RunLoop()` method, then calling the [Wait()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.wait#system-threading-tasks-task-wait) method of that same [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task), which also has plenty of [overloads](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.wait) that let you tweak the behavior a bit. This will let the `async` `RunLoop()` method tidy up on its own, then wait for it to finish before continuing. Here's what that looks like in its simplest form:

[!code-csharp[](example_multithread_cancelstart.cs)]

Tying this into your `Main()` method and adding the `async` `RunLoop()` method from two examples above, you should get something like this:

[!code-csharp[](example_multithread_singlebasic_skeleton.cs)]

Remember, the `_cts` and `_task` variables are assigned globally *purely* for easy re-use of this library; they can just as readily be declared locally, *within* the `Main()` method above, without issue.

All that's great, but what about something that stays running? A forecast that only updates on a schedule, a clock, even just a listener service that waits until called and then displays an image. While there's definitley benefit to creating a processing thread regardless, For anything that loops, the [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) and [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource) combination *shines.*

Say that you have this loop inside your `Main()` run method:

[!code-csharp[](example_multithread_loopstart.cs)]

This can *readily* be adapted to use a new processing [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) with a [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource), like so:

[!code-csharp[](example_multithread_loopbasic.cs)]


Now that we've rather quickly caught you up to speed on what asynchronous coding is, let's walk through how to *use* it.

## Using USBInterface Asynchronously - DreamCheekyPathLoaderApp

The most obvious disparity with using just one processing thread and asynchronous coding is that you really don't want to be creating and disposing a bunch of <xref:DreamCheekyLED.USBInterface> variables to constantly disconnect and reconnect to the same device. It's ***much*** better to only connect once, and ***remain*** connected to it for the duration of its use, and is considered good coding practice as well. This is how the `DreamCheekyPathLoaderApp` works, which is what runs when you click the `Start` button after opening the Visual Studio solution as it is the default project. And thanks to it implementing [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), we can make this much clearer by putting it in a `using` block within the main method, like so:

[!code-csharp[](example_multithread_loopusbstart.cs)]

The `DreamCheekyPathLoaderApp` simply opens up a command prompt window that asks for an image path, but also accepts image paths as a command line argument like so:

> `DreamCheekyPathLoaderApp.exe "C:\Path\To\Display\Image.gif"`

Alternatively, when you load it up, you could just type in [`C:\Path\To\Display\Image.gif`] and it would start playing the image all the same. Due to the fact that it displays animated GIF images, but allows you to change them at will (even mid-animation), the main thread is for the UI, while the secondary processing thread handles the frame animation. To do that, it takes all the shortcuts we've learned up until this point, including the <xref:DreamCheekyLED.ImageLoader>. First though, let's focus on the file path logic, leaving out all the multithreading and <xref:DreamCheekyLED.ImageLoader> use, so the purpose of the main loop is much clearer:

[!code-csharp[](example_filepath_start.cs)]

You'll notice the `shouldFinish` flag has now been replaced with simply reading the command prompt (or [System.Console](https://learn.microsoft.com/en-us/dotnet/api/system.console) to .NET) until the user only types "Q", whether upper or lowercase, which is pretty standard in terminal-based applications. It also does a quick check to see if the file even *exists* before trying to load anything, and simply ignores the user and enters the prompt again if it sees a blank string, which it trims leading and trailing whitespace on both when parsing from the Console `args` *and* when parsing from user unput at the end of the `do ... while` loop, negating the need to do so within the `if (!String.IsNullOrEmpty(filePathInput))` check. There is also a clear and logical spot for where to start working with a valid file path, while <xref:DreamCheekyLED.USBInterface> has been left open ***outside*** all this.

For an animated GIF, the problem becomes immediatley obvious. How do you loop through the animation frames to display each one with the appropriate timing, *and* wait for user input on the next file path, at the same time? *Maybe* it's possible to check between each frame update, but what if the first animation frame takes several seconds, and the user copy-pastes a different file path before it's even done? Are we forced to wait? Nope, this is why I walked you through the `async` and `await` operators in the last section. The `Main()` method belongs to the command prompt. It does command prompt things. That means user input, checking for valid file paths, telling the user if they entered an invalid file path, and even exiting. Beyond sending a valid file path off somewhere, that's all it needs to do. So, let's write it like that, tying the last two examples together and letting the <xref:DreamCheekyLED.ImageLoader> parse the data to provide to the <xref:DreamCheekyLED.USBInterface>

[!code-csharp[](example_consoleapp_main.cs)]

There is one pretty big change: this won't compile as-is, as the `RunLoop()` method only takes a single argument of a [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource). And this change comes with good reason: as we've established, the command prompt does command prompt things, *not* animated GIF things, it's the ***new*** processing thread that we're dedicating for handling the animation. So, let's rewrite the method header to send it the animation itself, and the thing to send it to, like so:

[!code-csharp[](example_consoleapp_runloop_before.cs)]

Now obviously, this doesn't really do anything. Let's start by setting the frame buffer to the first frame, then telling the <xref:DreamCheekyLED.USBInterface> to start running before the loop even starts, like so:

[!code-csharp[](example_consoleapp_runloop_firstframe.cs)]

Now let's adjust the main body of the loop. We want to loop through each frame (still starting at the first frame, even though we already set the initial frame buffer), set the frame buffer to that frame, then wait the amount of time the animated GIF has specified for this particular frame as long as it hasn't been requested to cancel yet.

[!code-csharp[](example_consoleapp_runloop_frameloop.cs)]

You might notice that we pass the [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource) to [Task.Delay()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay#system-threading-tasks-task-delay(system-int32-system-threading-cancellationtoken)). This allows it to exit abruptly and throw an  [OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception) exception, instead of waiting any further. That way, if your first animation frame lasts 30 seconds, but the user has already requested a new animated GIF be displayed four seconds in, the [Task.Delay()](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay#system-threading-tasks-task-delay%28system-int32-system-threading-cancellationtoken%29) method will throw a [OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception) as it will see the [CancellationTokenSource.IsCancellationRequested](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.iscancellationrequested#system-threading-cancellationtokensource-iscancellationrequested) flag has been set, landing us in the `catch (OperationCanceledException) { }` block we had wrapped this around earlier.

Now, all that's left is to tell the <xref:DreamCheekyLED.USBInterface> to <xref:DreamCheekyLED.USBInterface.Stop> updating the display, which usually takes about a quarter of a second to fully time out. The new RunLoop() should look something like this:

[!code-csharp[](example_consoleapp_runloop_after.cs)]

Combine that with our earlier `Main()` method call, and it should look like this:

[!code-csharp[](example_consoleapp_full_multithread.cs)]

You'll notice this is *exactly* what the DreamCheekyPathLoaderApp looks like :)

## Using USBInterface in a Single Thread

If all you're doing is sending a static display off to the Dream Cheeky LED message board, you could simplify the code *much* further and even one-line a static image to negate the need for multithreading *entirely*. After all, the <xref:DreamCheekyLED.USBInterface> *itself* is multithreaded, so we don't really *need* to create a whole new processing thread for it (even if you now know how). If you still do this in a loop, you can check to see if the <xref:DreamCheekyLED.USBInterface.IsRunning> flag is `false`, and if so, tell it to run with the <xref:DreamCheekyLED.USBInterface.Run> method. Something like this:

[!code-csharp[](example_singlethread_innerloop.cs)]

Without the animations, all the multitasking, the code cuts down ***significantly*** to only the following, with most of the bloat coming from the file path checking itself:

[!code-csharp[](example_consoleapp_full_singlethread.cs)]

And that's really it, the simplest example of how to send a single static image to a DreamCheekyLED USB display. Obviously this does not support animated GIF images, so the `DreamCheekyPathLoaderApp` sticks with the multithreaded approach above to loop through the frames, but for plenty of use cases, this should be sufficient. Whether you load images from dynamic file paths or hard-code the screens you want, this guide should have ***everything*** you've needed to use this library. If ever you need a quick reference on how to use this library, now that you know how it works, check out the [Quick Start Guide](quickstart.md) for easy-to-use, copyable code examples.