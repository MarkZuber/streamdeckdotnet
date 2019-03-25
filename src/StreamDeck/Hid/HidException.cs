// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Zube.StreamDeck.Hid
{
    internal class HidException : Exception
    {
        public HidException(string message) : base(message)
        {
        }
    }
}
