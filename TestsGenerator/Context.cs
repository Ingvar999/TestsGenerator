﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Generator;

namespace ConsoleApp
{
    public class Context<T>
    {
        public T Obj { get; set; }
        public FileSource Item { get; set; }
        public FileStream Stream { get; set; }

        public Context(T obj, FileSource item, FileStream stream)
        {
            Obj = obj;
            Item = item;
            Stream = stream;
        }
    }
}
