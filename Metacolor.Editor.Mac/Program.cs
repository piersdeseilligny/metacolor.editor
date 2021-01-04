using System;
using System.IO;
using Eto.Forms;
using Eto.Mac.Drawing;
using Metacolor.Editor.Classes;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using static System.Environment;

namespace Metacolor.Editor.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Eto.Style.Add<BitmapHandler>("template", h =>
            {
                var sourceImage = h.Control;
                sourceImage.Template = true;
                
                /*h.Control = NSImage.ImageWithSize(sourceImage.Size, false, rect =>
                {
                   //sourceImage.DrawInRect(rect, CGRect.Empty, NSCompositingOperation.SourceOver, 1f);

                   //NSColor.Blue.Set();
                   // NSGraphics.RectFill(rect, NSCompositingOperation.SourceAtop);
                    return true;
                });*/
            });
            StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
            // Use DoNotVerify in case LocalApplicationData doesn’t exist.
            string appData = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "metacolor.editor");
            // Ensure the directory and all its parents exist.
            Directory.CreateDirectory(appData);
            startup.SettingsDirectory = appData;
            new Application(Eto.Platforms.Mac64).Run(new MainForm(startup));
        }
    }
}
