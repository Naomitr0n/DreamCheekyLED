#pragma warning disable IDE0305 // Simplify collection initialization -- Honestly I just hate how this looks

namespace DreamCheekyLED
{
    /// <summary>
    /// Various methods and helper classes for LED array data
    /// </summary>
    public static class LEDArray
    {
        #region Helper Classes
        /// <summary>
        /// Brightness levels. Use this for <c>Brightness</c> param of <see cref="MakeFrameBuffer(byte, byte, IEnumerable{byte}, IEnumerable{byte})"/>
        /// </summary>
        public static class Brightness
        {
            /// <summary>
            /// Dimmest
            /// </summary>
            public const byte Dim = 0;
            /// <summary>
            /// Medium brightness
            /// </summary>
            public const byte Medium = 1;
            /// <summary>
            /// Brightest
            /// </summary>
            public const byte Max = 2;
        }

        /// <summary>
        /// Row section designator. Use this for <c>RowGroup</c> param of <see cref="MakeFrameBuffer(byte, byte, IEnumerable{byte}, IEnumerable{byte})"/>
        /// </summary>
        public static class RowGroups
        {
            /// <summary>
            /// Identifier for rows 1 and 2
            /// </summary>
            public const byte Rows12 = 0;
            /// <summary>
            /// Identifier for rows 3 and 4
            /// </summary>
            public const byte Rows34 = 2;
            /// <summary>
            /// Identifier for rows 5 and 6
            /// </summary>
            public const byte Rows56 = 4;
            /// <summary>
            /// Identifier for row 7 (row 8 is ignored)
            /// </summary>
            public const byte Row7 = 6;
        }

        /// <summary>
        /// Only exists for quick testing
        /// </summary>
        public static class Data
        {
            /// <summary>
            /// Row with every LED turned off
            /// </summary>
            public static readonly byte[] EmptyRow = [0xFF, 0xFF, 0xFF];
            /// <summary>
            /// Row with every LED fully illuminated
            /// </summary>
            public static readonly byte[] FullRow = [0x00, 0x00, 0x00];
            /// <summary>
            /// Alias for <see cref="EmptyRow"/>. There are only seven rows, Row 8 data is ignored completely on the device side and could be anything however.
            /// </summary>
            public static readonly byte[] Row8 = [0xFF, 0xFF, 0xFF];
        }
        #endregion

        #region LEDArray - Methods
        /// <summary>
        /// Creates a <see cref="byte"/> array from a <see cref="bool"/> array for use with <see cref="MakeFrameBuffer"/>.
        /// <br /><br />
        /// <paramref name="input"/> <b>MUST</b> be 21 elements long.
        /// </summary>
        /// <param name="input">Array of exactly 21 boolean values</param>
        /// <returns><see cref="byte"/> array of a single row section of the frame buffer</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="input"/> is a collection of booleans of a length other than 21</exception>
        public static byte[] ByteArrayFromBools(params bool[] input) => BytesFromBools(input).ToArray();

        /// <summary>
        /// Creates an <see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> from an <see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for use with <see cref="MakeFrameBuffer"/>.
        /// <br /><br />
        /// <paramref name="input"/> <b>MUST</b> be 21 elements long.
        /// </summary>
        /// <param name="input"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> of exactly 21 boolean values</param>
        /// <returns><see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> with 21 elements in it</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="input"/> is a collection of booleans of a length other than 21</exception>
        public static IEnumerable<byte> BytesFromBools(params IEnumerable<bool> input)
        {
            if (input.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(input), message: $"input array MUST be 21 LED's long. Current length is {input.Count()}");
            }

            input = [.. input, false, false, false];
            input = [.. input.Reverse()];

            byte[] returnArray = new byte[3];

            for (int iByteChunk = 2; iByteChunk >= 0; iByteChunk--)
            {
                int currByte = 0;

                for (int iBit = 0; iBit < 8; iBit++)
                {
                    int index = (iByteChunk * 8) + iBit;
                    bool val = false;

                    if (index < input.Count()) { val = !input.ElementAt(index); }

                    if (val)
                    {
                        currByte += 1 << (7 - iBit);
                    }
                }

                returnArray[iByteChunk] = (byte)currByte;
            }

