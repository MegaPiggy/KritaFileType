using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using PaintDotNet;
using PaintDotNet.Collections;
using PaintDotNet.Imaging;
using PaintDotNet.IO;

namespace KritaFileType
{
    public class KraFileType : FileType
    {
        private const int ThumbMaxSize = 256;

        private readonly string MimeType = "YXBwbGljYXRpb24veC1rcml0YQ==";
        private readonly string DefaultPixel = "AAAAAA==";
        private readonly string ICC = "AAAjeGxjbXMCEAAAbW50clJHQiBYWVogB98ACwAKAAwAEgA4YWNzcCpuaXgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPbWAAEAAAAA0y1sY21zAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALZGVzYwAAAQgAAACwY3BydAAAAbgAAAESd3RwdAAAAswAAAAUY2hhZAAAAuAAAAAsclhZWgAAAwwAAAAUYlhZWgAAAyAAAAAUZ1hZWgAAAzQAAAAUclRSQwAAA0gAACAMZ1RSQwAAA0gAACAMYlRSQwAAA0gAACAMY2hybQAAI1QAAAAkZGVzYwAAAAAAAAAcc1JHQi1lbGxlLVYyLXNyZ2J0cmMuaWNjAAAAAAAAAAAAAAAdAHMAUgBHAEIALQBlAGwAbABlAC0AVgAyAC0AcwByAGcAYgB0AHIAYwAuAGkAYwBjAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB0ZXh0AAAAAENvcHlyaWdodCAyMDE1LCBFbGxlIFN0b25lICh3ZWJzaXRlOiBodHRwOi8vbmluZWRlZ3JlZXNiZWxvdy5jb20vOyBlbWFpbDogZWxsZXN0b25lQG5pbmVkZWdyZWVzYmVsb3cuY29tKS4gVGhpcyBJQ0MgcHJvZmlsZSBpcyBsaWNlbnNlZCB1bmRlciBhIENyZWF0aXZlIENvbW1vbnMgQXR0cmlidXRpb24tU2hhcmVBbGlrZSAzLjAgVW5wb3J0ZWQgTGljZW5zZSAoaHR0cHM6Ly9jcmVhdGl2ZWNvbW1vbnMub3JnL2xpY2Vuc2VzL2J5LXNhLzMuMC9sZWdhbGNvZGUpLgAAAABYWVogAAAAAAAA9tYAAQAAAADTLXNmMzIAAAAAAAEMQgAABd7///MlAAAHkwAA/ZD///uh///9ogAAA9wAAMBuWFlaIAAAAAAAAG+gAAA49QAAA5BYWVogAAAAAAAAJJ8AAA+EAAC2xFhZWiAAAAAAAABilwAAt4cAABjZY3VydgAAAAAAABAAAAAAAQACAAQABQAGAAcACQAKAAsADAAOAA8AEAARABMAFAAVABYAGAAZABoAGwAcAB4AHwAgACEAIwAkACUAJgAoACkAKgArAC0ALgAvADAAMgAzADQANQA3ADgAOQA6ADsAPQA+AD8AQABCAEMARABFAEcASABJAEoATABNAE4ATwBRAFIAUwBUAFUAVwBYAFkAWgBcAF0AXgBfAGEAYgBjAGQAZgBnAGgAaQBrAGwAbQBuAG8AcQByAHMAdAB2AHcAeAB5AHsAfAB9AH4AgACBAIIAgwCFAIYAhwCIAIkAiwCMAI0AjgCQAJEAkgCTAJUAlgCXAJgAmgCbAJwAnQCfAKAAoQCiAKQApQCmAKcAqACqAKsArACtAK8AsACxALIAtAC1ALYAtwC5ALoAuwC8AL4AvwDAAMEAwgDEAMUAxgDHAMkAygDLAMwAzgDPANAA0QDTANQA1QDXANgA2QDaANwA3QDeAOAA4QDiAOQA5QDmAOgA6QDqAOwA7QDvAPAA8QDzAPQA9gD3APgA+gD7AP0A/gD/AQEBAgEEAQUBBwEIAQoBCwENAQ4BDwERARIBFAEVARcBGAEaARsBHQEfASABIgEjASUBJgEoASkBKwEtAS4BMAExATMBNAE2ATgBOQE7ATwBPgFAAUEBQwFFAUYBSAFKAUsBTQFPAVABUgFUAVUBVwFZAVoBXAFeAWABYQFjAWUBZwFoAWoBbAFuAW8BcQFzAXUBdgF4AXoBfAF+AX8BgQGDAYUBhwGJAYoBjAGOAZABkgGUAZYBlwGZAZsBnQGfAaEBowGlAacBqQGrAawBrgGwAbIBtAG2AbgBugG8Ab4BwAHCAcQBxgHIAcoBzAHOAdAB0gHUAdYB2AHaAdwB3gHhAeMB5QHnAekB6wHtAe8B8QHzAfUB+AH6AfwB/gIAAgICBAIHAgkCCwINAg8CEgIUAhYCGAIaAh0CHwIhAiMCJQIoAioCLAIuAjECMwI1AjgCOgI8Aj4CQQJDAkUCSAJKAkwCTwJRAlMCVgJYAloCXQJfAmECZAJmAmkCawJtAnACcgJ1AncCeQJ8An4CgQKDAoYCiAKLAo0CkAKSApUClwKaApwCnwKhAqQCpgKpAqsCrgKwArMCtQK4ArsCvQLAAsICxQLIAsoCzQLPAtIC1QLXAtoC3QLfAuIC5ALnAuoC7ALvAvIC9QL3AvoC/QL/AwIDBQMIAwoDDQMQAxMDFQMYAxsDHgMgAyMDJgMpAywDLgMxAzQDNwM6Az0DPwNCA0UDSANLA04DUQNUA1YDWQNcA18DYgNlA2gDawNuA3EDdAN3A3oDfQOAA4IDhQOIA4sDjgORA5QDmAObA54DoQOkA6cDqgOtA7ADswO2A7kDvAO/A8IDxQPJA8wDzwPSA9UD2APbA98D4gPlA+gD6wPuA/ID9QP4A/sD/gQCBAUECAQLBA8EEgQVBBgEHAQfBCIEJQQpBCwELwQzBDYEOQQ9BEAEQwRHBEoETQRRBFQEVwRbBF4EYgRlBGgEbARvBHMEdgR5BH0EgASEBIcEiwSOBJIElQSZBJwEoASjBKcEqgSuBLEEtQS4BLwEvwTDBMYEygTOBNEE1QTYBNwE4ATjBOcE6gTuBPIE9QT5BP0FAAUEBQgFCwUPBRMFFgUaBR4FIgUlBSkFLQUxBTQFOAU8BUAFQwVHBUsFTwVSBVYFWgVeBWIFZgVpBW0FcQV1BXkFfQWBBYQFiAWMBZAFlAWYBZwFoAWkBagFrAWvBbMFtwW7Bb8FwwXHBcsFzwXTBdcF2wXfBeMF5wXrBe8F9AX4BfwGAAYEBggGDAYQBhQGGAYcBiEGJQYpBi0GMQY1BjkGPgZCBkYGSgZOBlMGVwZbBl8GYwZoBmwGcAZ0BnkGfQaBBoUGigaOBpIGlwabBp8GpAaoBqwGsQa1BrkGvgbCBsYGywbPBtQG2AbcBuEG5QbqBu4G8gb3BvsHAAcEBwkHDQcSBxYHGwcfByQHKActBzEHNgc6Bz8HQwdIB00HUQdWB1oHXwdjB2gHbQdxB3YHewd/B4QHiQeNB5IHlwebB6AHpQepB64Hswe3B7wHwQfGB8oHzwfUB9kH3QfiB+cH7AfxB/UH+gf/CAQICQgNCBIIFwgcCCEIJggrCC8INAg5CD4IQwhICE0IUghXCFwIYQhmCGsIcAh1CHoIfwiECIkIjgiTCJgInQiiCKcIrAixCLYIuwjACMUIygjPCNQI2QjfCOQI6QjuCPMI+Aj9CQMJCAkNCRIJFwkdCSIJJwksCTEJNwk8CUEJRglMCVEJVglbCWEJZglrCXEJdgl7CYEJhgmLCZEJlgmbCaEJpgmrCbEJtgm8CcEJxgnMCdEJ1wncCeIJ5wntCfIJ+An9CgIKCAoNChMKGQoeCiQKKQovCjQKOgo/CkUKSgpQClYKWwphCmYKbApyCncKfQqDCogKjgqUCpkKnwql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ZmdmfWaSZqdmvWbSZuhm/WcSZyhnPWdTZ2hnfmeTZ6lnvmfUZ+ln/2gUaCpoP2hVaGpogGiWaKtowWjWaOxpAmkXaS1pQ2lYaW5phGmZaa9pxWnbafBqBmocajJqSGpdanNqiWqfarVqymrgavZrDGsiazhrTmtka3prkGuma7xr0mvoa/5sFGwqbEBsVmxsbIJsmGyubMRs2mzwbQZtHG0zbUltX211bYttoW24bc5t5G36bhFuJ249blNuam6AbpZurW7Dbtlu8G8GbxxvM29Jb2Bvdm+Mb6NvuW/Qb+Zv/XATcCpwQHBXcG1whHCacLFwx3DecPRxC3EicThxT3FmcXxxk3GqccBx13HucgRyG3IyckhyX3J2co1ypHK6ctFy6HL/cxZzLHNDc1pzcXOIc59ztnPNc+Rz+nQRdCh0P3RWdG10hHSbdLJ0yXTgdPd1DnUmdT11VHVrdYJ1mXWwdcd13nX2dg12JHY7dlJ2anaBdph2r3bHdt529XcMdyR3O3dSd2p3gXeYd7B3x3fed/Z4DXgleDx4VHhreIJ4mnixeMl44Hj4eQ95J3k+eVZ5bnmFeZ15tHnMeeN5+3oTeip6QnpaenF6iXqherh60HroewB7F3sve0d7X3t2e457pnu+e9Z77nwFfB18NXxNfGV8fXyVfK18xXzcfPR9DH0kfTx9VH1sfYR9nH20fc195X39fhV+LX5Ffl1+dX6NfqV+vn7Wfu5/Bn8efzd/T39nf39/l3+wf8h/4H/5gBGAKYBBgFqAcoCKgKOAu4DUgOyBBIEdgTWBToFmgX+Bl4GwgciB4YH5ghKCKoJDgluCdIKMgqWCvoLWgu+DB4MggzmDUYNqg4ODm4O0g82D5YP+hBeEMIRIhGGEeoSThKyExITdhPaFD4UohUGFWoVyhYuFpIW9hdaF74YIhiGGOoZThmyGhYaehreG0IbphwKHG4c0h02HZ4eAh5mHsofLh+SH/YgXiDCISYhiiHuIlYiuiMeI4Ij6iROJLIlGiV+JeImRiauJxIneifeKEIoqikOKXYp2io+KqYrCityK9YsPiyiLQotbi3WLjouoi8KL24v1jA6MKIxCjFuMdYyPjKiMwozcjPWND40pjUKNXI12jZCNqY3Djd2N944RjiuORI5ejniOko6sjsaO4I76jxOPLY9Hj2GPe4+Vj6+PyY/j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";

