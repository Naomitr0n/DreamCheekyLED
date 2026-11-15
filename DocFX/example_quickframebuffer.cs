string filePathInput = @"C:\Path\To\Image.png";

ScreenData screenData = ImageLoader.StaticScreenDataFromPath(filePathInput, autoSetBrightness: true);
byte[] screenBytes = screenData.MakeFrameBufferArray();

// screenData can be serialized or otherwise stored if desired
// screenBytes can be saved directly to a file, cached, hard-coded using 0xFF format, etc.

using USBInterface u = USBInterface.Open();
{
	u.SetFrameBuffer(screenBytes);
	u.Run();

	// Do more stuff here!

	Thread.Sleep(750); // Display it for 3/4 of a second, letting the device's 1/4 of a second cache round out to about a second
	u.Stop();
}