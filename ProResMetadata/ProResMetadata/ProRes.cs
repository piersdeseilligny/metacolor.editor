using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static ProResMetadata.Toolkit;
using System.Linq;

namespace ProResMetadata
{
       
    public static class ProRes
    {
        public static event EventHandler<float> AnalysisProgress;
        
        #region Dictionaries
        public static string GetValue(this Dictionary<int, string> dic, int value)
        {
            if (dic.ContainsKey(value)) return dic[value];
            else return "Invalid (" + value.ToString() + ")";
        }
        public static void AddValue(this Dictionary<int, List<int>> keyValues, int value, int frameloopcount)
        {
            if (keyValues.ContainsKey(value))
                keyValues[value].Add(frameloopcount);
            else
                keyValues.Add(value, new List<int>() { frameloopcount });
        }
        public static void AddValue(this Dictionary<string, List<int>> keyValues, string value, int frameloopcount)
        {
            if (keyValues.ContainsKey(value))
                keyValues[value].Add(frameloopcount);
            else
                keyValues.Add(value, new List<int>() { frameloopcount });
        }
        public static void AddValue(this Dictionary<byte[], List<int>> keyValues, byte[] value, int frameloopcount)
        {
            if (keyValues.ContainsKey(value))
                keyValues[value].Add(frameloopcount);
            else
                keyValues.Add(value, new List<int>() { frameloopcount });
        }
        public static Dictionary<int, string> SourcePixelFormats = new Dictionary<int, string>()
        {
            {0,"Unkown" },
            {1,"2vuy - 8-bit 4:2:2" },
            {2,"v210 - 10-bit 4:2:2" },
            {3,"v216 - 10/12/14/16-bit 4:2:2" },
            {4, "r408 - 8-bit 4:4:4:4 with alpha" },
            {5, "v408 - 8-bit 4:4:4:4 with alpha + super black" },
            {6, "r4fl - 32-bit floating point 4:4:4:4" },
            {7, "0x20 - 8-bit RGB" },
            {8, "BGRA - 8-bit RGB with alpha" },
            {9, "n302 - Undocumented" },
            {10, "b64a - 16-bit ARGB" },
            {11, "R10k - AJA 10-bit RGB" }
        };
        public static Dictionary<int, string> AlphaInfo = new Dictionary<int, string>()
        {
            {0,"No alpha" },
            {1,"8-bit Alpha" },
            {2,"16-bit Alpha" },
        };
        public static Dictionary<int, string> ColorPrimaries = new Dictionary<int, string>()
        {
            {0,"Reserved (0)" },
            {1,"ITU-R BT.709" },
            {2,"Unspecified (2)" },
            {3,"Reserved (3)" },
            {4, "ITU-R BT.470M" },
            {5, "ITU-R BT.470BG" },
            {6, "SMPTE 170M" },
            {7, "SMPTE 240M" },
            {8, "FILM" },
            {9, "ITU-R BT.2020" },
            {10, "SMPTE ST 428-1" },
            {11, "DCI P3" },
            {12, "P3 D65" }
        };
        public static Dictionary<int, string> ColorMatrixes = new Dictionary<int, string>()
        {
            {0,"GBR" },
            {1,"ITU-R BT.709" },
            {2,"Unspecified (2)" },
            {3,"Reserved (3)" },
            {4, "FCC" },
            {5, "BT470BG" },
            {6, "SMPTE 170M" },
            {7, "SMPTE 240M" },
            {8, "YCOCG" },
            {9, "BT2020 Non-constant Luminance" },
            {10, "BT2020 Constant Luminance" }
        };
        public static Dictionary<int, string> TransferFunctions = new Dictionary<int, string>()
        {
            {0,"Reserved (0)" },
            {1,"ITU-R BT.709" },
            {2,"Unspecified (2)" },
            {3,"Reserved (3)" },
            {4, "Gamma 2.2 curve" },
            {5, "Gamma 2.8 curve" },
            {6, "SMPTE 170M" },
            {7, "SMPTE 240M" },
            {8, "Linear" },
            {9, "Log" },
            {10, "Log Sqrt" },
            {11, "IEC 61966-2-4" },
            {12, "ITU-R BT.1361 Extended Color Gamut" },
            {13, "IEC 61966-2-1" },
            {14, "ITU-R BT.2020 10 bit" },
            {15, "ITU-R BT.2020 12 bit" },
            {16, "SMPTE ST 2084 (PQ)" },
            {17, "SMPTE ST 428-1" },
            {18, "ARIB STD-B67 (HLG)" }
        };
        public static Dictionary<string, string> CreatorIDs = new Dictionary<string, string>()
        {
            {"apl0","Apple, Inc. (apl0)" },
            {"atf0","The Foundry (atf0)" },
            {"aja0","AJA Kona hardware (aja0)" },
            {"arri","Arnold & Richter Cine Technik (arri)" },
            {"bmd0","Blackmagic Design (bmd0)" },
            {"fmpg","FFmpeg (fmpg)" },
            {"custom","Custom" },
        };
        public static Dictionary<int, string> FrameTypes = new Dictionary<int, string>()
        {
            {0,"Progressive" },
            {1,"Interlaced (top field first)" },
            {2,"Interlaced (bottom field first)" },
        };
        public static Dictionary<int, string> ChrominanceFactors = new Dictionary<int, string>()
        {
            {2,"422" },
            {3,"444" }
        };
        #endregion

