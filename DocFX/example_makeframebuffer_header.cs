byte brightness = LEDArray.Brightness.Max; // Or other setting

List<byte> lstFrameBufferRows12 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows12, lstRowBytes1, lstRowBytes2).ToList();
List<byte> lstFrameBufferRows34 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows34, lstRowBytes3, lstRowBytes4).ToList();
List<byte> lstFrameBufferRows56 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Rows56, lstRowBytes5, lstRowBytes6).ToList();
List<byte> lstFrameBufferRow7 = LEDArray.MakeFrameBuffer(brightness, LEDArray.RowGroups.Row7, lstRowBytes7).ToList();

// Optional: merge above frame buffers into a "per screen" frame buffer to cache for later:
List<byte> lstFrameBufferScreen =
[
	.. lstFrameBufferRows12,
	.. lstFrameBufferRows34,
	.. lstFrameBufferRows56,
	.. lstFrameBufferRow7,
];