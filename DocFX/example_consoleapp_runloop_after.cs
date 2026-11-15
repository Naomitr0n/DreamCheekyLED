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
				await Task.Delay(fd.DelayAfter, cancellationToken);
			}
		} while (!cancellationToken.IsCancellationRequested);
	}
	catch (OperationCanceledException) { } // Thrown by Task.Delay(), which monitors IsCancellationRequested flag
	finally
	{
		u.Stop();
	}
}