        #region Class declarations
        public class Frame
        {
            public int FrameSize { get; set; }
            public int HeaderSize { get; set; }
            public long Offset { get; set; }
            public int Version { get; set; }
            public string CreatorID { get; set; }
            public int FrameWidth { get; set; }
            public int FrameHeight { get; set; }
            public int PictureFormat { get;set; }
            public int FrameType { get; set; }
            public int Primaries { get; set; }
            public int TransferFunction { get; set; }
            public int ColorMatrix { get; set; }
            public int SourcePixelFormat { get; set; }
            public int AlphaInfo { get; set; }
            public bool CustomLumaQuant { get; set; }
            public bool CustomChromaQuant { get; set; }
            public byte[] QMatLuma { get; set; }
            public byte[] QMatChroma { get; set; }
            public byte IgnoredFirst { get; set; }
            public byte IgnoredSecond { get; set; }
            public int ChrominanceFactor { get; set; }
        }
        public enum ColorParameter { Video, Print, Unknown };
        public class ColorAtom
        {
            public int PrimariesIndex { get; set; }
            public int TransferFunction { get; set; }
            public int ColorMatrix { get; set; }
            public long Offset { get; set; }
            public ColorParameter Type { get; set; }
            public string RawColorParam { get; set; }
            public int Size { get; set; }
        }
        public class DecodeError
        {
            public DecodeError(string title, string details, ErrorGravity gravity)
            {
                Title = title;
                Details = details;
                Gravity = gravity;
            }
            public enum ErrorGravity { Critical = 3, Default = 2, Informational = 1, IncompatibleFile = 4, None = 0}
            public string Title { get; set; }
            public string Details { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public ErrorGravity Gravity { get; set; }
        }
        public class VideoElement
        {
            public string Container { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public Dictionary<int, List<int>> ColorPrimary { get; set; }
            public Dictionary<int, List<int>> TransferFunction { get; set; }
            public Dictionary<int, List<int>> ColorMatrix { get; set; }
            public Dictionary<int, List<int>> PixelFormat { get; set; }
            public Dictionary<int, List<int>> AlphaInfo { get; set; }
            public Dictionary<int, List<int>> ChrominanceFactor { get; set; }
            public Dictionary<string, List<int>> CreatorID { get; set; }
            public Dictionary<string, List<int>> Resolutions { get; set; }
            public Dictionary<int, List<int>> PictureType { get; set; }
            public Dictionary<byte[], List<int>> ChromaQuants { get; set; }
            public Dictionary<byte[], List<int>> LumaQuants { get; set; }
            
            public string DisplayColorPrimary = "";
            public string DisplayTransferFunction = "";
            public string DisplayColorMatrix ="";
            public string DisplayPixelFormat = "";
            public string DisplayAlphaInfo = "";
            public string DisplayCreatorId = "";
            public string DisplayResolution ="";
            public string DisplayChrominanceFactor;
            public string DisplayPictureType { get; set; }
            public List<Frame> Frames { get; set; }
            public long FrameCount { get; set; }
            public List<DecodeError> Errors { get; set; }
            public ColorAtom ColorAtom { get; set; }
            public DecodeError.ErrorGravity WorstError = DecodeError.ErrorGravity.None;
            public void AddError(string title, string details, DecodeError.ErrorGravity gravity)
            {
                if (gravity > WorstError) WorstError = gravity;
                Errors.Add(new DecodeError(title, details, gravity));
            }
        }
        #endregion

