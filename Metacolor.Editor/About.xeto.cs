using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Eto;

namespace Metacolor.Editor
{
    public class About : Dialog
    {
        ImageView appLogo;
        public About()
        {
            XamlReader.Load(this);
            var bmp = Bitmap.FromResource("Metacolor.Editor.Assets.icon.png", assembly: null);
            bmp.Style = "template";
            appLogo.Image = bmp;
        }

        private void OpenGithub(object sender, EventArgs e)
        {
                Application.Instance.Open("https://github.com/piersdeseilligny/prores.editor");
        }
        private void OpenLicense(object sender, EventArgs e)
        {
            string mitlicense = @"The MIT License (MIT)

Copyright(c) 2018 Piers Deseilligny

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";

            MessageBox.Show(this, mitlicense, "License", MessageBoxType.Information);
        }
        private void OpenLicenses(object sender, EventArgs e)
        {
            (new Licenses()).ShowModal(this);
        }
        private void Donate(object sender, EventArgs e)
        {
                Application.Instance.Open("https://paypal.me/piersdeseilligny");
        }
        private void Support(object sender, EventArgs e)
        {
                Application.Instance.Open("https://github.com/sponsors/piersdeseilligny");
        }
        private void OpenIcons8(object sender, EventArgs e)
        {
                Application.Instance.Open("https://icons8.com/");
        }
    }
}
