using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		// DreamCheekyLED successfully connected to
	}
	else
	{
		// DreamCheekyLED connection did not work
	}
}