        /// <summary>
        /// Create a byte array corresponding to a quicktime color atom
        /// </summary>
        /// <param name="colorPrimary">The color primary</param>
        /// <param name="transferFunction">The transfer function</param>
        /// <param name="colorMatrix">The color matrix</param>
        /// <returns></returns>
        private static byte[] CreateColorAtom(int colorPrimary, int transferFunction, int colorMatrix)
        {
            var cpArray = Convert.ToInt16(colorPrimary).ToBigEndian();
            var tfArray = Convert.ToInt16(transferFunction).ToBigEndian();
            var cmArray = Convert.ToInt16(colorMatrix).ToBigEndian();
            return new byte[]
            {
                0x00, 0x00, 0x00, 0x12, // Size (18)
                0x63, 0x6f, 0x6c, 0x72, // c o l r
                0x6e, 0x63, 0x6c, 0x63, // n c l c
                cpArray[0], cpArray[1], // Color primary
                tfArray[0], tfArray[1], // Transfer function
                cmArray[0], cmArray[1]  // Color matrix
            };
        }

        private static int[] ColorAtomHeader = new int[] { 0x63, 0x6F, 0x6C, 0x72 }; // "colr"
        // QUICKTIME COLOR ATOM:
        //
        // |   |   |   |   | c | o | l | r |   |   |   |   |  |  |  |  |  |  |
        // |   |   |   |   |   |   |   |   |   |   |   |   |  |  |  |  |  |  |
        // |               |               |               |     |     |     |
        // |       4       |       4       |       4       |  2  |  2  |  2  |
        // |               |               |               |     |     |     |
        // | size          | type          | color param   |primaries  |     |
        // |               |               |               |     |transfer func
        // |               |               |               |     |     |color matrix
        // |               |               |               |     |     |     |
        // |               |               |               |     |     |     |
        // |               |               |               |     |     |     |
        // |               |               |               |     |     |     |
        // 
        // All values are in big-endian
        // Color param can be "nclc" for video or "prof" for print

        private static int[] FrameHeader = new int[] { 0x69, 0x63, 0x70, 0x66 }; // "icpf"
        // PRORES HEADER:
        //
        // |   |   |   |   | i | c | p | f |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |||||....|||||||||....|||||
        // |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |||||....|||||||||....|||||
        // |               |               |       |       |               |       |       |   |   |   |   |   | | |   |   |   |   |            |            |
        // |       4       |       4       |   2   |   2   |       4       |   2   |   2   | 1 | 1 | 1 | 1 | 1 | | | 1 | 1 | 1 | 1 |     64     |     64     |
        // |               |               |       |       |               |       |       |   |xxx|   |   |   | | |xxx|   |   |   |            |            |
        // | size          | type          | hsize | ver   | creator ID    | width |height |frameflags |   |color matrix   |   |   |            |            |
        // |               |               |       |       |               |       |       |   |   |primaries  | | |   |QMatFlags  |  QMatLuma  | QMatChroma |
        // |               |               |       |       |               |       |       |   |   |   |transfer func  |   |   |   |            |            |
        // |               |               |       |       |               |       |       |   |   |   |   |   | | |   |   |   |   |            |            |
        // |               |               |       |       |               |       |       |   |   |   |   |   |source px  |   |   |            |            |
        // |               |               |       |       |               |       |       |   |   |   |   |   | |alpha|   |   |   |            |            |
        // |               |               |       |       |               |       |       |   |   |   |   |   | | |   |   |   |   |            |            |
        //        
        // All values are in big-endian
        // |xxx| = ignored

