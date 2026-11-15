// Create IEnumerable<bool> to store individual LED data
List<bool> lstRow1 = [];
List<bool> lstRow2 = [];
List<bool> lstRow3 = [];
List<bool> lstRow4 = [];
List<bool> lstRow5 = [];
List<bool> lstRow6 = [];
List<bool> lstRow7 = [];

// Fill above data here

// Create IEnumerable<byte> to store individual row byte data
List<byte> lstRowBytes1 = LEDArray.BytesFromBools(lstRow1).ToList();
List<byte> lstRowBytes2 = LEDArray.BytesFromBools(lstRow2).ToList();
List<byte> lstRowBytes3 = LEDArray.BytesFromBools(lstRow3).ToList();
List<byte> lstRowBytes4 = LEDArray.BytesFromBools(lstRow4).ToList();
List<byte> lstRowBytes5 = LEDArray.BytesFromBools(lstRow5).ToList();
List<byte> lstRowBytes6 = LEDArray.BytesFromBools(lstRow6).ToList();
List<byte> lstRowBytes7 = LEDArray.BytesFromBools(lstRow7).ToList();

byte brightness = LEDArray.Brightness.Max; // Or other setting

List<byte> lstFrameBufferRows12 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows12, lstRowBytes1, lstRowBytes2).ToList();
List<byte> lstFrameBufferRows34 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows34, lstRowBytes3, lstRowBytes4).ToList();
List<byte> lstFrameBufferRows56 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows56, lstRowBytes5, lstRowBytes6).ToList();
List<byte> lstFrameBufferRow7 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Row7, lstRowBytes7).ToList();

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