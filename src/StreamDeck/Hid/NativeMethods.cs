using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zube.StreamDeck.Hid
{
    internal static class NativeMethods
    {
        private const string HidApiLib = "libhidapi-libusb";

        [DllImport(HidApiLib, EntryPoint = "hid_init")]
        public static extern int HidInit();

        [DllImport(HidApiLib, EntryPoint = "hid_exit")]
        public static extern int HidExit();

        [DllImport(HidApiLib, EntryPoint = "hid_enumerate")]
        public static extern IntPtr HidEnumerate(
            ushort vendorId,
            ushort productId);

        [DllImport(HidApiLib, EntryPoint = "hid_free_enumeration")]
        public static extern int HidFreeEnumeration(IntPtr devices);

        [DllImport(HidApiLib, EntryPoint = "hid_open", CharSet = CharSet.Ansi)]
        public static extern IntPtr HidOpen(
            ushort vendorId,
            ushort productId,
            [MarshalAs(UnmanagedType.LPWStr)] string serialNumber);

        [DllImport(HidApiLib, EntryPoint = "hid_open_path", CharSet = CharSet.Ansi)]
        public static extern IntPtr HidOpenPath(
            [MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(HidApiLib, EntryPoint = "hid_write", CallingConvention = CallingConvention.Cdecl)]
        public static extern int HidWrite(
            IntPtr device,
            [MarshalAs(UnmanagedType.LPArray)] byte[] data,
            UIntPtr length);

        [DllImport(HidApiLib, EntryPoint = "hid_read_timeout")]
        public static extern int HidReadTimeout(
            IntPtr device,
            byte[] data,
            long length,
            int milliseconds);

        [DllImport(HidApiLib, EntryPoint = "hid_read")]
        public static extern int HidRead(IntPtr device, byte[] data, long length);

        [DllImport(HidApiLib, EntryPoint = "hid_set_nonblocking")]
        public static extern int HidSetNonblocking(IntPtr device, int nonblock);

        [DllImport(HidApiLib, EntryPoint = "hid_send_feature_report")]
        public static extern int HidSendFeatureReport(IntPtr device, byte[] data, long length);

        [DllImport(HidApiLib, EntryPoint = "hid_get_feature_report")]
        public static extern int HidGetFeatureReport(IntPtr device, byte[] data, long length);

        [DllImport(HidApiLib, EntryPoint = "hid_close")]
        public static extern int HidClose(IntPtr device);

        [DllImport(HidApiLib, EntryPoint = "hid_get_manufacturer_string", CharSet = CharSet.Unicode)]
        public static extern int HidGetManufacturerString(IntPtr device, StringBuilder value, long maxlen);

        [DllImport(HidApiLib, EntryPoint = "hid_get_product_string", CharSet = CharSet.Unicode)]
        public static extern int HidGetProductString(IntPtr device, StringBuilder value, long maxlen);

        [DllImport(HidApiLib, EntryPoint = "hid_get_serial_number_string", CharSet = CharSet.Unicode)]
        public static extern int HidGetSerialNumberString(IntPtr device, StringBuilder value, long maxlen);

        [DllImport(HidApiLib, EntryPoint = "hid_get_indexed_string", CharSet = CharSet.Unicode)]
        public static extern int HidGetIndexedString(IntPtr device, int stringIndex, StringBuilder value, long maxlen);

        [DllImport(HidApiLib, EntryPoint = "hid_error")]
        public static extern string HidError(IntPtr device);
    }
}
