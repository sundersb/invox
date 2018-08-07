using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace onkobuf {
    static class Options {
        const string RESOURCE_DIR = "resources";

        static string baseDirectory;
        static string resourceDirectory;

        static public string BaseDirectory { get { return baseDirectory; } }
        static public string ResourceDirectory { get { return resourceDirectory; } }

        public static void Init() {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm != null)
                baseDirectory = Path.GetDirectoryName(asm.Location) + Path.DirectorySeparatorChar;
            else
                baseDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

            resourceDirectory = baseDirectory + RESOURCE_DIR + Path.DirectorySeparatorChar;
        }
    }
}
