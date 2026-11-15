using SkiaSharp;
using static DreamCheekyLED.LEDArray;

#pragma warning disable IDE0090 // Use 'new(...)' -- This just saves a few characters for a format that I personally don't like and find really vague. Ignore this one.
#pragma warning disable IDE0130 // Namespace does not match folder structure -- This is intended behavior

namespace DreamCheekyLED
{
    /// <summary>
    /// Loads either a static or animated image and returns frame data to send to <see cref="USBInterface"/>
    /// </summary>
    public static class ImageLoader
    {
        #region ImageLoader - Helper Classes
        /// <summary>
        /// Helper class for an individual frame of an animated image, or just the data to a single static image
        /// </summary>
        /// <param name="delayAfter">Amount of ms to delay after this frame is displayed, before displaying the next one</param>
        /// <param name="screenData"><seealso cref="ScreenData"/> for the current frame</param>
        public class FrameData(int delayAfter, ScreenData screenData)
        {
            #region FrameData - Properties
            /// <summary>
            /// Amount of ms to delay after this frame is displayed, before displaying the one after
            /// </summary>
            public int DelayAfter { get; set; } = delayAfter;
            /// <summary>
            /// <seealso cref="ScreenData"/> for the current frame
            /// </summary>
            public ScreenData ScreenData { get; set; } = screenData;
            #endregion

            #region FrameData - Constructors
            /// <summary>
            /// Constructor that fills delay with zero and leaves ScreenData unfilled
            /// </summary>
            public FrameData()
                : this(0) { }
            /// <summary>
            /// Constructor that leaves ScreenData unfilled
            /// </summary>
            /// <param name="delayAfter">Amount of ms to delay after this frame is displayed, before displaying the one after</param>
            public FrameData(int delayAfter)
                : this(delayAfter, new ScreenData()) { }

            #endregion
        }
        #endregion

        #region ImageLoader - Parse Image Data
        /// <summary>
        /// Reads a static image (usch as GIF, PNG, BMP) and extracts it as a frame buffer. <b>Any</b> pixel that is not either fully transparent (alpha
        /// 0), or fully black (RGB #000000) will be illuminated. If <paramref name="autoSetBrightness"/> is set to true (or left unspecified), the first
        /// non-transparent, non-black pixel (when read left-to-right, then top-to-bottom) will set the brightness of the whole screen automatically.
        /// </summary>
        /// <param name="pathToImage">File path to static image to use for frame buffer</param>
        /// <param name="autoSetBrightness">If true, will automatically use the HSV value from the first non-black, non-transparent pixel (read left-to-right, then top-to-bottom)
        /// to set the <see cref="LEDArray.Brightness"/> value: V values of 75% or higher are set to max, 50% to 74.9% set to average, lower than 50% set to dim. False disables this behavior.</param>
        /// <returns><see cref="ScreenData"/> of the extracted image</returns>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="pathToImage"/> cannot be found</exception>
        /// <exception cref="IOException">Thrown if <see cref="SKCodec.Create(string, out SKCodecResult)">SKCodec.Create(string, out SKCodecResult)</see> is not successful</exception>
        public static ScreenData StaticScreenDataFromPath(string pathToImage, bool autoSetBrightness = true)
        {
            if (!File.Exists(pathToImage))
            {
                throw new FileNotFoundException(message: "no file found at specified path", fileName: pathToImage);
            }

            using SKCodec codec = SKCodec.Create(pathToImage, out SKCodecResult parseResult);
            if (parseResult != SKCodecResult.Success)
            {
                throw new IOException(message: $"Error in reading file \"{pathToImage}\"");
            }

            ScreenData output = new ScreenData();
            int scaleMultiplier = 1;
            SKColor? firstColorPixel = null;
            int scaleEdgeToCenter = 0;

            if (codec.Info.Width > 21 && codec.Info.Height > 7)
            {
                int scaleWidth = (codec.Info.Width / 21);
                int scaleHeight = scaleMultiplier = (codec.Info.Height / 7);
                if (scaleHeight > scaleWidth) { scaleMultiplier = scaleWidth; }
                scaleEdgeToCenter = ((scaleMultiplier - 1) / 2);
            }

            SKBitmap b = SKBitmap.Decode(pathToImage);

            for (int y = 0; (y < codec.Info.Height && y < 7); y++)
            {
                bool[] row = new bool[21];
                int scaledY = (y * scaleMultiplier) + scaleEdgeToCenter;

                for (int x = 0; (x < codec.Info.Width && x < 21); x++)
                {
                    int scaledX = (x * scaleMultiplier) + scaleEdgeToCenter;
                    SKColor c = b.GetPixel(scaledX, scaledY);

                    if (c.Alpha == 255 && c != SKColors.Empty && c != SKColors.Black)
                    {
                        if (firstColorPixel == null)
                        {
                            firstColorPixel = c;
                        }

                        row[x] = true;
                    }
                }

                output.FillRowFromBools(y + 1, row);

                byte brightness = Brightness.Max;

                if (autoSetBrightness && firstColorPixel.HasValue)
                {
                    firstColorPixel.Value.ToHsv(out _, out _, out float v);

                    if (v < 75.0)
                    {
                        brightness = (v < 50.0 ? Brightness.Dim : Brightness.Medium);
                    }
                }

                output.Brightness = brightness;
            }

            return output;
        }

