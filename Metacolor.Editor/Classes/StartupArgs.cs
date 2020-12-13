using System;
using System.Collections.Generic;
using System.Text;

namespace Metacolor.Editor.Classes
{
    public class StartupArgs
    {
        public string SettingsDirectory { get; set; }
        public string[] OpenWithFiles { get; set; }

        public char Seperator = '/';
    }
}
