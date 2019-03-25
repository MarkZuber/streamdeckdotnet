// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace Zube.StreamDeck.Hid
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct HidDeviceInfoStruct
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Path;  // (char*)
        public ushort VendorId;
        public ushort ProductId;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string SerialNumber; // wchar_t*

        public ushort ReleaseNumber;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string ManufacturerString; // wchar_t*

        [MarshalAs(UnmanagedType.LPWStr)]
        public string ProductString; // wchar_t*

        public ushort UsagePage;
        public ushort Usage;
        public int InterfaceNumber;
        public IntPtr NextHidDeviceInfoStruct;
    }
}
