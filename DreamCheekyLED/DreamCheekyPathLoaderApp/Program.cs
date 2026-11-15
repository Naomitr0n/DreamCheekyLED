using DreamCheekyLED;
using FrameData = DreamCheekyLED.ImageLoader.FrameData;

#pragma warning disable IDE0090 // Use 'new(...)' -- This just saves a few characters for a format that I personally don't like and find really vague. Ignore this one.

internal class Program
{
    private static Task? _task = null;
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    private static void Main(string[] args)
    {        
        string filePathInput = "";

        // This is done in case a path that includes spaces is supplied, without being surrounded by quotes
        filePathInput = string.Join(' ', args.Prepend(filePathInput)).Trim();

        using USBInterface u = USBInterface.Open();
        {
            do
            {
                //Ignore blank input and repeat prompt
                if (!String.IsNullOrEmpty(filePathInput))
                {
                    //If there is a quoted section at the start of the input, isolate it and ignore the rest
                    if (filePathInput.StartsWith('"') && filePathInput[1..].Contains('"'))
                    {
                        filePathInput = filePathInput[1..];
                        filePathInput = filePathInput[..filePathInput.IndexOf('"')];
                    }

                    if (!Path.Exists(filePathInput))
                    {
                        Console.WriteLine($"ERROR! File not found: \"{filePathInput}\"");
                    }
                    else
                    {
                        try
                        {
                            IEnumerable<FrameData> frames = ImageLoader.AnimatedScreenDataFromPath(filePathInput);

                            //Loaded frames successfully (no exception thrown), so cancel the RunLoop(), then start a new image update loop
                            if (_task != null)
                            {
                                _cts.Cancel();
                                _task.Wait();
                            }

                            _cts = new CancellationTokenSource();
                            _task = RunLoop(frames, u, _cts.Token);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR! {ex.Message}"); ;
                        }
                    }
                }

                Console.Write("Enter file path to display (or 'Q' to quit): ");
                filePathInput = (Console.ReadLine() ?? "").Trim();
            } while (!filePathInput.Equals("Q", StringComparison.CurrentCultureIgnoreCase));

            u.Stop();
            _cts.Cancel();
            _task?.Wait();
        }
    }

    private static async Task RunLoop(IEnumerable<FrameData> frames, USBInterface u, CancellationToken cancellationToken)
    {
        try
        {
            u.SetFrameBuffer(frames.First().ScreenData.MakeFrameBuffer());
            u.Run();

            do
            {
                foreach (FrameData fd in frames)
                {
                    u.SetFrameBuffer(fd.ScreenData.MakeFrameBuffer());

                    if (fd.DelayAfter > 0)
                    {
                        await Task.Delay(fd.DelayAfter, cancellationToken);
                    }
                    else
                    {
                        // Task.Delay(0) is synchronous for some reason, so use Task.Yield() for delays of zero
                        await Task.Yield();
                    }
                }
            } while (!cancellationToken.IsCancellationRequested);
        }
        catch (OperationCanceledException) { } // Thrown by Task.Delay(), which monitors IsCancellationRequested flag
        finally { u.Stop(); }
    }
}