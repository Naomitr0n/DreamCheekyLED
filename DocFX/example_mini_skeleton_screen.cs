using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		// DreamCheekyLED successfully connected to
		u.SetFrameBuffer(lstFrameBufferScreen);
		u.Run();

		// Do more stuff here!

		u.Stop();
	}
}