        /// <summary>
        /// Read the specified amount of bytes from the specified stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="length">How many bytes to read</param>
        /// <returns></returns>
        public static byte[] ReadBytes(Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            int position = 0;
            while(true)
            {
                buffer[position] = Convert.ToByte(stream.ReadByte());
                position++;

                if (position+1 > length) return buffer;
            }
        }

        /// <summary>
        /// Create a user-friendly string describing the occurence of the videos, and add any conflict errors to the video
        /// </summary>
        /// <param name="keyValues">A dictionary specifying the key of the property, and a list of every frame it occurs in</param>
        /// <param name="displayValues">The dictionary corresponding to the property</param>
        /// <param name="video">The video </param>
        /// <param name="errorName">The name of the property to be used in the error messages</param>
        /// <param name="gravityIfMultiple">The gravity of the errror if multiple occurences are detected</param>
        /// <param name="atomValue">The value of this property in the quicktime atom</param>
        /// <returns></returns>
        private static string HumanizeValues(Dictionary<int, List<int>> keyValues, ref Dictionary<int,string> displayValues, ref VideoElement video, string errorName, DecodeError.ErrorGravity gravityIfMultiple, int atomValue)
        {
            if (keyValues.Count == 0)
                return "N/A"; //There
            if (keyValues.Count == 1)
            {
                var firstpair = keyValues.Keys.First();
                if (firstpair != atomValue && atomValue != -1 && firstpair != 2)
                {
                    video.AddError("Conflicting " + errorName, "The QuickTime Color header defines the " + errorName + " as " + displayValues.GetValue(atomValue) + ", but the ProRes frames define it as " + displayValues.GetValue(firstpair), DecodeError.ErrorGravity.Critical);
                }
                return displayValues.GetValue(keyValues.Keys.First());
            }
            else
            {
                List<string> values = new List<string>();
                int divergingvalues = 0;
                int totalvalues = 0;
                foreach (var value in keyValues.Keys)
                {
                    values.Add(displayValues.GetValue(value));
                    totalvalues += keyValues[value].Count;
                    if (atomValue != -1 && atomValue != value && value != 2)
                        divergingvalues += keyValues[value].Count;
                }
                if(atomValue != -1)
                {
                    if (divergingvalues == totalvalues)
                        video.AddError("Conflicting " + errorName + " (All)", "The QuickTime Color header defines the " + errorName + " as " + displayValues.GetValue(atomValue) + ", but the ProRes frames define it as multiple other values, none of which are the same", DecodeError.ErrorGravity.Critical);
                    else if (divergingvalues < totalvalues/2)
                        video.AddError("Conflicting " + errorName + " (Minority)", "The QuickTime color header defines the " + errorName + " as " + displayValues.GetValue(atomValue) + ", but some ProRes frames define it differently (" + divergingvalues + ")", DecodeError.ErrorGravity.Critical);
                    else
                        video.AddError("Conflicting " + errorName + " (Majority)", "The QuickTime color header defines the " + errorName + " as " + displayValues.GetValue(atomValue) + ", but the majority of ProRes frames define it differently (" + divergingvalues + ")", DecodeError.ErrorGravity.Critical);     
                }
                string jointElements = string.Join(", ", values);
                video.AddError("More than one " + errorName, jointElements, gravityIfMultiple);
                return "Multiple (" + jointElements + ")";
            }
        }

        public static byte[] nullReplacement = new byte[] { 0x00 }; //This is the byte array used for representing a null custom luma/chroma quanitzation matrix

        /// <summary>
        /// A counter for measuring the <see cref="feedbackOccurence"/>
        /// </summary>
        private static int feedbackCounter = 0;

        /// <summary>
        /// How often (in bytes analyzed) feedback is provided via <see cref="AnalysisProgress"/>
        /// </summary>
        private static int feedbackOccurence = 1000000; //Every megabyte

