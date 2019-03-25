// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Zube.StreamDeck
{
    public class StreamDeckKeyChangedEventArgs : EventArgs
    {
        public StreamDeckKeyChangedEventArgs(int keyIndex, bool keyOn)
        {
            KeyIndex = keyIndex;
            KeyOn = keyOn;
        }

        public int KeyIndex { get; }
        public bool KeyOn { get; }
    }
}
