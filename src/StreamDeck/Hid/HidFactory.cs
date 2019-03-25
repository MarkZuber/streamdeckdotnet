// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zube.StreamDeck.Hid
{
    internal static class HidFactory
    {
        static HidFactory()
        {
            HidStart();
        }

        public static void HidStart()
        {
            int ret = NativeMethods.HidInit();
            if (ret != 0)
            {
                throw new HidException($"HidInit failed: {ret}");
            }
        }

        public static void HidExit()
        {
            NativeMethods.HidExit();
        }

        public static IEnumerable<HidDeviceInfo> GetHidDeviceInfos(ushort vendorId, ushort productId)
        {
            var deviceInfos = new List<HidDeviceInfo>();

            IntPtr enumeratePtr = NativeMethods.HidEnumerate(vendorId, productId);
            try
            {
                IntPtr curPtr = enumeratePtr;
                while (curPtr != IntPtr.Zero)
                {
                    HidDeviceInfoStruct diStruct = Marshal.PtrToStructure<HidDeviceInfoStruct>(curPtr);
                    var deviceInfo = new HidDeviceInfo(diStruct);
                    deviceInfos.Add(deviceInfo);
                    curPtr = diStruct.NextHidDeviceInfoStruct;
                }
            }
            finally
            {
                int ret = NativeMethods.HidFreeEnumeration(enumeratePtr);
                if (ret != 0)
                {
                    throw new HidException($"HidFreeEnumeration returned: {ret}");
                }
            }

            return deviceInfos;
        }

        public static HidDevice OpenHidDevice(ushort vendorId, ushort productId, string serialNumber)
        {
            IntPtr val = NativeMethods.HidOpen(vendorId, productId, serialNumber);
            if (val == IntPtr.Zero)
            {
                throw new HidException($"Hid Device not Found {vendorId:X} {productId:X} {serialNumber}");
            }
            return new HidDevice(val);
        }

        public static HidDevice OpenHidDevice(string path)
        {
            IntPtr hidPtr = NativeMethods.HidOpenPath(path);
            if (hidPtr == IntPtr.Zero)
            {
                throw new HidException($"Hid Device not Found {path}");
            }
            return new HidDevice(hidPtr);
        }
    }
}
