using HidSharp;

#pragma warning disable CS8618 // HidSharp allows nulls as intended behavior for what Visual Studio thinks are "Non-nullable fields," so just ignore this entirely.
#pragma warning disable CS8603 // "Possible null reference return." -- yes. This is intended behavior by HidSharp, so ignore this one too.
#pragma warning disable IDE0090 // Use 'new(...)' -- This just saves a few characters for a format that I personally don't like and find really vague. Ignore this one.
#pragma warning disable IDE0305 // Simplify collection initialization -- Honestly I just hate how this looks

namespace DreamCheekyLED

{
    /// <summary>
    /// USB Interface operations for the Dream Cheeky LED Message Board
    /// </summary>
    public class USBInterface : IDisposable
    {
        /// <summary>
        /// Readonly flag variable that indicates whether or not the current instance is actively running screen updates.
        /// Does <b>not</b> require additional computing to retrieve the current state as it is set internally.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        private bool _isOpen = false;
        private bool _shouldStop = false;
        private bool _frameBufferNeedsUpdate = false;
        private HidDevice _device;
        private HidStream _stream;
        private byte[] _currentFrameBuffer;
        private byte[] _nextFrameBuffer;
        private Task? _task;

        #region USBInterface - Open Methods
        /// <summary>
        /// Tries to open a HidDevice for the Dream Cheeky USB LED Message Board. Will swallow errors unless you forget to dispose it before opening it again.
        /// </summary>
        /// <param name="openSuccess">Whether or not accessing the USB LED Message Board was successful</param>
        /// <returns><see cref="USBInterface"/> with opened stream</returns>
        /// <exception cref="InvalidOperationException">Thrown if you try to open the device twice without disposing it. Keep only one USBInterface Open() at any given time, and use Run() and Stop() to control behavior instead.</exception>
        public static USBInterface TryOpen(out bool openSuccess) => Open(false, out openSuccess);

        /// <summary>
        /// Tries to open a HidDevice for the Dream Cheeky USB LED Message Board. Will throw errors if it is not successful. If you do not want errors, use <see cref="TryOpen(out bool)"/> instead.
        /// </summary>
        /// <returns><see cref="USBInterface"/> with opened stream</returns>
        /// <exception cref="InvalidOperationException">Thrown if you try to open the device twice without disposing it. Keep only one USBInterface Open() at any given time, and use Run() and Stop() to control behavior instead.</exception>
        /// <exception cref="DriveNotFoundException">Thrown if HidSharp can't find the Dream Link USB LED Message Board v1.0.1 (Vendor 7476, Product 19)</exception>
        /// <exception cref="Exception">Whatever exceptions HidSharp throws when the HidDevice is attempted to be open will be thrown too</exception>
        public static USBInterface Open() => Open(true, out _);

        /// <summary>
        /// Private overload for public Open() and TryOpen() methods that holds the open logic itself.
        /// </summary>
        /// <param name="throwErrors">set to <value>false</value> to handle errors internally to this method</param>
        /// <param name="openSuccess">Whether or not accessing the USB LED Message Board was successful</param>
        /// <returns><see cref="USBInterface"/> with opened stream</returns>
        /// <exception cref="InvalidOperationException">Thrown if you try to open the device twice without disposing it. Keep only one USBInterface Open() at any given time, and use Run() and Stop() to control behavior instead.</exception>
        /// <exception cref="DriveNotFoundException">Thrown if HidSharp can't find the Dream Link USB LED Message Board v1.0.1 (Vendor 7476, Product 19), unless throwErrors is false</exception>
        /// <exception cref="Exception">Only if throwErrors is true, whatever exceptions HidSharp throws when the HidDevice is attempted to be open will be thrown too</exception>
        private static USBInterface Open(bool throwErrors, out bool openSuccess)
        {
            openSuccess = false;

            USBInterface u = new USBInterface();

            if (u._isOpen)
            {
                // HidSharp.HidStream has a TryOpen() method that returns a boolean but passes the stream as an out parameter. While this works
                // great for TryParse() situations, it no longer allows using() statements to work. The intended goal of TryOpen() is to avoid
                // throwing exceptions should something not work, and instead let the coder handle scenarios more gracefully.
                // 
                // [ using ( USBInterface dev = USBInterface.Open(false) ) ] for example, there is no scenario in which you should get this.
                // You should NEVER get this exception, so I'm opting to allow throwing this particular error to discourage bad coding practices.
                throw new InvalidOperationException(message: "Cannot open device when device is already open (did you mean to call Run() instead?)");
            }

            u._device = DeviceList.Local.GetHidDeviceOrNull(7476, 19);

            if (u._device == null)
            {
                if (throwErrors)
                {
                    throw new DriveNotFoundException(message: "Dream Link USB LED Message Board v1.0.1 not found");
                }
                else
                {
                    return null;
                }
            }

            try
            {
                u._stream = u._device.Open();
                openSuccess = true;
                u._isOpen = true;
                return u;
            }
            catch (Exception)
            {
                if (throwErrors) { throw; }
                return null;
            }
        }
        #endregion

