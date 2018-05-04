using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace civox.Lib {
    /// <summary>
    /// Let them read something usefull there in FOMS
    /// </summary>
    /// <remarks>Reads a text file line by line</remarks>
    class ReadingBot {
        StreamReader reader = null;

        public bool OK { get { return reader != null; } }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="filename">Path to the file to read</param>
        public ReadingBot(string filename) {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename)) {
                reader = new StreamReader(filename);
            }
        }

        /// <summary>
        /// Read a line
        /// </summary>
        /// <returns>Line of text from the file</returns>
        public string Read() {
            if (reader != null) {
                if (reader.EndOfStream) {
                    reader.Close();
                    reader = null;
                    return string.Empty;
                }
                return reader.ReadLine();
            } else return string.Empty;
        }

        /// <summary>
        /// Close the reader
        /// </summary>
        public void Close() {
            if (reader != null) {
                reader.Close();
                reader = null;
            }
        }
    }
}
