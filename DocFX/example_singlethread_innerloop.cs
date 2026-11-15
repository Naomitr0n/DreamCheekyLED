u.SetFrameBuffer(ImageLoader.StaticScreenDataFromPath(filePathInput, autoSetBrightness: true).MakeFrameBuffer());

if (!u.IsRunning)
{
	u.Run();
}