do
{
	foreach (FrameData fd in frames)
	{
		u.SetFrameBuffer(fd.ScreenData.MakeFrameBuffer());
		await Task.Delay(fd.DelayAfter, cancellationToken);
	}
} while (!cancellationToken.IsCancellationRequested);