// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;

namespace StreamDeck
{
    public interface IStreamDeckController : IDisposable
    {
        int IconSize { get; }
        int NumButtonColumns { get; }
        int NumButtonRows { get; }
        int NumKeys { get; }

        event EventHandler<StreamDeckKeyChangedEventArgs> KeyPressed;

        void ClearAllKeys();
        void ClearKey(int keyIndex);
        void Close();
        void FillAllKeysWithColor(byte r, byte g, byte b);
        void FillColor(int keyIndex, byte r, byte g, byte b);
        void Reset();
        void SetBrightness(int percentage);
    }
}