        /// <summary>
        /// Reads an animated image (usch as GIF) and extracts each frame as a frame buffer. <b>Any</b> pixel that is not either fully transparent (alpha
        /// 0), or fully black (RGB #000000) will be illuminated. If <paramref name="autoSetBrightness"/> is set to true (or left unspecified), the first
        /// non-transparent, non-black pixel (when read left-to-right, then top-to-bottom) will set the brightness of the screen for that particular frame.
        /// </summary>
        /// <param name="pathToImage">File path to animated image to use for frame buffer</param>
        /// <param name="autoSetBrightness">If true, will automatically use the HSV value from the first non-black, non-transparent pixel (read left-to-right, then top-to-bottom) per frame
        /// to set the <see cref="LEDArray.Brightness"/> value: V values of 75% or higher are set to max, 50% to 74.9% set to average, lower than 50% set to dim. False disables this behavior.</param>
        /// <returns><see cref="IEnumerable{FrameData}">IEnumerable&lt;ImageLoader.FrameData&gt;</see> of the extracted frames</returns>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="pathToImage"/> cannot be found</exception>
        /// <exception cref="IOException">Thrown if <see cref="SKCodec.Create(string, out SKCodecResult)">SKCodec.Create(string, out SKCodecResult)</see> is not successful</exception>
        public static IEnumerable<FrameData> AnimatedScreenDataFromPath(string pathToImage, bool autoSetBrightness = true)
        {
            if (!File.Exists(pathToImage))
            {
                throw new FileNotFoundException(message: "no file found at specified path", fileName: pathToImage);
            }

            using SKCodec codec = SKCodec.Create(pathToImage, out SKCodecResult parseResult);
            if (parseResult != SKCodecResult.Success)
            {
                throw new IOException(message: $"Error in reading file \"{pathToImage}\"");
            }

            SKBitmap[] bitmaps = new SKBitmap[codec.FrameCount];
            IEnumerable<FrameData> output = [];
            int scaleMultiplier = 1;
            int scaleEdgeToCenter = 0;

            if (codec.Info.Width > 21 && codec.Info.Height > 7)
            {
                int scaleWidth = (codec.Info.Width / 21);
                int scaleHeight = scaleMultiplier = (codec.Info.Height / 7);
                if (scaleHeight > scaleWidth) { scaleMultiplier = scaleWidth; }
                scaleEdgeToCenter = ((scaleMultiplier - 1) / 2);
            }

            for (int i = 0; i < codec.FrameCount; i++)
            {
                FrameData fd = new FrameData(codec.FrameInfo[i].Duration);
                SKImageInfo imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                
                bitmaps[i] = new SKBitmap(imageInfo);

                IntPtr pointer = bitmaps[i].GetPixels();
                SKCodecOptions codecOptions = new SKCodecOptions(i);
                SKColor? firstColorPixel = null;

                codec.GetPixels(imageInfo, pointer, codecOptions);

                for (int y = 0; (y < codec.Info.Height && y < 7); y++)
                {
                    bool[] row = new bool[21];
                    int scaledY = (y * scaleMultiplier) + scaleEdgeToCenter;

                    for (int x = 0; (x < codec.Info.Width && x < 21); x++)
                    {
                        int scaledX = (x * scaleMultiplier) + scaleEdgeToCenter;
                        SKColor c = bitmaps[i].GetPixel(scaledX, scaledY);

                        if (c.Alpha == 255 && c != SKColors.Empty && c != SKColors.Black)
                        {
                            if (firstColorPixel == null)
                            {
                                firstColorPixel = c;
                            }

                            row[x] = true;
                        }
                    }

                    fd.ScreenData.FillRowFromBools(y + 1, row);
                }

                byte brightness = Brightness.Max;

                if (autoSetBrightness && firstColorPixel.HasValue)
                {
                    firstColorPixel.Value.ToHsv(out _, out _, out float v);

                    if (v < 75.0)
                    {
                        brightness = (v < 50.0 ? Brightness.Dim : Brightness.Medium);
                    }
                }

                fd.ScreenData.Brightness = brightness;

                output = output.Append(fd);
            }

            return output;
        }
        #endregion
    }
}