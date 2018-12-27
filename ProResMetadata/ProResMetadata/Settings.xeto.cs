using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using static ProResMetadata.FileManagement;
using System.ComponentModel;

namespace ProResMetadata
{	
	public class Settings : Dialog
	{
        private CheckBox detectColr;
        private CheckBox detectFrame;
        private CheckBox skipFrameContent;
        private CheckBox openVlc;
		public Settings()
		{
			XamlReader.Load(this);
            Setup();
		}
        protected override void OnClosing(CancelEventArgs e)
        {
            settings.DetectColrAtom = detectColr.Checked == true;
            settings.DetectFrameHeader = detectFrame.Checked == true;
            settings.SkipFrameContent = skipFrameContent.Checked == true;
            settings.OpenFilesInVlc = openVlc.Checked == true;
            SaveSettings();
            base.OnClosing(e);
        }
        public async void Setup()
        {
            GetSettings();
            detectColr.Checked = settings.DetectColrAtom;
            detectFrame.Checked = settings.DetectFrameHeader;
            skipFrameContent.Checked = settings.SkipFrameContent;
            openVlc.Checked = (bool)settings.OpenFilesInVlc;
        }
	}
}
