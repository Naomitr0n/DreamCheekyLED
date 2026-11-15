using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		// DreamCheekyLED successfully connected to
		u.SetFrameBuffer(lstFrameBufferRows12, lstFrameBufferRows34, lstFrameBufferRows56, lstFrameBufferRow7);
		u.Run();

		// Do more stuff here!

		u.Stop();
	}
}