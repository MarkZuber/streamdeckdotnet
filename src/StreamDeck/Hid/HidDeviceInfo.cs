// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

namespace Zube.StreamDeck.Hid
{
    internal class HidDeviceInfo
    {
        internal HidDeviceInfo(HidDeviceInfoStruct diStruct)
        {
            Path = diStruct.Path;
            VendorId = diStruct.VendorId;
            ProductId = diStruct.ProductId;
            SerialNumber = diStruct.SerialNumber;
            ReleaseNumber = diStruct.ReleaseNumber;
            Manufacturer = diStruct.ManufacturerString;
            Product = diStruct.ProductString;
            UsagePage = diStruct.UsagePage;
            Usage = diStruct.Usage;
            InterfaceNumber = diStruct.InterfaceNumber;
        }

        public override string ToString()
        {
            return $"HidDevice Path({Path}) VendorId({VendorId:X}) ProductId({ProductId:X}) SerNo({SerialNumber}) Manufacturer({Manufacturer})";
        }

        public string Path { get; }
        public ushort VendorId { get; }
        public ushort ProductId { get; }
        public string SerialNumber { get; }
        public ushort ReleaseNumber { get; }
        public string Manufacturer { get; }
        public string Product { get; }
        public ushort UsagePage { get; }
        public ushort Usage { get; }
        public int InterfaceNumber { get; }
    }
}
