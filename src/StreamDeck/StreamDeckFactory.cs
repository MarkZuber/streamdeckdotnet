// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Zube.StreamDeck.Hid;

namespace Zube.StreamDeck
{
    public static class StreamDeckFactory
    {
        public static IStreamDeckController CreateDeck(string path = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    var deviceInfo = HidFactory.GetHidDeviceInfos(StreamDeckController.VendorId, StreamDeckController.ProductId).FirstOrDefault();

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
    }
}
