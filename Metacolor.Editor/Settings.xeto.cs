using Eto.Forms;
using Eto.Serialization.Xaml;
using static Metacolor.Editor.FileManagement;
using System.ComponentModel;
using System;
using Eto;

namespace Metacolor.Editor
{
    public class Settings : Dialog
    {
        private CheckBox detectColr;
        private CheckBox detectFrame;
        private CheckBox skipFrameContent;
        private CheckBox openVlc;

        private RadioButtonList writeLocation;
        private FilePicker selectFolder;
        private CheckBox createColrAtom;
        private CheckBox overrideColrPrint;
        private CheckBox overrideColrUnknown;
        private TabControl tabControl;

        public enum SettingsTab { General, Output }

        public Settings(SettingsTab tab)
        {
            XamlReader.Load(this);
            if(tab == SettingsTab.Output)
                tabControl.SelectedIndex = 1;
            if (EtoEnvironment.Platform.IsMac)
            {
                this.Padding = new Eto.Drawing.Padding(12, 20, 12, 12);
            }
            Setup();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            settings.DetectColrAtom = detectColr.Checked == true;
            settings.DetectFrameHeader = detectFrame.Checked == true;
            settings.SkipFrameContent = skipFrameContent.Checked == true;
            settings.OpenFilesInVlc = openVlc.Checked == true;
            switch (writeLocation.SelectedIndex)
            {
                case 0:
                    settings.WriteLocation = "?override"; break;
                case 1:
                    settings.WriteLocation = "?default"; break;
                case 2:
                    settings.WriteLocation = selectFolder.FilePath; break;
            }
            //TODO:
            //settings.CreateColrAtom = (bool)createColrAtom.Checked;
            //settings.OverrideColrPrint = (bool)overrideColrPrint.Checked;
            //settings.OverrideColrUnknown = (bool)overrideColrUnknown.Checked;
            SaveSettings();
            base.OnClosing(e);
        }
        public void Setup()
        {
            GetSettings();
            detectColr.Checked = settings.DetectColrAtom;
            detectFrame.Checked = settings.DetectFrameHeader;
            skipFrameContent.Checked = settings.SkipFrameContent;
            openVlc.Checked = (bool)settings.OpenFilesInVlc;
            switch (settings.WriteLocation)
            {
                case "?override":
                    writeLocation.SelectedIndex = 0;
                    break;
                case "?default":
                    writeLocation.SelectedIndex = 1;
                    break;
                default:
                    writeLocation.SelectedIndex = 2;
                    selectFolder.FilePath = settings.WriteLocation;
                    selectFolder.Enabled = true;
                    break;
            }
            writeLocation.SelectedIndexChanged += WriteLocation_SelectedIndexChanged;
            //TODO:
            //createColrAtom.Checked = settings.CreateColrAtom;
            //overrideColrPrint.Checked = settings.OverrideColrPrint;
            //overrideColrUnknown.Checked = settings.OverrideColrUnknown;
        }

        private void WriteLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If the save location is set to custom, enable the folder picker
            if (writeLocation.SelectedIndex == 2)
                selectFolder.Enabled = true;
            else
                selectFolder.Enabled = false;
        }
    }
}
