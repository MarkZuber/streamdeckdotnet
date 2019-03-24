// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;
using StreamDeck;

namespace StreamDeckCon
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Streamdeck!");

            try
            {
                using (var deck = StreamDeckFactory.CreateDeck())
                {
                    Console.WriteLine("StreamDeck created.");

                    deck.SetBrightness(75);
                    System.Threading.Thread.Sleep(500);

                    deck.Reset();
                    System.Threading.Thread.Sleep(500);
                    deck.ClearAllKeys();
                    System.Threading.Thread.Sleep(500);
                    deck.Reset();
                    System.Threading.Thread.Sleep(500);
                    deck.ClearAllKeys();
                    System.Threading.Thread.Sleep(500);

                    deck.KeyPressed += (_, keyArgs) =>
                    {
                        Console.WriteLine($"Key Changed: Idx({keyArgs.KeyIndex}) State({keyArgs.KeyOn})");
                    };

                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (r = 0; r < 255; r += 10)
                    {
                        deck.FillAllKeysWithColor(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                    }
                    r = 0;
                    g = 0;
                    b = 0;
                    for (g = 0; g < 255; g += 10)
                    {
                        deck.FillAllKeysWithColor(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                    }

                    r = 0;
                    g = 0;
                    b = 0;
                    for (b = 0; b < 255; b += 10)
                    {
                        deck.FillAllKeysWithColor(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                        System.Threading.Thread.Sleep(00);
                    }

                    deck.FillAllKeysWithColor(0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
