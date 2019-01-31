using System;
using System.IO;
using System.Text;

namespace invox.Lib {
    class Logger {
        const string LOG_FILENAME = "\\invox.log";

        static StreamWriter instance = null;
        static object flock = new object();

        static StreamWriter Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new StreamWriter(invox.Options.BaseDirectory + LOG_FILENAME, true, Encoding.UTF8);
                        }
                    }
                return instance;
            }
        }

        /// <summary>
        /// Log message to console and to the logfile
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message) {
            Console.WriteLine(message);
            Instance.WriteLine(message);
        }
    }
}