            return returnArray;
        }

        /// <summary>
        /// Makes a Frame buffer to send to the USB LED message board.
        /// </summary>
        /// <param name="Brightness">How bright the display will be. See <see cref="Brightness"/></param>
        /// <param name="RowGroup">Which rows to make the frame buffer for. See <see cref="RowGroups"/></param>
        /// <param name="UpperRow">Data for upper row of each grouping. See also <seealso cref="ByteArrayFromBools(bool[])"/> and <seealso cref="BytesFromBools(IEnumerable{bool})"/>.</param>
        /// <param name="LowerRow">Data for lower row of each grouping. Ignored by LED Message Board for row 7. If not supplied, will be empty.
        /// See also <seealso cref="ByteArrayFromBools(bool[])"/> and <seealso cref="BytesFromBools(IEnumerable{bool})"/>.</param>
        /// <returns>Array of bytes to send to the USB LED message board.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Brightness or RowGroup values are invalid, or if UpperRow has a length other than 3 (or if LowerRow has a length other than 3 if supplied)</exception>
        public static byte[] MakeFrameBufferArray(byte Brightness, byte RowGroup, byte[] UpperRow, byte[]? LowerRow = null) => MakeFrameBuffer(Brightness, RowGroup, UpperRow, LowerRow).ToArray();

        /// <summary>
        /// Makes a frame buffer to send to the USB LED message board.
        /// </summary>
        /// <param name="Brightness">How bright the display will be. See <see cref="Brightness"/></param>
        /// <param name="RowGroup">Which rows to make the frame buffer for. See <see cref="RowGroups"/></param>
        /// <param name="UpperRow">Data for upper row of each grouping. See also <seealso cref="ByteArrayFromBools(bool[])"/> and <seealso cref="BytesFromBools(IEnumerable{bool})"/>.</param>
        /// <param name="LowerRow">Data for lower row of each grouping. Ignored by LED Message Board for row 7. If not supplied, will be empty.
        /// See also <seealso cref="ByteArrayFromBools(bool[])"/> and <seealso cref="BytesFromBools(IEnumerable{bool})"/>.</param>
        /// <returns><see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> to send to the USB LED message board.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Brightness or RowGroup values are invalid, or if UpperRow has a length other than 3, or if LowerRow has a length other than 3 (if it was supplied)</exception>
        public static IEnumerable<byte> MakeFrameBuffer(byte Brightness, byte RowGroup, IEnumerable<byte> UpperRow, IEnumerable<byte>? LowerRow = null)
        {
            if (Brightness < 0 || Brightness > 2)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Brightness), message: $"Brightness value was {Brightness:0}, but expected 0 - 2");
            }

            if (!(RowGroup == 0 || RowGroup == 2 || RowGroup == 4 || RowGroup == 6))
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(RowGroup), message: $"RowGroup value was {RowGroup:0}, but expected 0, 2, 4, or 6 (use LEDArray.Rows for this)");
            }

            if (UpperRow.Count() != 3)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(UpperRow), message: $"UpperRow was {UpperRow.Count()} bytes long. Row data MUST be exactly three bytes long (use BytesFromBools for this)");
            }

            IEnumerable<byte> lowerRowParsed = LowerRow ?? [0xFF, 0xFF, 0xFF];

            if (lowerRowParsed.Count() != 3)
            {
                if (RowGroup == RowGroups.Row7)
                {
                    //We really shouldn't do super-extra cleanup like this, but...
                    lowerRowParsed = Data.Row8;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(LowerRow), message: $"LowerRow was {lowerRowParsed.Count()} bytes long. Row data MUST be exactly three bytes long (use BytesFromBools for this)");
                }
            }

            // The v1.0 documentation omits the requirement for the first byte of 0xFF. I don't know if it serves a purpose, but 0xFF has worked for everything I've thrown at it.
            // Possible TODO for a fork if someone figures it out.
            return [0xFF, Brightness, RowGroup, ..UpperRow, ..lowerRowParsed];
        }
        #endregion
    }
}