        /// <summary>
        /// Decode the useful prores-related data from the file
        /// </summary>
        /// <param name="filepath">The full path of the file to analyze</param>
        /// <returns></returns>
        public static VideoElement Decode(string filepath, bool optimizations = true)
        {
            Stopwatch sw = new Stopwatch();
            Logger.Log("Analysing file " + filepath);
            sw.Start();
            Stream stream = null;
            VideoElement video = new VideoElement();
            video.Frames = new List<Frame>();
            video.Errors = new List<DecodeError>();
            Searcher frameHeaderSearch = new Searcher(FrameHeader);
            Searcher colorAtomSearch = new Searcher(ColorAtomHeader);
            try
            {
                stream = File.OpenRead(filepath);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to open " + filepath + " (" + ex.Message + ")");
                video.AddError("Unable to open file", ex.Message, DecodeError.ErrorGravity.Critical);
                return video;
            }
            long framecount = 0;

            while (true) //Loop until we arrive at the end (-1)
            {
                var latestbyte = stream.ReadByte();
                if (latestbyte == -1) break;
                feedbackCounter++;
                if (feedbackCounter == feedbackOccurence)
                {
                    feedbackCounter = 0;
                    AnalysisProgress?.Invoke(filepath, (float)stream.Position / stream.Length);
                }
                if (FileManagement.settings.DetectFrameHeader)
                {
                    frameHeaderSearch.NextByte(latestbyte);
                    if (frameHeaderSearch.Matched)
                    {
                        //A Frame header has been found
                        framecount++;

                        var frame = new Frame();
                        stream.Position -= 8;
                        long startpos = stream.Position;
                        frame.Offset = stream.Position;
                        frame.FrameSize = ToBigEndianInt(ReadBytes(stream, 4), 0);

                        stream.Position += 4;
                        frame.HeaderSize = ToBigEndianShort(ReadBytes(stream, 2), 0);
                        frame.Version = ToBigEndianShort(ReadBytes(stream, 2), 0);
                        frame.CreatorID = System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes(stream, 4));
                        frame.FrameWidth = ToBigEndianShort(ReadBytes(stream, 2), 0);
                        frame.FrameHeight = ToBigEndianShort(ReadBytes(stream, 2), 0);

                        byte FrameFlags = (byte)stream.ReadByte();
                        frame.ChrominanceFactor = DoubleBitToByte(FrameFlags.GetBit(0), FrameFlags.GetBit(1));
                        frame.FrameType = DoubleBitToByte(FrameFlags.GetBit(4), FrameFlags.GetBit(5));

                        frame.IgnoredFirst = (byte)stream.ReadByte();
                        frame.Primaries = stream.ReadByte();
                        frame.TransferFunction = stream.ReadByte();
                        frame.ColorMatrix = stream.ReadByte();

                        byte sourceInfo = (byte)stream.ReadByte();
                        frame.SourcePixelFormat = sourceInfo & 0x0F;
                        frame.AlphaInfo = sourceInfo >> 4;

                        byte QMatFlags = (byte)stream.ReadByte();
                        frame.CustomLumaQuant = QMatFlags.GetBit(6);
                        frame.CustomChromaQuant = QMatFlags.GetBit(7);
                        frame.IgnoredSecond = (byte)stream.ReadByte();
                        if (frame.CustomLumaQuant)
                        {
                            frame.QMatLuma = ReadBytes(stream, 64);
                        }
                        if (frame.CustomChromaQuant)
                        {
                            frame.QMatChroma = ReadBytes(stream, 64);
                        }
                        video.Frames.Add(frame);
                        if (optimizations && FileManagement.settings.SkipFrameContent)
                            stream.Position = startpos + frame.FrameSize; //Skip to the end of this frame's content to avoid analyzing useless data
                    }
                }
                if (FileManagement.settings.DetectColrAtom)
                {
                    colorAtomSearch.NextByte(latestbyte);
                    if (colorAtomSearch.Matched)
                    {
                        if (video.ColorAtom != null)
                            video.AddError("Multiple color atoms", "This file appears to contain multiple instances of the QuickTime \"colr\" atom.", DecodeError.ErrorGravity.Critical);
                        video.ColorAtom = new ColorAtom();
                        stream.Position -= 8;
                        video.ColorAtom.Offset = stream.Position;
                        video.ColorAtom.Size = ToBigEndianInt(ReadBytes(stream, 4), 0);
                        stream.Position += 4;
                        Logger.Log("Detected color atom at offset " + video.ColorAtom.Offset);
                        string type = System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes(stream, 4));
                        video.ColorAtom.RawColorParam = type;
                        if (type == "nclc")
                            video.ColorAtom.Type = ColorParameter.Video;
                        else if (type == "prof")
                            video.ColorAtom.Type = ColorParameter.Print;
                        else
                        {
                            video.ColorAtom.Type = ColorParameter.Unknown;
                            video.AddError("Unkown color parameter type", "The color parameter type in the QuickTime \"colr\" atom is unknown (" + type + ")", DecodeError.ErrorGravity.Informational);
                        }
                        video.ColorAtom.PrimariesIndex = ToBigEndianShort(ReadBytes(stream, 2), 0);
                        video.ColorAtom.TransferFunction = ToBigEndianShort(ReadBytes(stream, 2), 0);
                        video.ColorAtom.ColorMatrix = ToBigEndianShort(ReadBytes(stream, 2), 0);
                    }
                }
            }
            stream.Dispose();
            Frame previousframe = null;
            int frameLoopCount = 1;
            video.ColorPrimary = new Dictionary<int, List<int>>();
            video.TransferFunction = new Dictionary<int, List<int>>();
            video.ColorMatrix = new Dictionary<int, List<int>>();
            video.AlphaInfo = new Dictionary<int, List<int>>();
            video.PixelFormat = new Dictionary<int, List<int>>();
            video.CreatorID = new Dictionary<string, List<int>>();
            video.Resolutions = new Dictionary<string, List<int>>();
            video.PixelFormat = new Dictionary<int, List<int>>();
            video.PictureType = new Dictionary<int, List<int>>();
            video.LumaQuants = new Dictionary<byte[], List<int>>();
            video.ChromaQuants = new Dictionary<byte[], List<int>>();
            video.ChrominanceFactor = new Dictionary<int, List<int>>();
           
