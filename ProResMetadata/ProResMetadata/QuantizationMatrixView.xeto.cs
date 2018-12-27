using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace ProResMetadata
{
    public class QuantizationMatrixView : Dialog
    {
        private Label title;
        private StackLayout stacker;

        public class Group
        {
            public Group(int startposition)
            {
                StartPosition = startposition;
                EndPosition = startposition;
            }
            public string name
            {
                get
                {
                    return (StartPosition) + "-" + (EndPosition);
                }
            }
            public int StartPosition = 0;
            public int EndPosition = 0;
        }
        public List<Group> GetRanges(List<int> input)
        {
            List<Group> groups = new List<Group>();
            for (var i = 0; i < input.Count; i++)
            {
                if (groups.Count > 0 && groups[groups.Count - 1].EndPosition + 1 == input[i])
                {
                    groups[groups.Count - 1].EndPosition = input[i];
                }
                else
                {
                    groups.Add(new Group(input[i]));
                }
            }
            return groups;
        }

        public string MatrixToString(byte[] input)
        {
            string output = "";
            for (var i = 1; i < 9; i++)
            {
                for (var j = 1; j < 9; j++)
                {
                    var value = input[(i * j) - 1];
                    output += value;
                    if (j != 8) output += ",";
                    if (value < 10) output += "   ";
                    else if (value < 100) output += "  ";
                    else output += " ";
                    
                }
                output += "\n";
            }
            return output.ToMonospace();
        }


        public QuantizationMatrixView(string type, Dictionary<byte[], List<int>> content)
        {
            XamlReader.Load(this);
            title.Text = type;
            Title = type;
            foreach (var item in content)
            {
                StackLayout stack = new StackLayout();
                var ranges = GetRanges(item.Value);
                List<string> rangeString = new List<string>();
                foreach (var group in ranges)
                {
                    rangeString.Add(group.name);
                }

                stack.Items.Add(new Label() { Text = "Used for frames " + string.Join(", ", rangeString)+":" });
                stack.Items.Add(new Label() { Height = 6 });
                var innerStack = new StackLayout() { Orientation = Orientation.Horizontal };
                innerStack.Items.Add(new Label() { Text = MatrixToString(item.Key), Style = "MatrixContent" });
                stack.Items.Add(innerStack);
                stacker.Items.Add(stack);
            }
        }
    }
}
