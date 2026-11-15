IEnumerable<bool> ledStates = [];

// Populate ledStates with (21 x 7 = 147) bool values

using (USBInterface u = USBInterface.TryOpen(out bool openResult))
{
	ScreenData sd = new ScreenData(ledStates);
	byte[] frameBuffer = sd.MakeFrameBufferArray();
	u.SetFrameBuffer(frameBuffer);
	u.Run();

	// Do more stuff here!

	u.Stop();
}