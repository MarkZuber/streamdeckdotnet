// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Zube.StreamDeck
{
    public class StreamDeckException : Exception
    {
        public StreamDeckException(string message)
            : base(message)
        {
        }

        public StreamDeckException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
