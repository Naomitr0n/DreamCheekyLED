#pragma warning disable IDE0060 // Remove unused parameter -- These are all related to the phantom row 8
#pragma warning disable IDE0305 // Simplify collection initialization -- Honestly I just hate how this looks

namespace DreamCheekyLED
{
    /// <summary>
    /// Helper class for storing screen data to send to <see cref="USBInterface"/>
    /// </summary>
    public class ScreenData
    {
        private byte _brightness;
        private byte[] _row1;
        private byte[] _row2;
        private byte[] _row3;
        private byte[] _row4;
        private byte[] _row5;
        private byte[] _row6;
        private byte[] _row7;

        #region ScreenData - Properties
        /// <summary>
        /// Brightness value for screen
        /// </summary>
        public byte Brightness
        {
            get => _brightness;
            set
            {
                if (Brightness < 0 || Brightness > 2)
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(Brightness), message: $"Brightness value was {Brightness:0}, but expected 0 - 2");
                }

                _brightness = value;
            }
        }

        /// <summary>
        /// Byte data for row 1
        /// </summary>
        public byte[] Row1
        {
            get => _row1.ToArray();
            set => value.CopyTo(_row1);
        }

        /// <summary>
        /// Byte data for row 2
        /// </summary>
        public byte[] Row2
        {
            get => _row2.ToArray();
            set => value.CopyTo(_row2);
        }

        /// <summary>
        /// Byte data for row 3
        /// </summary>
        public byte[] Row3
        {
            get => _row3.ToArray();
            set => value.CopyTo(_row3);
        }

        /// <summary>
        /// Byte data for row 4
        /// </summary>
        public byte[] Row4
        {
            get => _row4.ToArray();
            set => value.CopyTo(_row4);
        }

        /// <summary>
        /// Byte data for row 5
        /// </summary>
        public byte[] Row5
        {
            get => _row5.ToArray();
            set => value.CopyTo(_row5);
        }

        /// <summary>
        /// Byte data for row 6
        /// </summary>
        public byte[] Row6
        {
            get => _row6.ToArray();
            set => value.CopyTo(_row6);
        }

        /// <summary>
        /// Byte data for row 7
        /// </summary>
        public byte[] Row7
        {
            get => _row7.ToArray();
            set => value.CopyTo(_row7);
        }
        #endregion

        #region ScreenData - Constructors
        /// <summary>
        /// Constructor for ScreenData class. Sets brightness to maximum with every LED turned off.
        /// </summary>
        public ScreenData()
        {
            _brightness = LEDArray.Brightness.Max;
            _row1 = [0xFF, 0xFF, 0xFF];
            _row2 = [0xFF, 0xFF, 0xFF];
            _row3 = [0xFF, 0xFF, 0xFF];
            _row4 = [0xFF, 0xFF, 0xFF];
            _row5 = [0xFF, 0xFF, 0xFF];
            _row6 = [0xFF, 0xFF, 0xFF];
            _row7 = [0xFF, 0xFF, 0xFF];
        }

        /// <summary>
        /// Constructor for ScreenData class that accepts brightness only. Every LED will be turned off by edf
        /// </summary>
        /// <param name="brightness">Brightness value, from <see cref="LEDArray.Brightness"/></param>
        public ScreenData(byte brightness)
            : this() => _brightness = brightness;

        /// <summary>
        /// Constructor for ScreenData class that accepts (21 x 7 = 147) individual LED data, read left-to-right, then top-to-bottom. Sets brightness to maximum.
        /// </summary>
        /// <param name="LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for each LED on the screen, read left-to-right, then top-to-bottom</param>
        public ScreenData(params IEnumerable<bool> LEDData)
            : this() => FillFromBools(LEDData);

        /// <summary>
        /// Constructor for ScreenData class that accepts individual row data, with 21 values each. Sets brightness to maximum.
        /// </summary>
        /// <param name="Row1LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 1, having 21 elements</param>
        /// <param name="Row2LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 2, having 21 elements</param>
        /// <param name="Row3LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 3, having 21 elements</param>
        /// <param name="Row4LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 4, having 21 elements</param>
        /// <param name="Row5LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 5, having 21 elements</param>
        /// <param name="Row6LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 6, having 21 elements</param>
        /// <param name="Row7LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 7, having 21 elements</param>
        /// <param name="Row8LEDData">Ignored</param>
        public ScreenData(IEnumerable<bool> Row1LEDData, IEnumerable<bool> Row2LEDData, IEnumerable<bool> Row3LEDData, IEnumerable<bool> Row4LEDData,
            IEnumerable<bool> Row5LEDData, IEnumerable<bool> Row6LEDData, IEnumerable<bool> Row7LEDData, IEnumerable<bool>? Row8LEDData = null)
            : this() => FillFromBools(Row1LEDData, Row2LEDData, Row3LEDData, Row4LEDData, Row5LEDData, Row6LEDData, Row7LEDData);

        /// <summary>
        /// Constructor for ScreenData class that accepts (21 x 7 = 147) individual LED data, read left-to-right, then top-to-bottom.
        /// </summary>
        /// <param name="brightness">Brightness value, from <see cref="LEDArray.Brightness"/></param>
        /// <param name="LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for each LED on the screen, read left-to-right, then top-to-bottom</param>
        public ScreenData(byte brightness, params IEnumerable<bool> LEDData)
            : this(LEDData) => _brightness = brightness;

        /// <summary>
        /// Constructor for ScreenData class that accepts individual row data, with 21 values each.
        /// </summary>
        /// <param name="brightness">Brightness value, from <see cref="LEDArray.Brightness"/></param>
        /// <param name="Row1LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 1, having 21 elements</param>
        /// <param name="Row2LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 2, having 21 elements</param>
        /// <param name="Row3LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 3, having 21 elements</param>
        /// <param name="Row4LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 4, having 21 elements</param>
        /// <param name="Row5LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 5, having 21 elements</param>
        /// <param name="Row6LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 6, having 21 elements</param>
        /// <param name="Row7LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 7, having 21 elements</param>
        /// <param name="Row8LEDData">Ignored</param>
        public ScreenData(byte brightness, IEnumerable<bool> Row1LEDData, IEnumerable<bool> Row2LEDData, IEnumerable<bool> Row3LEDData, IEnumerable<bool> Row4LEDData,
            IEnumerable<bool> Row5LEDData, IEnumerable<bool> Row6LEDData, IEnumerable<bool> Row7LEDData, IEnumerable<bool>? Row8LEDData = null)
            : this(Row1LEDData, Row2LEDData, Row3LEDData, Row4LEDData, Row5LEDData, Row6LEDData, Row7LEDData) => _brightness = brightness;

        /// <summary>
        /// Constructor for ScreenData class that accepts row pair frame buffer data. Sets brightness to maximum. Not recommended, but here if you need it.
        /// </summary>
        /// <param name="Rows12FrameBuffer">Frame buffer for Rows 1 and 2 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Rows34FrameBuffer">Frame buffer for Rows 3 and 4 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Rows56FrameBuffer">Frame buffer for Rows 5 and 6 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Row7FrameBuffer">Frame buffer for Row 7 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the frame buffers have lengths other than 9</exception>
        public ScreenData(IEnumerable<byte> Rows12FrameBuffer, IEnumerable<byte> Rows34FrameBuffer, IEnumerable<byte> Rows56FrameBuffer, IEnumerable<byte> Row7FrameBuffer)
        {
            if (Rows12FrameBuffer.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows12FrameBuffer), message: $"Rows12 array MUST have exactly 9 items in it. Current length is {Rows12FrameBuffer.Count()}");
            }
            if (Rows34FrameBuffer.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows34FrameBuffer), message: $"Rows34 array MUST have exactly 9 items in it. Current length is {Rows34FrameBuffer.Count()}");
            }
            if (Rows56FrameBuffer.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows56FrameBuffer), message: $"Rows56 array MUST have exactly 9 items in it. Current length is {Rows56FrameBuffer.Count()}");
            }
            if (Row7FrameBuffer.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row7FrameBuffer), message: $"Row7 array MUST have exactly 9 items in it. Current length is {Row7FrameBuffer.Count()}");
            }

            _row1 = Rows12FrameBuffer.Skip(3).Take(3).ToArray();
            _row2 = Rows12FrameBuffer.Skip(6).Take(3).ToArray();
            _row3 = Rows34FrameBuffer.Skip(3).Take(3).ToArray();
            _row4 = Rows34FrameBuffer.Skip(6).Take(3).ToArray();
            _row5 = Rows56FrameBuffer.Skip(3).Take(3).ToArray();
            _row6 = Rows56FrameBuffer.Skip(6).Take(3).ToArray();
            _row7 = Row7FrameBuffer.Skip(3).Take(3).ToArray();
        }

        /// <summary>
        /// Constructor for ScreenData class that accepts brightness and row pair frame buffer data
        /// </summary>
        /// <param name="brightness">Brightness value, from <see cref="LEDArray.Brightness"/></param>
        /// <param name="Rows12FrameBuffer">Frame buffer for Rows 1 and 2 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Rows34FrameBuffer">Frame buffer for Rows 3 and 4 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Rows56FrameBuffer">Frame buffer for Rows 5 and 6 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <param name="Row7FrameBuffer">Frame buffer for Row 7 (see <seealso cref="MakeFrameBuffer(byte?)"/>)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the frame buffers have lengths other than 9</exception>
        public ScreenData(byte brightness, IEnumerable<byte> Rows12FrameBuffer, IEnumerable<byte> Rows34FrameBuffer, IEnumerable<byte> Rows56FrameBuffer, IEnumerable<byte> Row7FrameBuffer)
            : this(Rows12FrameBuffer, Rows34FrameBuffer, Rows56FrameBuffer, Row7FrameBuffer) => _brightness = brightness;
        #endregion

        #region ScreenData - Methods
        /// <summary>
        /// Fills a single row for the current ScreenData object with the correct frame buffer from the provided boolean values.
        /// Does NOT change screen data on the USB device itself (use <seealso cref="USBInterface"/> for this).
        /// </summary>
        /// <param name="RowNum">Which row of the screen to fill in, from 1 to 7</param>
        /// <param name="LEDData">Array of exactly 21 boolean values, read left-to-right</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="LEDData"/> is a collection of booleans of a length other than 21. Also thrown if <paramref name="RowNum"/> is not a valid row number from 1 to 7</exception>
        public void FillRowFromBools(int RowNum, params IEnumerable<bool> LEDData)
        {
            if (LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(LEDData), message: $"input array MUST be 21 LED's long. Current length is {LEDData.Count()}");
            }

            switch (RowNum)
            {
                case 1:
                    _row1 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 2:
                    _row2 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 3:
                    _row3 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 4:
                    _row4 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 5:
                    _row5 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 6:
                    _row6 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 7:
                    _row7 = LEDArray.BytesFromBools(LEDData).ToArray();
                    break;
                case 8:
                    break;
                default:
                    throw new ArgumentException(message: $"invalid RowNum provided. Got {RowNum} but expected 1 through 7", paramName: nameof(RowNum));
            }
        }

        /// <summary>
        /// Fills the whole screen of rows for the current ScreenData object with the correct frame buffer from the provided boolean values.
        /// Can be provided 147 bool values (7 rows) or 168 bool values (8 rows), but row 8 will always be ignored.
        /// Does NOT change screen data on the USB device itself (use <seealso cref="USBInterface"/> for this)
        /// </summary>
        /// <param name="LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> of exactly (21 x 7 = 147) boolean values (or 21 x 8 = 168 if the last row is to be ignored), read left-to-right, then top-to-bottom</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="LEDData"/> is a collection of booleans of a length other than 147 (or 168 if the last row is to be ignored)</exception>
        public void FillFromBools(params IEnumerable<bool> LEDData)
        {
            if (LEDData.Count() != 147 && LEDData.Count() != 168)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(LEDData), message: $"input array MUST be 147 long (or 168 if last 21 are to be ignored). Current length is {LEDData.Count()}");
            }

            IEnumerable<IEnumerable<bool>> rows = LEDData.Chunk(21);

            _row1 = LEDArray.BytesFromBools(rows.ElementAt(0)).ToArray();
            _row2 = LEDArray.BytesFromBools(rows.ElementAt(1)).ToArray();
            _row3 = LEDArray.BytesFromBools(rows.ElementAt(2)).ToArray();
            _row4 = LEDArray.BytesFromBools(rows.ElementAt(3)).ToArray();
            _row5 = LEDArray.BytesFromBools(rows.ElementAt(4)).ToArray();
            _row6 = LEDArray.BytesFromBools(rows.ElementAt(5)).ToArray();
            _row7 = LEDArray.BytesFromBools(rows.ElementAt(6)).ToArray();
        }

        /// <summary>
        /// Fills the whole screen of rows for the current ScreenData object with the correct frame buffer from the provided boolean values.
        /// Can be provided 7 or 8 rows of 21 bools each, but row 8 will always be ignored.
        /// Does NOT change screen data on the USB device itself (use <seealso cref="USBInterface"/> for this)
        /// </summary>
        /// <param name="Row1LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 1, having 21 elements</param>
        /// <param name="Row2LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 2, having 21 elements</param>
        /// <param name="Row3LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 3, having 21 elements</param>
        /// <param name="Row4LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 4, having 21 elements</param>
        /// <param name="Row5LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 5, having 21 elements</param>
        /// <param name="Row6LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 6, having 21 elements</param>
        /// <param name="Row7LEDData"><see cref="IEnumerable{Bool}">IEnumerable&lt;bool&gt;</see> for row 7, having 21 elements</param>
        /// <param name="Row8LEDData">Ignored</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the 7 expected rows is not exactly 21 items long</exception>
        public void FillFromBools(IEnumerable<bool> Row1LEDData, IEnumerable<bool> Row2LEDData, IEnumerable<bool> Row3LEDData, IEnumerable<bool> Row4LEDData,
            IEnumerable<bool> Row5LEDData, IEnumerable<bool> Row6LEDData, IEnumerable<bool> Row7LEDData, IEnumerable<bool>? Row8LEDData = null)
        {
            if (Row1LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row1LEDData), message: $"Invalid LED count on row 1; expected 21, received {Row1LEDData.Count()}");
            }
            else if (Row2LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row2LEDData), message: $"Invalid LED count on row 2; expected 21, received {Row2LEDData.Count()}");
            }
            else if (Row3LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row3LEDData), message: $"Invalid LED count on row 3; expected 21, received {Row3LEDData.Count()}");
            }
            else if (Row4LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row4LEDData), message: $"Invalid LED count on row 4; expected 21, received {Row4LEDData.Count()}");
            }
            else if (Row5LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row5LEDData), message: $"Invalid LED count on row 5; expected 21, received {Row5LEDData.Count()}");
            }
            else if (Row6LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row6LEDData), message: $"Invalid LED count on row 6; expected 21, received {Row6LEDData.Count()}");
            }
            else if (Row7LEDData.Count() != 21)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row7LEDData), message: $"Invalid LED count on row 7; expected 21, received {Row7LEDData.Count()}");
            }

            _row1 = LEDArray.BytesFromBools(Row1LEDData).ToArray();
            _row2 = LEDArray.BytesFromBools(Row2LEDData).ToArray();
            _row3 = LEDArray.BytesFromBools(Row3LEDData).ToArray();
            _row4 = LEDArray.BytesFromBools(Row4LEDData).ToArray();
            _row5 = LEDArray.BytesFromBools(Row5LEDData).ToArray();
            _row6 = LEDArray.BytesFromBools(Row6LEDData).ToArray();
            _row7 = LEDArray.BytesFromBools(Row7LEDData).ToArray();
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> of the frame buffer for the whole screen at the provided <paramref name="Brightness"/> value
        /// </summary>
        /// <param name="Brightness"><seealso cref="LEDArray.Brightness"/>. Defaults to current Brightness value.</param>
        /// <returns><see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> to send to the USB LED message board.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Brightness value is invalid</exception>
        public IEnumerable<byte> MakeFrameBuffer(byte? Brightness = null)
        {
            if (!Brightness.HasValue)
            {
                Brightness = _brightness;
            }
            else if (Brightness < 0 || Brightness > 2)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Brightness), message: $"Brightness value was {Brightness:0}, but expected 0 - 2");
            }

            return
            [
                .. LEDArray.MakeFrameBuffer(Brightness.Value, LEDArray.RowGroups.Rows12, Row1, Row2),
                .. LEDArray.MakeFrameBuffer(Brightness.Value, LEDArray.RowGroups.Rows34, Row3, Row4),
                .. LEDArray.MakeFrameBuffer(Brightness.Value, LEDArray.RowGroups.Rows56, Row5, Row6),
                .. LEDArray.MakeFrameBuffer(Brightness.Value, LEDArray.RowGroups.Row7, Row7)
            ];
        }

        /// <summary>
        /// Creates a frame buffer array for the whole screen at the provided <paramref name="Brightness"/> value
        /// </summary>
        /// <param name="Brightness">See <see cref="LEDArray.Brightness"/>. Defaults to current Brightness value.</param>
        /// <returns>Array of bytes to send to the USB LED message board.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Brightness value is invalid</exception>
        public byte[] MakeFrameBufferArray(byte? Brightness = null) => MakeFrameBuffer(Brightness).ToArray();

        /// <summary>
        /// Checks to see if this ScreenData instance is equal to another
        /// </summary>
        /// <param name="obj">Object to compare against</param>
        /// <returns><code>true</code> only if object is a non-null ScreenData with identical Brightness and identical Row Data</returns>
        public override bool Equals(object? obj)
        {
            // Get the obvious checks out of the way
            if (obj == null) return false;
            else if (ReferenceEquals(this, obj)) return true;
            else if (!obj.GetType().Equals(GetType())) return false;

            ScreenData sd = (ScreenData)obj;

            if (_brightness != sd.Brightness) return false;

            if (_row1.Length != sd.Row1.Length) return false;
            if (!_row1.SequenceEqual(sd.Row1)) return false;

            if (_row2.Length != sd.Row2.Length) return false;
            if (!_row2.SequenceEqual(sd.Row2)) return false;

            if (_row3.Length != sd.Row3.Length) return false;
            if (!_row3.SequenceEqual(sd.Row3)) return false;

            if (_row4.Length != sd.Row4.Length) return false;
            if (!_row4.SequenceEqual(sd.Row4)) return false;

            if (_row5.Length != sd.Row5.Length) return false;
            if (!_row5.SequenceEqual(sd.Row5)) return false;

            if (_row6.Length != sd.Row6.Length) return false;
            if (!_row6.SequenceEqual(sd.Row6)) return false;

            if (_row7.Length != sd.Row7.Length) return false;
            if (!_row7.SequenceEqual(sd.Row7)) return false;

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();
        #endregion
    }
}
