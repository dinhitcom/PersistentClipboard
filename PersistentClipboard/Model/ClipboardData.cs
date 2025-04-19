using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentClipboard.Model
{
    public enum ClipboardDataType
    {
        None = 0,
        Text = 1,
        Image = 2,
        FileDropList = 3
    }
    internal class ClipboardData
    {
        public ClipboardDataType Type { get; set; }
        public string[] Data { get; set; }
    }
}
