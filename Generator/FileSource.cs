using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class FileSource
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }

        public FileSource(long length, string name)
        {
            Data = new byte[length];
            FileName = name;
        }

        public FileSource(byte[] data, string name)
        {
            Data = data;
            FileName = name;
        }
    }
}