        public static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private struct LayerInfo
        {
            public int x;
            public int y;

            public LayerInfo(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public KraFileType()
            : base("KritaRaster", new FileTypeOptions
            {
                SupportsLayers = true,
                LoadExtensions = new string[] { ".kra" },
                //SaveExtensions = new string[] { ".kra" }
            })
        {
        }

        private unsafe static Bitmap getBitmapFromKraLayer(int xofs, int yofs, Stream inStream, int baseWidth, int baseHeight, string fileName)
        {
            Bitmap result = null;
            using (Bitmap bitmap = new Bitmap(baseWidth, baseHeight))
            {
                byte[] layerContent;
                using (var memoryStream = new MemoryStream())
                {
                    inStream.CopyTo(memoryStream);
                    layerContent = memoryStream.ToArray();
                }

                int currentIndex = 0;
                int version = ParseHeaderElement(layerContent, "VERSION ", ref currentIndex);
                int tileWidth = ParseHeaderElement(layerContent, "TILEWIDTH ", ref currentIndex);
                int tileHeight = ParseHeaderElement(layerContent, "TILEHEIGHT ", ref currentIndex);
                int pixelSize = ParseHeaderElement(layerContent, "PIXELSIZE ", ref currentIndex);
                int decompressedLength = pixelSize * tileWidth * tileHeight;
                int numberOfTiles = ParseHeaderElement(layerContent, "DATA ", ref currentIndex);

                for (int i = 0; i < numberOfTiles; i++)
                {
                    /* Now it is time to extract & decompress the data */
                    /* First the non-general element of the header needs to be extracted */
                    string headerString = GetHeaderElement(layerContent, ref currentIndex);
                    var match = System.Text.RegularExpressions.Regex.Match(headerString, "(-?\\d*),(-?\\d*),(\\w*),(\\d*)");

                    /* This header contains: */
                    /* 1. A number that defines the left position of the tile (CAN BE NEGATIVE!!!) */
                    /* 2. A number that defines the top position of the tile (CAN BE NEGATIVE!!!) */
                    /* 3. The string "LZF" which states the compression algorithm */
                    /* 4. A number that defines the number of bytes of the data that comes after this header. */

                    /* sm[0] is the full string match which we don't need */
                    /* The left position of the tile */
                    var left = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    /* The top position of the tile */
                    var top = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    /* We don't really care about match[3] since it is always 'LZF' */
                    /* The number of compressed bytes coming after this header */
                    var compressedLength = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    /* Put all the data in a vector */
                    byte[] dataVector = layerContent.Skip(currentIndex).Take(compressedLength).ToArray();
                    /* Allocate memory for the output */
                    byte[] output = new byte[decompressedLength];

                    /* Now... the first byte of this dataVector is actually an indicator of compression */
                    /* As follows: */
                    /* 0 . No compression, the data is actually raw! */
                    /* 1 . The data was compressed using LZF */
                    bool isLZF = IsLZF(dataVector);
                    if (isLZF)
                    {
                        LzffDecompress(dataVector, (uint)compressedLength, ref output, (uint)decompressedLength);
                    }
                    else
                    {
                        output = dataVector.Skip(1).ToArray();
                    }

                    /* TODO: Krita might also normalize the colors in some way */
                    /* This needs to be check and if present, the colors need to be denormalized */

                    /* Data is saved in following format: */
                    /* - Firstly all the RED data */
                    /* - Secondly all the GREEN data */
                    /* - Thirdly all the BLUE data */
                    /* - Fourthly all the ALPHA data */
                    /* This is different from the required ImageMagick format which requires quartets of: */
                    /* R1, G1, B1, A1, R2, G2, ... etc */
                    /* We'll just sort this here!*/
                    /* TODO: Sometimes there won't be any alpha channel when it is RGB instead of RGBA. */
                    
                    int tileArea = tileHeight * tileWidth;
                    for (int j = 0; j < tileArea; j++)
                    {
                        var w = j / (float)tileHeight;
                        var v = (int)MathF.Floor(w);
                        var u = (int)((w - v) * tileWidth);
                        var y = xofs + top + v;
                        var x = yofs + left + u;

                        // Paint.NET does not have anything outside these borders
                        if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height) continue;

                        var red = output[2 * tileArea + j];     //RED CHANNEL
                        var green = output[tileArea + j];     //GREEN CHANNEL
                        var blue = output[j];                //BLUE CHANNEL
                        var alpha = output[3 * tileArea + j]; //ALPHA CHANNEL
                        bitmap.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                    }
                    
                    /* Q: Why are the RED and BLUE channels swapped? */
                    /* A: I have no clue... that's how it is saved in the tile! */

                    /* Add the compressedLength to the currentIndex so the next tile starts at the correct position */
                    currentIndex += compressedLength;
                }

                result = (Bitmap)bitmap.Clone();
            }
            return result;
        }

        private static Color GetPixel(Bitmap bitmap, int x, int y)
        {
            if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height) return Color.Transparent;
            return bitmap.GetPixel(x, y);
        }

