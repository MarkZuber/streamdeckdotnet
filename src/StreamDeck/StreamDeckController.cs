// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using StreamDeck.Hid;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeck
{
    internal class StreamDeckController : IStreamDeckController
    {
        private const ushort VendorId = 0x0fd9;
        private const ushort ProductId = 0x0060;

        private const int PagePacketSize = 8191;
        private const int NumFirstPagePixels = 2583;
        private const int NumSecondPagePixels = 2601;

        private readonly object _lockObj = new object();
        private readonly HidDevice _hidDevice;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _readTask;

        private StreamDeckController(HidDevice hidDevice)
        {
            _hidDevice = hidDevice;
        }

        private void ReadThreadProc()
        {
            try
            {
                byte[] lastKeyStates = new byte[NumKeys + 1];
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var keyStates = ReadKeyStates();
                    if (keyStates != null && keyStates.Length > 0)
                    {
                        for (int keyIdx = 1; keyIdx <= NumKeys; keyIdx++)
                        {
                            if (keyStates[keyIdx] != lastKeyStates[keyIdx])
                            {
                                KeyPressed?.Invoke(this, new StreamDeckKeyChangedEventArgs(ConvertKeyIndex(keyIdx - 1), keyStates[keyIdx] > 0));
                            }
                        }

                        lastKeyStates = keyStates;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"ReadThreadProc exiting due to FAILURE: {ex}");
            }
        }

        private byte[] ReadKeyStates()
        {
            const int timeoutMsec = 500;
            try
            {
                return _hidDevice.ReadTimeout(NumKeys + 1, timeoutMsec);
            }
            catch (HidException ex)
            {
                throw new StreamDeckException("ReadKeyStates failed", ex);
            }
        }

        internal void Start()
        {
            if (_readTask != null)
            {
                throw new StreamDeckException("_readTask has already been started...");
            }

            _readTask = Task.Factory.StartNew(
                ReadThreadProc,
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current);
        }

        public void Close()
        {
            Dispose();
        }

        public event EventHandler<StreamDeckKeyChangedEventArgs> KeyPressed;

        public int IconSize => 72;
        public int NumKeys => 15;
        public int NumButtonColumns => 5;
        public int NumButtonRows => 3;

        private byte[] PadBufferToLength(byte[] buffer, int padLength)
        {
            var retval = new byte[padLength];

            if (padLength <= buffer.Length)
            {
                Array.Copy(buffer, retval, padLength);
            }
            else
            {
                Array.Copy(buffer, retval, buffer.Length);
            }
            return retval;
        }

        public static IStreamDeckController Create(string path = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    var deviceInfo = HidFactory.GetHidDeviceInfos(VendorId, ProductId).FirstOrDefault();

                    if (deviceInfo == null)
                    {
                        throw new StreamDeckException("No stream decks are connected");
                    }

                    path = deviceInfo.Path;
                }

                var streamDeck = new StreamDeckController(HidFactory.OpenHidDevice(path));
                streamDeck.Start();
                return streamDeck;
            }
            catch (HidException ex)
            {
                throw new StreamDeckException("Create failed", ex);
            }
        }

        private int ValidateKeyIndex(int keyIndex)
        {
            if (keyIndex < 0 || keyIndex >= NumKeys)
            {
                throw new ArgumentException($"key index must be between 0 and {NumKeys}", nameof(keyIndex));
            }

            return ConvertKeyIndex(keyIndex);
        }

        private int ConvertKeyIndex(int keyIndex)
        {
            int keyCol = keyIndex % NumButtonColumns;
            return (keyIndex - keyCol) + ((NumButtonColumns - 1) - keyCol);
        }

        public void FillAllKeysWithColor(byte r, byte g, byte b)
        {
            lock (_lockObj)
            {
                for (int i = 0; i < NumKeys; i++)
                {
                    FillColor(i, r, g, b);
                }
            }
        }

        public void FillColor(int keyIndex, byte r, byte g, byte b)
        {
            lock (_lockObj)
            {
                int key = ValidateKeyIndex(keyIndex);
                var pixel = new byte[] { b, g, r };
                WritePage1(key, CreateBuffer(NumFirstPagePixels * 3, pixel));
                WritePage2(key, CreateBuffer(NumSecondPagePixels * 3, pixel));
            }
        }

        private byte[] CreateBuffer(int bufferLength, byte[] repeatedFillData)
        {
            var buf = new byte[bufferLength];
            for (int i = 0; i < bufferLength; i += repeatedFillData.Length)
            {
                Array.Copy(repeatedFillData, 0, buf, i, repeatedFillData.Length);
            }
            return buf;
        }

        public void ClearKey(int keyIndex)
        {
            int key = ValidateKeyIndex(keyIndex);
            FillColor(key, 0, 0, 0);
        }

        public void ClearAllKeys()
        {
            FillAllKeysWithColor(0, 0, 0);
        }

        public void Reset()
        {
            var buf = new byte[] { 0x0B, 0x63 };
            SendFeatureReport(PadBufferToLength(buf, 17));
        }

        public void SetBrightness(int percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException("Value must be between 0 and 100", nameof(percentage));
            }

            var brightnessCommandBuffer = new byte[] { 0x05, 0x55, 0xaa, 0xd1, 0x01, Convert.ToByte(percentage) };
            SendFeatureReport(PadBufferToLength(brightnessCommandBuffer, 17));
        }

        private void WritePage1(int keyIndex, byte[] buffer)
        {
            var header = new byte[]
            {
                0x02, 0x01, 0x01, 0x00, 0x00, Convert.ToByte(keyIndex + 1), 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x42, 0x4d, 0xf6, 0x3c, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00,
                0x00, 0x00, 0x48, 0x00, 0x00, 0x00, 0x48, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xc0, 0x3c, 0x00, 0x00, 0xc4, 0x0e,
                0x00, 0x00, 0xc4, 0x0e, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            Write(BuildPacket(header, buffer, PagePacketSize));
        }

        private byte[] BuildPacket(byte[] header, byte[] buffer, int paddedBufferLength)
        {
            var contents = new byte[header.Length + buffer.Length];
            Array.Copy(header, contents, header.Length);
            Array.Copy(buffer, 0, contents, header.Length, buffer.Length);
            return PadBufferToLength(contents, paddedBufferLength);
        }

        private void WritePage2(int keyIndex, byte[] buffer)
        {
            var header = new byte[]
            {
                0x02, 0x01, 0x02, 0x00, 0x01, Convert.ToByte(keyIndex + 1), 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            Write(BuildPacket(header, buffer, PagePacketSize));
        }

        private void Write(byte[] buffer)
        {
            lock (_lockObj)
            {
                try
                {
                    _hidDevice.Write(buffer);
                }
                catch (HidException ex)
                {
                    throw new StreamDeckException("Write failed", ex);
                }
            }
        }

        private void ConsoleWriteBuffer(byte[] buffer)
        {
            Console.Write($"Buffer (len:{buffer.Length}): ");
            foreach (byte b in buffer)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine();
        }

        private void SendFeatureReport(byte[] buffer)
        {
            lock (_lockObj)
            {
                try
                {
                    _hidDevice.SendFeatureReport(buffer);
                }
                catch (HidException ex)
                {
                    throw new StreamDeckException("SendFeatureReport failed", ex);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    Reset();
                    _cancellationTokenSource.Cancel();
                    _readTask.Wait();
                    _hidDevice?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
