// Create IEnumerable<bool> to store individual LED data
List<bool> lstRow1 = [];
List<bool> lstRow2 = [];
List<bool> lstRow3 = [];
List<bool> lstRow4 = [];
List<bool> lstRow5 = [];
List<bool> lstRow6 = [];
List<bool> lstRow7 = [];

// Fill above data here

// Create IEnumerable<bool> to store individual LED data for the whole screen
List<bool> lstScreen =
[
	.. lstRow1,
	.. lstRow2,
	.. lstRow3,
	.. lstRow4,
	.. lstRow5,
	.. lstRow6,
	.. lstRow7,
];

ScreenData sd = new ScreenData(LEDArray.Brightness.Max, lstScreen);

using USBInterface u = USBInterface.TryOpen(out bool openResult);
{
	if (openResult)
	{
		// DreamCheekyLED successfully connected to
		u.SetFrameBuffer(sd.MakeFrameBuffer());
		u.Run();

		// Do more stuff here!
		Thread.Sleep(750); // Display it for 3/4 of a second, letting the device's 1/4 of a second cache round out to about a second

		u.Stop();
	}
}