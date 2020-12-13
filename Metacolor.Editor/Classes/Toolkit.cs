using Eto.Drawing;
using Eto.Forms;
using System;

namespace Metacolor.Editor
{
    public static class Toolkit
    {
        public const string MultipleSelectionString = "Multiple selected";
        public static void UpdateTextForMultiple(this Label label, ref string oldstr, string newstr, bool firstiteration)
        {
            label.Text = CheckMultiple(ref oldstr, newstr, firstiteration);
            if (label.Text == MultipleSelectionString)
            {
                label.TextColor = new Eto.Drawing.Color(0, 0, 0, 0.4f);
            }
            else
            {
                label.TextColor = new Eto.Drawing.Color(0, 0, 0, 1);
            }
        }

        public static Image TemplateImage(this Image image)
        {
            image.Style = "templateimage";
            return image;
        }

        public static string CheckMultiple(ref string oldstr, string newstr, bool firstiteration)
        {
            if (firstiteration)
            {
                oldstr = newstr;
                return newstr;
            }
            else
            {
                if (oldstr != newstr)
                {
                    oldstr = MultipleSelectionString;
                    return MultipleSelectionString;
                }
                else
                {
                    return newstr;
                }
            }
        }
        public static string ToMonospace(this string input)
        {
            string output = "";
            foreach (var c in input)
            {
                switch (c)
                {
                    case '0': output += "𝟶"; break;
                    case '1': output += "𝟷"; break;
                    case '2': output += "𝟸"; break;
                    case '3': output += "𝟹"; break;
                    case '4': output += "𝟺"; break;
                    case '5': output += "𝟻"; break;
                    case '6': output += "𝟼"; break;
                    case '7': output += "𝟽"; break;
                    case '8': output += "𝟾"; break;
                    case '9': output += "𝟿"; break;
                    default: output += c; break;
                }
            }
            return output;
        }
        public static bool GetBit(this byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
        public static byte DoubleBitToByte(bool bit1, bool bit2)
        {
            if (!bit1 && !bit2) return 0;
            else if (!bit1 && bit2) return 1;
            else if (bit1 && !bit2) return 2;
            else if (bit1 && bit2) return 3;
            else return 0;
        }

        public static byte[] ToBigEndian(this int value)
        {
            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                return new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] };
            else
                return buffer;
        }
        public static byte[] ToBigEndian(this short value)
        {
            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                return new byte[2] { buffer[1], buffer[0] };
            else
                return buffer;
        }
        public static short ToBigEndianShort(byte[] buffer, int i)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(new byte[2] { buffer[1], buffer[0] }, 0);
            else
                return BitConverter.ToInt16(buffer, 0);
        }
        public static int ToBigEndianInt(byte[] buffer, int i)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] }, 0);
            else
                return BitConverter.ToInt32(buffer, 0);
        }
    }
}
