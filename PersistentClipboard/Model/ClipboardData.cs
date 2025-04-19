using System;
using System.Runtime.Serialization;

namespace PersistentClipboard.Model
{
    internal enum ClipboardDataType
    {
        None = 0,
        Text = 1,
        Image = 2,
        FileDropList = 3
    }

    [DataContract]
    internal class ClipboardData
    {
        [DataMember]
        public ClipboardDataType Type { get; set; }

        [DataMember]
        public string[] Data { get; set; }
    }
}