            if (video.Frames.Count > 0)
            {
                foreach (var frame in video.Frames)
                {
                    video.ColorPrimary.AddValue(frame.Primaries, frameLoopCount);
                    video.TransferFunction.AddValue(frame.TransferFunction, frameLoopCount);
                    video.ColorMatrix.AddValue(frame.ColorMatrix, frameLoopCount);
                    video.AlphaInfo.AddValue(frame.AlphaInfo, frameLoopCount);
                    video.PixelFormat.AddValue(frame.SourcePixelFormat, frameLoopCount);
                    video.CreatorID.AddValue(frame.CreatorID, frameLoopCount);
                    video.Resolutions.AddValue(frame.FrameWidth + "x" + frame.FrameHeight, frameLoopCount);
                    video.PictureType.AddValue(frame.PictureFormat, frameLoopCount);
                    video.PixelFormat.AddValue(frame.SourcePixelFormat, frameLoopCount);
                    if (frame.CustomLumaQuant)
                        video.LumaQuants.AddValue(frame.QMatLuma, frameLoopCount);
                    else
                        video.LumaQuants.AddValue(nullReplacement, frameLoopCount);
                    if(frame.CustomChromaQuant)
                        video.ChromaQuants.AddValue(frame.QMatChroma, frameLoopCount);
                    else
                        video.ChromaQuants.AddValue(nullReplacement, frameLoopCount);

                    frameLoopCount++;
                }

                int atomPrimary = -1;
                int atomTransfer = -1;
                int atomMatrix = -1;
                if(video.ColorAtom != null)
                {
                    atomPrimary = video.ColorAtom.PrimariesIndex;
                    atomTransfer = video.ColorAtom.TransferFunction;
                    atomMatrix = video.ColorAtom.ColorMatrix;
                }

                video.DisplayColorPrimary = HumanizeValues(video.ColorPrimary, ref ColorPrimaries, ref video, "color primary", DecodeError.ErrorGravity.Critical, atomPrimary);
                video.DisplayTransferFunction = HumanizeValues(video.TransferFunction, ref TransferFunctions, ref video, "transfer function", DecodeError.ErrorGravity.Critical, atomTransfer);
                video.DisplayColorMatrix = HumanizeValues(video.ColorMatrix, ref ColorMatrixes, ref video, "color matrix", DecodeError.ErrorGravity.Critical, atomMatrix);
                video.DisplayPixelFormat = HumanizeValues(video.PixelFormat, ref SourcePixelFormats, ref video, "source pixel format", DecodeError.ErrorGravity.Informational, -1);
                video.DisplayAlphaInfo = HumanizeValues(video.AlphaInfo, ref AlphaInfo, ref video, "alpha type", DecodeError.ErrorGravity.Informational, -1);
                video.DisplayPictureType = HumanizeValues(video.PictureType, ref FrameTypes, ref video, "frame type", DecodeError.ErrorGravity.Informational, -1);
                video.DisplayChrominanceFactor = HumanizeValues(video.ChrominanceFactor, ref ChrominanceFactors, ref video, "chrominance factor", DecodeError.ErrorGravity.Informational, -1);

                if (video.CreatorID.Count == 0)
                    video.DisplayCreatorId = "N/A";
                else if (video.CreatorID.Count == 1)
                    video.DisplayCreatorId = video.CreatorID.Keys.First();
                else
                {
                    string jointIDs = string.Join(", ", video.CreatorID.Keys.ToList());
                    video.DisplayCreatorId = "Multiple (" + jointIDs + ")";
                    video.AddError("More than one creator ID", jointIDs, DecodeError.ErrorGravity.Informational);
                }

                if (video.Resolutions.Count == 0)
                    video.DisplayResolution = "N/A";
                else if (video.Resolutions.Count == 1)
                    video.DisplayResolution = video.Resolutions.Keys.First();
                else
                {
                    string jointIDs = string.Join(", ", video.Resolutions.Keys.ToList());
                    video.DisplayResolution = "Multiple (" + jointIDs + ")";
                    video.AddError("More than one resolution", jointIDs, DecodeError.ErrorGravity.Informational);
                }
            }
            else
            {
                if(video.ColorAtom != null)
                {
                    video.DisplayColorPrimary = ColorPrimaries.GetValue(video.ColorAtom.PrimariesIndex);
                    video.DisplayTransferFunction = TransferFunctions.GetValue(video.ColorAtom.TransferFunction);
                    video.DisplayColorMatrix = ColorMatrixes.GetValue(video.ColorAtom.ColorMatrix);
                }
            }

