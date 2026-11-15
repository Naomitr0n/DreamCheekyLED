using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		// DreamCheekyLED successfully connected to
		u.SetFrameBuffer(lstFrameBufferRows12, lstFrameBufferRows34, lstFrameBufferRows56, lstFrameBufferRow7);
		u.Run();

		// Do more stuff here!
		Thread.Sleep(750); // Display it for 3/4 of a second, letting the device's 1/4 of a second cache round out to about a second

		u.Stop();
	}
}