        /// <summary>
        /// Extract a header element and match it with the <paramref name="elementName"/>.
        /// </summary>
        private static int ParseHeaderElement(byte[] layerContent, string elementName, ref int currentIndex)
        {
            int elementIntValue = -1;
            /* First extract the header element */
            string elementValue = GetHeaderElement(layerContent, ref currentIndex);
            /* Try to match the elementValue string */
            if (elementValue.Contains(elementName))
            {
                int pos = elementValue.IndexOf(elementName);
                /* If found then erase it from string */
                elementValue = elementValue.Remove(pos, elementName.Length);
                elementIntValue = int.Parse(elementValue, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception($"WARNING: Missing header element in tile with name '{elementName}'\n");
            }
            return elementIntValue;
        }


        /// <summary>
        /// Extract first byte
        /// </summary>
        private static bool IsLZF(byte[] bytes)
        {
            return bytes.First() == 1;
        }

        /// <summary>
        /// Extract a header element starting from the <paramref name="currentIndex"/> until the next "0x0A".
        /// </summary>
        /// <returns>The header element</returns>
        private static string GetHeaderElement(byte[] layerContent, ref int currentIndex)
        {
            int startIndex = currentIndex;
            /* Just go through the vector until you encounter "0x0A" */
            while (layerContent.ElementAt(currentIndex) != (char)0x0A)
            {
                currentIndex++;
            }
            int endIndex = currentIndex;
            /* Extract this header element */
            byte[] elementContent = layerContent.Skip(startIndex).Take(endIndex - startIndex).ToArray();
            string elementValue = System.Text.Encoding.Default.GetString(elementContent);
            /* Increment the currentIndex pointer so that we skip the "0x0A" character */
            currentIndex++;

            return elementValue;
        }

        /// <summary>
        /// Decompresses the data using the Krita LZFF algorithm
        /// </summary>
        /// <param name="input">The data to decompress</param>
        /// <param name="inputLength">Length of the data to decompress</param>
        /// <param name="output">A buffer which will contain the decompressed data</param>
        /// <param name="outputLength">The size of the decompressed archive in the output buffer</param>
        /// <returns></returns>
        public static bool LzffDecompress(byte[] input, uint inputLength, ref byte[] output, uint outputLength)
        {
            outputLength = outputLength + 1;
            Array.Resize(ref output, (int)outputLength);
            int i = 0;
            int o = 0;

            while (i < inputLength)
            {
                uint control = input[i++];

                if (control < (1 << 5))
                {
                    var length = (int)(control + 1);

                    if (o + length > outputLength)
                    {
                        return false;
                    }

                    Array.Copy(input, i, output, o, length);
                    i += length;
                    o += length;
                }
                else
                {
                    var length = (int)(control >> 5);
                    var offset = (int)((control & 0x1F) << 8);

                    if (length == 7)
                    {
                        length += input[i++];
                    }
                    length += 2;

                    offset |= input[i++];

                    if (o + length > outputLength)
                    {
                        return false;
                    }

                    offset = o - 1 - offset;
                    if (offset < 0)
                    {
                        return false;
                    }

                    int block = Math.Min(length, o - offset);
                    Array.Copy(output, offset, output, o, block);
                    o += block;
                    offset += block;
                    length -= block;

                    while (length > 0)
                    {
                        output[o++] = output[offset++];
                        length--;
                    }
                }
            }

            output = output.Skip(1).ToArray();

            return true;
        }

        private struct Tile
        {
            public int left;
            public int top;
            public byte[] data;
        }

        private static void WriteLayer(Stream layerContent, Bitmap bitmap)
        {
            WriteHeaderElement(layerContent, "VERSION", 2);
            int tileWidth = 64;
            int tileHeight = 64;
            int pixelSize = 4;
            WriteHeaderElement(layerContent, "TILEWIDTH", tileWidth);
            WriteHeaderElement(layerContent, "TILEHEIGHT", tileHeight);
            WriteHeaderElement(layerContent, "PIXELSIZE", pixelSize);
            List<Tile> tiles = new List<Tile>();
            var w = ((int)MathF.Ceiling(bitmap.Width / (float)tileWidth));
            var h = ((int)MathF.Ceiling(bitmap.Height / (float)tileHeight));
            for (int x = 0; x < w; x++)
            {
                var left = x * tileWidth;
                for (int y = 0; y < h; y++)
                {
                    var top = y * tileHeight;
                    tiles.Add(new Tile
                    {
                        left = left,
                        top = top,
                        data = GetTileBytes(bitmap, left, top, tileWidth, tileHeight, pixelSize)
                    });
                }
            }
            WriteHeaderElement(layerContent, "DATA", tiles.Count);
            foreach (var tile in tiles)
            {
                byte[] output = new byte[0x20000];
                var compressedSize = LzffCompress(tile.data, tile.data.Length, output, 0x20000);
                WriteHeaderElement(layerContent, $"{tile.left},{tile.top},LZF,{compressedSize}");
                layerContent.Write(output, 0, compressedSize);
            }
        }

        private static byte[] GetTileBytes(Bitmap bitmap, int left, int top, int tileWidth, int tileHeight, int pixelSize)
        {
            int uncompressedLength = pixelSize * tileWidth * tileHeight;
            byte[] output = new byte[uncompressedLength];
            int tileArea = tileHeight * tileWidth;
            for (int j = 0; j < tileArea; j++)
            {
                var w = j / (float)tileHeight;
                var v = (int)MathF.Floor(w);
                var u = (int)((w - v) * tileWidth);
                var y = top + v;
                var x = left + u;

                Color color = GetPixel(bitmap, x, y);
                output[2 * tileArea + j] = color.R;     //RED CHANNEL
                output[tileArea + j] = color.G;     //GREEN CHANNEL
                output[j] = color.B;                //BLUE CHANNEL
                output[3 * tileArea + j] = color.A; //ALPHA CHANNEL
            }
            return new byte[1] { 1 }.Concat(output).ToArray();
        }

        private static void WriteHeaderElement(Stream layerContent, string elementName, int elementValue) => WriteHeaderElement(layerContent, elementName + " " + elementValue);

        private static void WriteHeaderElement(Stream layerContent, string element)
        {
            WriteString(layerContent, element, false);
            layerContent.WriteByte(0x0A);
        }

        private static void WriteString(Stream stream, string value, bool includeNewLine = true)
        {
            stream.Write(Encoding.Default.GetBytes(value));
            if (includeNewLine)
                WriteLine(stream);
        }


        private static void WriteLine(Stream stream)
            => stream.Write(Encoding.Default.GetBytes("\r\n"));


        /// <summary>
        /// Hashtable, that can be allocated only once
        /// </summary>
        private static readonly long[] HashTable = new long[HSIZE];

        private const uint HLOG = 14;
        private const uint HSIZE = (1 << 14);
        private const uint MAX_LIT = (1 << 5);
        private const uint MAX_OFF = (1 << 13);
        private const uint MAX_REF = ((1 << 8) + (1 << 3));

        /// <summary>
        /// Compresses the data using LibLZF algorithm
        /// </summary>
        /// <param name="input">Reference to the data to compress</param>
        /// <param name="inputLength">Length of the data to compress</param>
        /// <param name="output">Reference to a buffer which will contain the compressed data</param>
        /// <param name="outputLength">Length of the compression buffer (should be bigger than the input buffer)</param>
        /// <returns>The size of the compressed archive in the output buffer</returns>
        public static int LzffCompress(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            Array.Clear(HashTable, 0, (int)HSIZE);

            long hslot;
            uint iidx = 0;
            uint oidx = 0;
            long reference;

            uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); // FRST(in_data, iidx);
            long off;
            int lit = 0;

            for (; ; )
            {
                if (iidx < inputLength - 2)
                {
                    hval = (hval << 8) | input[iidx + 2];
                    hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
                    reference = HashTable[hslot];
                    HashTable[hslot] = (long)iidx;


                    if ((off = iidx - reference - 1) < MAX_OFF
                        && iidx + 4 < inputLength
                        && reference > 0
                        && input[reference + 0] == input[iidx + 0]
                        && input[reference + 1] == input[iidx + 1]
                        && input[reference + 2] == input[iidx + 2]
                        )
                    {
                        /* match found at *reference++ */
                        uint len = 2;
                        uint maxlen = (uint)inputLength - iidx - len;
                        maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

                        if (oidx + lit + 1 + 3 >= outputLength)
                        {
                            return 0;
                        }

                        do
                        {
                            len++;
                        }
                        while (len < maxlen && input[reference + len] == input[iidx + len]);

                        if (lit != 0)
                        {
                            output[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do
                            {
                                output[oidx++] = input[iidx + lit];
                            }
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;

                        if (len < 7)
                        {
                            output[oidx++] = (byte)((off >> 8) + (len << 5));
                        }
                        else
                        {
                            output[oidx++] = (byte)((off >> 8) + (7 << 5));
                            output[oidx++] = (byte)(len - 7);
                        }

                        output[oidx++] = (byte)off;

                        iidx += len - 1;
                        hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;
                        continue;
                    }
                }
                else if (iidx == inputLength)
                {
                    break;
                }

                /* one more literal byte we must copy */
                lit++;
                iidx++;

                if (lit == MAX_LIT)
                {
                    if (oidx + 1 + MAX_LIT >= outputLength)
                    {
                        return 0;
                    }

                    output[oidx++] = (byte)(MAX_LIT - 1);
                    lit = -lit;
                    do
                    {
                        output[oidx++] = input[iidx + lit];
                    }
                    while ((++lit) != 0);
                }
            }

            if (lit != 0)
            {
                if (oidx + lit + 1 >= outputLength)
                {
                    return 0;
                }

                output[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do
                {
                    output[oidx++] = input[iidx + lit];
                }
                while ((++lit) != 0);
            }

            return (int)oidx;
        }

        public enum KritaNodeType
        {
            paintlayer,
            vectorlayer,
            grouplayer,
            referenceimages
        }

        protected override Document OnLoad(Stream input)
        {
            Document result;
            using (ZipArchive zipArchive = new ZipArchive(input, ZipArchiveMode.Read))
            {
                try
                {
                    string text;
                    using (StreamReader streamReader = new StreamReader(zipArchive.GetEntry("mimetype").Open()))
                    {
                        text = streamReader.ReadToEnd();
                    }
                    if (!text.Equals("application/x-krita", StringComparison.Ordinal))
                    {
                        throw new FormatException("Incorrect mimetype: " + text);
                    }
                }
                catch (NullReferenceException)
                {
                    throw new FormatException("No mimetype found in OpenRaster file");
                }
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(zipArchive.GetEntry("maindoc.xml").Open());
                XmlElement documentElement = (XmlElement)xmlDocument.DocumentElement.GetElementsByTagName("IMAGE")[0];
                int width = int.Parse(documentElement.GetAttribute("width"), CultureInfo.InvariantCulture);
                int height = int.Parse(documentElement.GetAttribute("height"), CultureInfo.InvariantCulture);
                string name = documentElement.GetAttribute("name");
                Document document = new Document(width, height)
                {
                    DpuUnit = MeasurementUnit.Inch,
                    DpuX = double.Parse(KraFileType.getAttribute(documentElement, "x-res", "72"), CultureInfo.InvariantCulture),
                    DpuY = double.Parse(KraFileType.getAttribute(documentElement, "y-res", "72"), CultureInfo.InvariantCulture)
                };
                XmlNodeList elementsByTagName = ((XmlElement)xmlDocument.GetElementsByTagName("layers")[0]).GetElementsByTagName("layer");
                if (elementsByTagName.Count == 0)
                {
                    throw new FormatException("No layers found in KritaRaster file");
                }
                int reverseCount = elementsByTagName.Count - 1;
                for (int i = reverseCount; i >= 0; i--)
                {
                    XmlElement xmlElement = (XmlElement)elementsByTagName[i];
                    int layerNumber = reverseCount - i;
                    KritaNodeType nodeType;
                    if (!Enum.TryParse<KritaNodeType>(KraFileType.getAttribute(xmlElement, "nodetype", "paintlayer"), true, out nodeType))
                    {
                        nodeType = KritaNodeType.paintlayer;
                    }
                    if (nodeType == KritaNodeType.paintlayer)
                    {
                        string layerName = KraFileType.getAttribute(xmlElement, "name", string.Format("Layer {0}", layerNumber));
                        string filename = KraFileType.getAttribute(xmlElement, "filename", string.Format("layer{0}", layerNumber));
                        int xofs = int.Parse(KraFileType.getAttribute(xmlElement, "x", "0"), CultureInfo.InvariantCulture);
                        int yofs = int.Parse(KraFileType.getAttribute(xmlElement, "y", "0"), CultureInfo.InvariantCulture);
                        byte opacity = byte.Parse(KraFileType.getAttribute(xmlElement, "opacity", "255"), CultureInfo.InvariantCulture);
                        bool visible = KraFileType.getAttribute(xmlElement, "visible", "1") == "1";
                        using (Stream stream = zipArchive.GetEntry(name + "/layers/" + filename).Open())
                        {
                            using (Bitmap bitmapFromOraLayer = KraFileType.getBitmapFromKraLayer(xofs, yofs, stream, width, height, filename))
                            {
                                BitmapLayer bitmapLayer = null;
                                if (i == reverseCount)
                                {
                                    bitmapLayer = Layer.CreateBackgroundLayer(width, height);
                                }
                                else
                                {
                                    bitmapLayer = new BitmapLayer(width, height);
                                }
                                bitmapLayer.Name = layerName;
                                bitmapLayer.Opacity = opacity;
                                bitmapLayer.Visible = visible;
                                string compositeop = KraFileType.getAttribute(xmlElement, "compositeop", "normal");
                                try
                                {
                                    bitmapLayer.BlendMode = (LayerBlendMode)Enum.Parse(typeof(LayerBlendMode), compositeop, true);
                                }
                                catch
                                {
                                    switch (compositeop)
                                    {
                                        case "add":
                                            bitmapLayer.BlendMode = LayerBlendMode.Additive;
                                            break;
                                        case "burn":
                                            bitmapLayer.BlendMode = LayerBlendMode.ColorBurn;
                                            break;
                                        case "dodge":
                                            bitmapLayer.BlendMode = LayerBlendMode.ColorDodge;
                                            break;
                                        default:
                                            bitmapLayer.BlendMode = LayerBlendMode.Normal;
                                            break;
                                    }
                                }
                                bitmapLayer.Surface.CopyFromGdipBitmap(bitmapFromOraLayer, false);
                                document.Layers.Insert(layerNumber, bitmapLayer);
                            }
                        }
                    }
                }
                result = document;
            }
            return result;
        }

        protected unsafe override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
        {
            if (input == null || output == null)
            {
                throw new ArgumentNullException("Null error");
            }
            using (ZipArchive zipArchive = new ZipArchive(output, ZipArchiveMode.Update, true))
            {
                LayerInfo[] array2 = new LayerInfo[input.Layers.Count];
                for (int i = 0; i < input.Layers.Count; i++)
                {
                    BitmapLayer bitmapLayer = (BitmapLayer)input.Layers[i];
                    Rectangle bounds = bitmapLayer.Surface.Bounds;
                    int width = bitmapLayer.Width;
                    int height = bitmapLayer.Height;
                    using (Stream layer = zipArchive.CreateEntry("root/layers/layer" + i.ToString(CultureInfo.InvariantCulture)).Open())
                    {
                        var aliasedBitmap = bitmapLayer.Surface.CreateAliasedBitmap(bounds, true);
                        WriteLayer(layer, aliasedBitmap);
                    }
                    using (Stream defaultPixel = zipArchive.CreateEntry("root/layers/layer" + i.ToString(CultureInfo.InvariantCulture) + ".defaultpixel").Open())
                    {
                        byte[] defaultPixelArray = Convert.FromBase64String(this.DefaultPixel);
                        defaultPixel.Write(defaultPixelArray, 0, defaultPixelArray.Length);
                    }
                    using (Stream icc = zipArchive.CreateEntry("root/layers/layer" + i.ToString(CultureInfo.InvariantCulture) + ".icc").Open())
                    {
                        byte[] iccArray = Convert.FromBase64String(this.ICC);
                        icc.Write(iccArray, 0, iccArray.Length);
                    }
                }
                using (Stream icc = zipArchive.CreateEntry("root/annotations/icc").Open())
                {
                    byte[] iccArray = Convert.FromBase64String(this.ICC);
                    icc.Write(iccArray, 0, iccArray.Length);
                }
                using (Stream animation = zipArchive.CreateEntry("root/animation/index.xml").Open())
                {
                    byte[] animationArray = getAnimationIndexData();
                    animation.Write(animationArray, 0, animationArray.Length);
                }
                byte[] array = Convert.FromBase64String(this.MimeType);
                output.Write(array, 0, array.Length);
                using (Stream stream4 = zipArchive.CreateEntry("maindoc.xml").Open())
                {
                    double dpiX;
                    double dpiY;
                    switch (input.DpuUnit)
                    {
                        case MeasurementUnit.Pixel:
                            dpiX = Document.GetDefaultDpu(MeasurementUnit.Inch);
                            dpiY = Document.GetDefaultDpu(MeasurementUnit.Inch);
                            break;
                        case MeasurementUnit.Inch:
                            dpiX = input.DpuX;
                            dpiY = input.DpuY;
                            break;
                        case MeasurementUnit.Centimeter:
                            dpiX = Document.DotsPerCmToDotsPerInch(input.DpuX);
                            dpiY = Document.DotsPerCmToDotsPerInch(input.DpuY);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("Invalid measurement unit.");
                    }
                    array = getLayerXmlData(input.Layers, array2, dpiX, dpiY);
                    stream4.Write(array, 0, array.Length);
                }
                using (Stream info = zipArchive.CreateEntry("documentinfo.xml").Open())
                {
                    var infoArray = getInfoXmlData();
                    info.Write(infoArray, 0, infoArray.Length);
                }
                using (Stream mimetype = zipArchive.CreateEntry("mimetype").Open())
                {
                    byte[] mimetypeArray = Convert.FromBase64String(this.MimeType);
                    mimetype.Write(mimetypeArray, 0, mimetypeArray.Length);
                }
                using (Surface surface = new Surface(input.Width, input.Height))
                {
                    input.Flatten(surface);
                    using (MemoryStream memoryStream2 = new MemoryStream())
                    {
                        surface.CreateAliasedBitmap().Save(memoryStream2, ImageFormat.Png);
                        array = memoryStream2.ToArray();
                    }
                    using (Stream stream5 = zipArchive.CreateEntry("mergedimage.png").Open())
                    {
                        stream5.Write(array, 0, array.Length);
                    }
                    using (Surface surface2 = new Surface(getThumbDimensions(input.Width, input.Height)))
                    {
                        surface2.FitSurface(ResamplingAlgorithm.SuperSampling, surface);
                        using (MemoryStream memoryStream3 = new MemoryStream())
                        {
                            surface2.CreateAliasedBitmap().Save(memoryStream3, ImageFormat.Png);
                            array = memoryStream3.ToArray();
                        }
                    }
                    using (Stream stream6 = zipArchive.CreateEntry("preview.png").Open())
                    {
                        stream6.Write(array, 0, array.Length);
                    }
                }
            }
        }

        private static byte[] getLayerXmlData(LayerList layers, KraFileType.LayerInfo[] info, double dpiX, double dpiY)
        {
            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseOutput = false
                };
                XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
                xmlWriter.WriteStartDocument();
                Xmlns(xmlWriter, "DOC");
                xmlWriter.WriteAttributeString("width", layers.GetAt(0).Width.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("syntaxVersion", "2.0");
                xmlWriter.WriteAttributeString("kritaVersion", "5.0.2");
                xmlWriter.WriteAttributeString("editor", "Krita");
                xmlWriter.WriteStartElement("IMAGE");
                xmlWriter.WriteAttributeString("width", layers.GetAt(0).Width.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("height", layers.GetAt(0).Height.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("version", "0.0.3");
                xmlWriter.WriteAttributeString("x-res", dpiX.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("y-res", dpiY.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("name", "root");
                xmlWriter.WriteAttributeString("mime", "application/x-kra");
                xmlWriter.WriteAttributeString("colorspacename", "RGBA");
                xmlWriter.WriteAttributeString("profile", "sRGB-elle-V2-srgbtrc.icc");
                xmlWriter.WriteStartElement("layers");
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    BitmapLayer bitmapLayer = (BitmapLayer)layers[i];
                    xmlWriter.WriteStartElement("layer");
                    xmlWriter.WriteAttributeString("nodetype", "paintlayer");
                    xmlWriter.WriteAttributeString("name", bitmapLayer.Name);
                    xmlWriter.WriteAttributeString("uuid", "{" + Guid.NewGuid().ToString().ToLower() + "}");
                    xmlWriter.WriteAttributeString("opacity", bitmapLayer.Opacity.ToString(CultureInfo.InvariantCulture));
                    xmlWriter.WriteAttributeString("filename", "layer" + i.ToString(CultureInfo.InvariantCulture));
                    xmlWriter.WriteAttributeString("visible", bitmapLayer.Visible ? "1" : "0");
                    xmlWriter.WriteAttributeString("x", info[i].x.ToString(CultureInfo.InvariantCulture));
                    xmlWriter.WriteAttributeString("y", info[i].y.ToString(CultureInfo.InvariantCulture));
                    xmlWriter.WriteAttributeString("colorspacename", "RGBA");
                    xmlWriter.WriteAttributeString("channelflags", "");
                    xmlWriter.WriteAttributeString("collapsed", "0");
                    xmlWriter.WriteAttributeString("locked", "0");
                    xmlWriter.WriteAttributeString("intimeline", "1");
                    xmlWriter.WriteAttributeString("onionskin", "0");
                    xmlWriter.WriteAttributeString("colorlabel", "0");
                    xmlWriter.WriteAttributeString("channellockflags", "1111");
                    if (bitmapLayer.IsBackground)
                    {
                        xmlWriter.WriteAttributeString("selected", "true");
                    }
                    try
                    {
                        var blendMode = bitmapLayer.BlendMode;
                        var compositeOP = blendMode.ToString().ToLower();
                        if (blendMode == LayerBlendMode.Additive) compositeOP = "add";
                        else if(blendMode == LayerBlendMode.ColorDodge) compositeOP = "dodge";
                        else if (blendMode == LayerBlendMode.ColorBurn) compositeOP = "burn";
                        else if (blendMode == LayerBlendMode.Difference) compositeOP = "normal";
                        xmlWriter.WriteAttributeString("compositeop", compositeOP);
                    }
                    catch
                    {
                        xmlWriter.WriteAttributeString("compositeop", "normal");
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                AttributedElement(xmlWriter, "ProjectionBackgroundColor", "ColorData", "AAAAAA==");
                AttributedElement(xmlWriter, "GlobalAssistantsColor", "SimpleColorData", "176,176,176,255");
                EmptyElement(xmlWriter, "Palettes");
                EmptyElement(xmlWriter, "resources");
                xmlWriter.WriteStartElement("animation");
                AnimationElement(xmlWriter, false);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                result = memoryStream.ToArray();
            }
            return result;
        }

        private static byte[] getAnimationIndexData()
        {
            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseOutput = false
                };

                XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
                xmlWriter.WriteStartDocument();
                Xmlns(xmlWriter, "animation-metadata");
                AnimationElement(xmlWriter, true);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                result = memoryStream.ToArray();
            }
            return result;
        }

        private static void AnimationElement(XmlWriter xmlWriter, bool index)
        {
            xmlWriter.WriteStartElement("framerate");
            xmlWriter.WriteAttributeString("type", "value");
            xmlWriter.WriteAttributeString("value", "24");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("range");
            xmlWriter.WriteAttributeString("type", "timerange");
            xmlWriter.WriteAttributeString("from", "0");
            xmlWriter.WriteAttributeString("to", "100");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("currentTime");
            xmlWriter.WriteAttributeString("type", "value");
            xmlWriter.WriteAttributeString("value", "0");
            xmlWriter.WriteEndElement();
            if (index)
            {
                xmlWriter.WriteStartElement("export-settings");
                xmlWriter.WriteStartElement("sequenceFilePath");
                xmlWriter.WriteAttributeString("type", "value");
                xmlWriter.WriteAttributeString("value", "");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("sequenceBaseName");
                xmlWriter.WriteAttributeString("type", "value");
                xmlWriter.WriteAttributeString("value", "");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("sequenceInitialFrameNumber");
                xmlWriter.WriteAttributeString("type", "value");
                xmlWriter.WriteAttributeString("value", "-1");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }

        private static void Xmlns(XmlWriter xmlWriter, string name, string link = "http://www.calligra.org/DTD/krita")
        {
            xmlWriter.WriteStartElement(name, link);
            xmlWriter.WriteStartAttribute("xmlns", null, null);
            xmlWriter.WriteRaw(link);
            xmlWriter.WriteEndAttribute();
        }


        private static byte[] getInfoXmlData()
        {
            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseOutput = false
                };
                XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
                xmlWriter.WriteStartDocument();
                Xmlns(xmlWriter, "document-info", "http://www.calligra.org/DTD/document-info");
                xmlWriter.WriteStartElement("about");
                ValuedElement(xmlWriter, "title", "root");
                EmptyElement(xmlWriter, "description");
                EmptyElement(xmlWriter, "subject");
                ValuedElement(xmlWriter, "abstract", "<![CDATA[]]>");
                EmptyElement(xmlWriter, "keyword");
                ValuedElement(xmlWriter, "initial-creator", "Unknown");
                ValuedElement(xmlWriter, "editing-cycles", "1");
                ValuedElement(xmlWriter, "editing-time", "1");
                ValuedElement(xmlWriter, "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
                ValuedElement(xmlWriter, "creation-date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
                EmptyElement(xmlWriter, "language");
                EmptyElement(xmlWriter, "license");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("author");
                EmptyElement(xmlWriter, "full-name");
                EmptyElement(xmlWriter, "creator-first-name");
                EmptyElement(xmlWriter, "creator-last-name");
                EmptyElement(xmlWriter, "initial");
                EmptyElement(xmlWriter, "author-title");
                EmptyElement(xmlWriter, "position");
                EmptyElement(xmlWriter, "company");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
                result = memoryStream.ToArray();
            }
            return result;
        }

        private static void ValuedElement(XmlWriter xmlWriter, string name, string value)
        {
            xmlWriter.WriteStartElement(name);
            xmlWriter.WriteValue(value);
            xmlWriter.WriteEndElement();
        }

        private static void AttributedElement(XmlWriter xmlWriter, string name, string attribute, string value)
        {
            xmlWriter.WriteStartElement(name);
            xmlWriter.WriteAttributeString(attribute, value);
            xmlWriter.WriteEndElement();
        }

        private static void EmptyElement(XmlWriter xmlWriter, string name)
        {
            xmlWriter.WriteStartElement(name);
            xmlWriter.WriteEndElement();
        }

        private static Size getThumbDimensions(int width, int height)
        {
            if (width <= ThumbMaxSize && height <= ThumbMaxSize)
            {
                return new Size(width, height);
            }
            if (width <= height)
            {
                return new Size((int)((double)width / (double)height * ThumbMaxSize), ThumbMaxSize);
            }
            return new Size(ThumbMaxSize, (int)((double)height / (double)width * ThumbMaxSize));
        }

        private static string getAttribute(XmlElement element, string attribute, string defValue)
        {
            string attribute2 = element.GetAttribute(attribute);
            if (!string.IsNullOrEmpty(attribute2))
            {
                return attribute2;
            }
            return defValue;
        }
    }
}