            video.FrameCount = framecount;

            if (video.FrameCount == 0 && video.ColorAtom == null)
                video.AddError("Incompatible file", "This file does not contain any ProRes frames, nor does it contain any QuickTime color information", DecodeError.ErrorGravity.IncompatibleFile);
            else if (video.FrameCount == 0 && video.ColorAtom != null)
                video.AddError("No ProRes frames", "This file possibly contain QuickTime color information, however it does not contain any ProRes frames", DecodeError.ErrorGravity.IncompatibleFile);
            else if (video.FrameCount != 0 && video.ColorAtom == null)
                video.AddError("No color atom", "The file does not contain any QuickTime \"colr\" atom", DecodeError.ErrorGravity.Informational);
            Logger.Log("Done in " + sw.ElapsedMilliseconds + "ms");
            return video;
        }


        public static async Task<List<DecodeError>> Encode(VideoElement video, int targetColorPrimary, int targetTransferFunction, int targetColorMatrix, string targetCreatorId)
        {
            List<DecodeError> errors = new List<DecodeError>();
            FileStream stream = null;

            //Check if the file can be overwritten, or if a copy must be made, then open the stream:
            try
            {
                string directory = FileManagement.settings.WriteLocation;
                if (directory == "?override")
                    stream = new FileStream(video.FilePath, FileMode.Open, FileAccess.ReadWrite);
                else
                {
                    var videofile = new FileInfo(video.FilePath);
                    if (directory == "?default")
                        directory = videofile.DirectoryName;
                    if (!Directory.Exists(directory)) throw new Exception("Target directory ("+directory+") doesn't exist!");

                    int itteration = 0;
                    while (File.Exists(directory + MainForm.PathSeperator + videofile.Name + "_modified" + itteration.ToString() + videofile.Extension))
                        itteration++;
                    File.Copy(video.FilePath, directory + MainForm.PathSeperator+ videofile.Name + "_modified" + itteration.ToString() + videofile.Extension);
                    stream = new FileStream(directory + MainForm.PathSeperator + videofile.Name + "_modified" + itteration.ToString() + videofile.Extension, FileMode.Open, FileAccess.ReadWrite);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FileNotFoundException))
                {
                    errors.Add(new DecodeError("Missing file", "The file " + video.FileName + " no longer exists!", DecodeError.ErrorGravity.Critical));
                }
                else
                {
                    errors.Add(new DecodeError("Failed to open source file", ex.Message, DecodeError.ErrorGravity.Critical));
                }
                return errors;
            }

