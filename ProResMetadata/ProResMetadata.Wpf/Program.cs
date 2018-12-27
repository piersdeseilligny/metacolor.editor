using System;
using Eto.Forms;
using Eto.Wpf.Forms.Controls;

namespace ProResMetadata.Wpf
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
            new Application(Eto.Platforms.Wpf).Run(new MainForm(args, '\\'));
		}
	}
}
