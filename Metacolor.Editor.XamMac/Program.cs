using System;
using System.IO;
using AppKit;
using CoreGraphics;
using Eto.Forms;
using Eto.Mac.Drawing;
using Metacolor.Editor;
using Metacolor.Editor.Classes;
using static System.Environment;

namespace Metacolor.Editor.XamMac
{
	static class MainClass
	{
		static void Main(string[] args)
		{
			
			Eto.Style.Add<BitmapHandler>("template", h =>
			{
				// Ideally, this style should make the image look more native, and react to hover etc...
				// However, it currently crashes on launch when anything is done (macOS 11.1), which is why there's nothing

				/*var sourceImage = h.Control;
				sourceImage.Template = true;

				h.Control = NSImage.ImageWithSize(sourceImage.Size, false, rect =>
				{
					sourceImage.DrawInRect(rect, CGRect.Empty, NSCompositingOperation.SourceOver, 1f);

					NSColor.Gray.Set();
					NSGraphics.RectFill(rect, NSCompositingOperation.SourceAtop);
					return true;
				});*/
			});

			StartupArgs startup = new StartupArgs() { OpenWithFiles = args };
			// Use DoNotVerify in case LocalApplicationData doesn’t exist.
			string appData = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "metacolor.editor");
			// Ensure the directory and all its parents exist.
			Directory.CreateDirectory(appData);
			startup.SettingsDirectory = appData;
			new Application(Eto.Platforms.XamMac2).Run(new MainForm(startup));

		}
	}
}
