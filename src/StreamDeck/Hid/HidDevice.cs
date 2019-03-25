// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Zube.StreamDeck.Hid
{
    internal class HidDevice : IDisposable
    {
        private IntPtr _deviceHandle;

        internal HidDevice(IntPtr deviceHandle)
        {
            if (deviceHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(deviceHandle));
            }

            _deviceHandle = deviceHandle;
        }

        public void Write(byte[] data)
        {
            UIntPtr length = new UIntPtr(Convert.ToUInt64(data.LongLength));
            int retval = NativeMethods.HidWrite(_deviceHandle, data, length);

            if (retval < 0)
            {
                throw new HidException("Write failed: " + GetErrorString());
            }
        }

        public byte[] ReadTimeout(int dataLength, int timeoutMilliseconds)
        {
            byte[] buf = new byte[dataLength];

            int retval = NativeMethods.HidReadTimeout(_deviceHandle, buf, dataLength, timeoutMilliseconds);
            if (retval < 0)
            {
                throw new HidException("read with timeout failed: " + GetErrorString());
            }

            Array.Resize(ref buf, retval);
            return buf;
        }

        public byte[] Read(int dataLength)
        {
            byte[] buf = new byte[dataLength];

            int retval = NativeMethods.HidRead(_deviceHandle, buf, dataLength);
            if (retval < 0)
            {
                throw new HidException("read failed: " + GetErrorString());
            }

            Array.Resize(ref buf, retval);
            return buf;
        }

        public void SetNonBlocking(bool nonBlocking)
        {
            int retval = NativeMethods.HidSetNonblocking(_deviceHandle, nonBlocking ? 1 : 0);
            if (retval < 0)
            {
                throw new HidException("HidSetNonBlocking failed: " + GetErrorString());
            }
        }

        private string GetErrorString()
        {
            return NativeMethods.HidError(_deviceHandle);
        }

        public void SendFeatureReport(byte[] data)
        {
            // will be -1 on error
            int numBytesWritten = NativeMethods.HidSendFeatureReport(_deviceHandle, data, data.Length);
            if (numBytesWritten < 0)
            {
                throw new HidException(NativeMethods.HidError(_deviceHandle));
            }
        }

        public byte[] GetFeatureReport(long dataLength)
        {
            throw new NotImplementedException();
        }

        public string GetManufacturer()
        {
            StringBuilder value = new StringBuilder();
            int retval = NativeMethods.HidGetManufacturerString(_deviceHandle, value, 1000);
            if (retval < 0)
            {
                throw new HidException("GetManufacturer failed: " + GetErrorString());
            }

            return value.ToString();
        }

        public string GetProduct()
        {
            StringBuilder value = new StringBuilder();
            int retval = NativeMethods.HidGetProductString(_deviceHandle, value, 1000);
            if (retval < 0)
            {
                throw new HidException("GetProduct failed: " + GetErrorString());
            }

            return value.ToString();
        }

        public string GetSerialNumber()
        {
            StringBuilder value = new StringBuilder();
            int retval = NativeMethods.HidGetSerialNumberString(_deviceHandle, value, 1000);
            if (retval < 0)
            {
                throw new HidException("GetSerialNumber failed: " + GetErrorString());
            }

            return value.ToString();
        }

        public string GetIndexedString(int index)
        {
            StringBuilder value = new StringBuilder();
            int retval = NativeMethods.HidGetIndexedString(_deviceHandle, index, value, 1000);
            if (retval < 0)
            {
                throw new HidException("GetIndexedString failed: " + GetErrorString());
            }

            return value.ToString();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (_deviceHandle != IntPtr.Zero)
                {
                    NativeMethods.HidClose(_deviceHandle);
                    _deviceHandle = IntPtr.Zero;
                }

                _disposedValue = true;
            }
        }

        ~HidDevice()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
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
