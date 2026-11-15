// Set the path to the image:
string filePathInput = @"extended_test_10x_input.gif";

using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		if (Path.Exists(filePathInput))
		{
			IEnumerable<FrameData> frames = ImageLoader.AnimatedScreenDataFromPath(filePathInput);

			// Set first frame
			u.SetFrameBuffer(frames.First().ScreenData.MakeFrameBuffer());
			u.Run();

			// Loop through every frame
			foreach (FrameData fd in frames)
			{
				u.SetFrameBuffer(fd.ScreenData.MakeFrameBuffer());
			}

			u.Stop();
		}
	}
}