            //Overwrite the data in each frame:
            foreach(var frame in video.Frames)
            {
                stream.Position = frame.Offset+12;

                if (targetCreatorId != null)
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(targetCreatorId), 0, 4);
                    stream.Position += 6;
                }  
                else
                    stream.Position += 10;

                if (targetColorPrimary != -1)
                    stream.WriteByte(Convert.ToByte(targetColorPrimary));
                else
                    stream.Position += 1;

                if (targetTransferFunction != -1)
                    stream.WriteByte(Convert.ToByte(targetTransferFunction));
                else
                    stream.Position += 1;

                if (targetColorMatrix != -1)
                    stream.WriteByte(Convert.ToByte(targetColorMatrix));
            }
            //Overwrite the data in the color atom:
            if(video.ColorAtom != null && video.ColorAtom.Type == ColorParameter.Video)
            {
                if (video.ColorAtom.Type == ColorParameter.Video)
                {
                    stream.Position = video.ColorAtom.Offset + 12;
                    if (targetColorPrimary != -1)
                    {
                        Debug.WriteLine("Color primary=" + Convert.ToInt16(targetColorPrimary).ToBigEndian());
                        stream.Write(Convert.ToInt16(targetColorPrimary).ToBigEndian(), 0, 2);
                    }
                    else
                        stream.Position += 2;

                    if (targetTransferFunction != -1)
                        stream.Write(Convert.ToInt16(targetTransferFunction).ToBigEndian(), 0, 2);
                    else
                        stream.Position += 2;

                    if (targetColorMatrix != -1)
                        stream.Write(Convert.ToInt16(targetColorMatrix).ToBigEndian(), 0, 2);
                    else
                        stream.Position += 2;
                }
                else if ((video.ColorAtom.Type == ColorParameter.Print && FileManagement.settings.OverrideColrPrint) ||
                        (video.ColorAtom.Type == ColorParameter.Unknown && FileManagement.settings.OverrideColrUnknown))
                {
                    var replacement = CreateColorAtom(targetColorPrimary, targetTransferFunction, targetColorMatrix);
                    if (video.ColorAtom.Size == 18)
                    {
                        //It's the right size, we can just replace the existing one
                        stream.Position = video.ColorAtom.Offset;
                        await stream.WriteAsync(replacement, 0, 18);
                    }
                    else if(video.ColorAtom.Size < 18)
                    {
                        //TODO add the amount of bytes in 18-video.ColorAtom.Size
                    }
                    else
                    {
                        //TODO add the amount of bytes in video.ColorAtom.Size-18
                    }
                }
            }
            else
            {
                errors.Add(new DecodeError("No QuickTime colr atom", "There was no QuickTime color atom information to replace", DecodeError.ErrorGravity.Informational));
            }
            stream.Dispose();
            return errors;
        }

        public class Searcher
        {
            public Searcher(int[] pattern)
            {
                Pattern = pattern;
            }
            public int[] Pattern { get; set; }
            public bool Matched = false;
            private int SearchPosition = 0;

            public void NextByte(int b)
            {
                Matched = false;
                if (b == Pattern[0])
                    SearchPosition = 0;
                if (b == Pattern[SearchPosition])
                {
                    SearchPosition++;
                    if(SearchPosition == Pattern.Length)
                    {
                        Matched = true;
                        SearchPosition = 0;
                    }
                }
                else
                {
                    SearchPosition = 0;
                }
            }
        }
    }
}