        #region USBInterface - Update Methods
        /// <summary>
        /// Updates the frame buffer for the USB LED Message Board. See <seealso cref="LEDArray"/> for generating this data.
        /// </summary>
        /// <param name="Rows12">Framebuffer data for rows 1 and 2</param>
        /// <param name="Rows34">Framebuffer data for rows 3 and 4</param>
        /// <param name="Rows56">Framebuffer data for rows 5 and 6</param>
        /// <param name="Row7">Framebuffer data for row 7 (and dummy data for row 8)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any parameter is not 9 bytes long</exception>
        public void SetFrameBuffer(IEnumerable<byte> Rows12, IEnumerable<byte> Rows34, IEnumerable<byte> Rows56, IEnumerable<byte> Row7)
        {
            if (Rows12.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows12), message: $"Rows12 array MUST have exactly 9 items in it. Current length is {Rows12.Count()}");
            }
            if (Rows34.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows34), message: $"Rows34 array MUST have exactly 9 items in it. Current length is {Rows34.Count()}");
            }
            if (Rows56.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Rows56), message: $"Rows56 array MUST have exactly 9 items in it. Current length is {Rows56.Count()}");
            }
            if (Row7.Count() != 9)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(Row7), message: $"Row7 array MUST have exactly 9 items in it. Current length is {Row7.Count()}");
            }

            SetFrameBuffer(new[] { Rows12, Rows34, Rows56, Row7 }.SelectMany(x => x).ToArray());
        }

        /// <summary>
        /// Updates the frame buffer for the USB LED Message Board. See <seealso cref="ScreenData"/> for generating this data.
        /// </summary>
        /// <param name="AllRows">Framebuffer data for whole screen</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Framebuffer is not 36 bytes long</exception>
        public void SetFrameBuffer(IEnumerable<byte> AllRows)
        {
            if (AllRows.Count() != 36)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(AllRows), message: $"Row7 array MUST have exactly 36 items in it. Current length is {AllRows.Count()}");
            }

            if (IsRunning)
            {
                _nextFrameBuffer = AllRows.ToArray();
                _frameBufferNeedsUpdate = true;
            }
            else
            {
                _currentFrameBuffer = AllRows.ToArray();
            }
        }

        /// <summary>
        /// Private-only method that updates the frame buffers asyncronously to ensure minimal hiccups in the timing of updating the screen.
        /// (There is always a delay after this is called that's long enough for the frame buffer arrays to be updated before the next screen refresh starts.)
        /// </summary>
        private void UpdateFrameBuffer()
        {
            _nextFrameBuffer.CopyTo(_currentFrameBuffer);
            _frameBufferNeedsUpdate = false;
        }
        #endregion

        #region USBInterface - Run Methods
        /// <summary>
        /// Takes the frame buffer and sends it to the device on a continual basis. Use <seealso cref="SetFrameBuffer(IEnumerable{byte})"/> or
        /// <seealso cref="SetFrameBuffer(IEnumerable{byte}, IEnumerable{byte}, IEnumerable{byte}, IEnumerable{byte})"/> to set the data it sends,
        /// or use a different overload to set the initial frame buffer to run.
        /// <br /><br />
        /// Will exit early without throwing an error if <see cref="IsRunning"/> is <c>true</c> or if the frame buffer is not set correctly.
        /// </summary>
        public void Run()
        {
            if (IsRunning) { return; }
            if (_currentFrameBuffer.Length != 36) { return; } // Frame buffer not set, so ignore. Shouldn't ever be hit once SetFrameBuffers() is called the first time, so we do not need to check this again.

            IsRunning = true;

            if (_frameBufferNeedsUpdate) { _nextFrameBuffer.CopyTo(_currentFrameBuffer); }

            if (_shouldStop)
            {
                IsRunning = false;
                return;
            }

            _task = Task.Run(RunLoop);
        }

        /// <summary>
        /// Takes the frame buffer and sends it to the device on a continual basis. Will exit early without throwing an error if <see cref="IsRunning"/> is <c>true</c>
        /// </summary>
        /// <param name="InitialData_AllRows">Framebuffer data for whole screen</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Framebuffer is not 36 bytes long</exception>
        public void Run(IEnumerable<byte> InitialData_AllRows)
        {
            SetFrameBuffer(InitialData_AllRows);
            Run();
        }

        /// <summary>
        /// Takes the frame buffer and sends it to the device on a continual basis. Will exit early without throwing an error if <see cref="IsRunning"/> is <c>true</c>
        /// </summary>
        /// <param name="InitialData_Rows12">Framebuffer data for rows 1 and 2</param>
        /// <param name="InitialData_Rows34">Framebuffer data for rows 3 and 4</param>
        /// <param name="InitialData_Rows56">Framebuffer data for rows 5 and 6</param>
        /// <param name="InitialData_Row7">Framebuffer data for row 7 (and dummy data for row 8)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any parameter is not 9 bytes long</exception>
        public void Run(IEnumerable<byte> InitialData_Rows12, IEnumerable<byte> InitialData_Rows34, IEnumerable<byte> InitialData_Rows56, IEnumerable<byte> InitialData_Row7)
        {
            SetFrameBuffer(InitialData_Rows12, InitialData_Rows34, InitialData_Rows56, InitialData_Row7);
            Run();
        }

        /// <summary>
        /// The actual run loop itself, as a task to allow multithreading
        /// </summary>
        /// <returns><seealso cref="Task"/></returns>
        private async Task RunLoop()
        {
            try
            {
                //awaits inside this loop are specifically there in case the tasks for whatever reason run long. They shouldn't, but just in case. 
                //Loop is checked at the end so it runs at least one screen update no matter what. In theory you could wrap this in a RunOnce() that
                //does Run(); Stop(); without delay if you want to show the screen only briefly, but the timing of such is dependent on packet delay timings set above
                do
                {
                    _stream.Write(_currentFrameBuffer);
                    Thread.Sleep(5);
                    if (_frameBufferNeedsUpdate) { UpdateFrameBuffer(); }
                }
                while (!_shouldStop);

                IsRunning = false;
                _shouldStop = false;
            }
            catch (Exception)
            {
                IsRunning = false;
                _shouldStop = false;
                throw;
            }
        }
        #endregion

        #region USBInterface - Stop Methods
        /// <summary>
        /// Stops the LED Message Board from updating, does NOT clear it. The message board has a hardware timeout on its own of about half a second.
        /// This method will exit early without trying to stop the display if a prior Stop() call has not completed, or if <see cref="IsRunning"/> is <c>false</c>.
        /// <br /><br />
        /// Simply tells the <see cref="Run()"/> method to stop sending the frame buffer to the device on a timer. <see cref="Dispose"/> will still handle correctly without calling this method.
        /// </summary>
        public void Stop()
        {
            if (_shouldStop) return;
            if (!IsRunning) return;

            // Set flag first            
            _shouldStop = true;

            //Wait it out
            _task?.Wait();

            //Clear thread
            _task = null;

            //Doesn't need to stop anymore as it's already stopped
            _shouldStop = false;
        }

        /// <summary>
        /// Closes the Stream
        /// </summary>
        /// <param name="throwErrors">Optional parameter. Set to <value>true</value> if you want to throw any generated exceptions</param>
        public void Close(bool throwErrors = false)
        {
            try
            {
                _isOpen = false;
                _stream.Close();
            }
            catch (Exception)
            {
                if (throwErrors) { throw; } // Pretty rare case to want this
            }
        }

        /// <summary>
        /// Generic Dispose() method. Mostly cleans up the <see cref="HidStream"/> and stops the subprocess that handles screen updates
        /// </summary>
        public void Dispose()
        {
            Stop();
            _stream.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}