// Copyright (c) Mark Zuber. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Zube.StreamDeck;

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
                    deck.KeyPressed += (_, keyArgs) =>
                    {
                        Console.WriteLine($"Key Changed: Idx({keyArgs.KeyIndex}) State({keyArgs.KeyOn})");
                    };

                    deck.SetBrightness(75);
                    System.Threading.Thread.Sleep(500);

                    CycleImages(deck);
                    CycleImagesExact(deck);
                    ResetAndClearCycle(deck);
                    ColorCycle(deck);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void CycleImagesExact(IStreamDeckController deck)
        {
            string imageFilePath = Path.Combine(GetImagePath(), "hellothere.png");
            Image<Bgr24> image;
            using (var stream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                image = Image.Load<Bgr24>(stream);
            }

            deck.Reset();
            for (int i = 0; i < deck.NumKeys; i++)
            {
                deck.SetImageExact(i, image);
                System.Threading.Thread.Sleep(500);
            }
        }

        private static string GetImagePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static void CycleImages(IStreamDeckController deck)
        {
            deck.Reset();
            for (int i = 0; i < deck.NumKeys; i++)
            {
                deck.SetImage(i, Path.Combine(GetImagePath(), "smiles.png"));
                System.Threading.Thread.Sleep(500);
            }
        }

        private static void ResetAndClearCycle(IStreamDeckController deck)
        {
            deck.Reset();
            System.Threading.Thread.Sleep(500);
            deck.ClearAllKeys();
            System.Threading.Thread.Sleep(500);
            deck.Reset();
            System.Threading.Thread.Sleep(500);
            deck.ClearAllKeys();
            System.Threading.Thread.Sleep(500);
        }

        private static void ColorCycle(IStreamDeckController deck)
        {
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
}
