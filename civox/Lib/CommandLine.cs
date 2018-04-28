using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace civox.Lib {
    /// <summary>
    /// Command line option attribute
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    public class CommandLineOptionAttribute : System.Attribute {
        string ShortOption;
        string LongOption;
        string HelpText;
        public bool IsHelp;

        /// <summary>
        /// Mark property as command line attribute
        /// </summary>
        /// <param name="shortOption">Short option name</param>
        /// <param name="longOption">Long option name</param>
        /// <param name="helpText">Help text for the option</param>
        /// <remarks>Short option may be marked with eighter - or /
        /// Long option is preceded by double dash --</remarks>
        public CommandLineOptionAttribute(string shortOption, string longOption, string helpText) {
            ShortOption = shortOption;
            LongOption = longOption;
            HelpText = helpText;
            IsHelp = false;
        }

        /// <summary>
        /// Match this option with the given one
        /// </summary>
        /// <param name="option">Short or long option preceded with -, / or --</param>
        /// <returns>True if this is the very option</returns>
        public bool Matches(string option) {
            if (option.StartsWith("--")) {
                return option.Substring(2) == LongOption;
            } else if (option.StartsWith("-") || option.StartsWith("/")) {
                return option.Substring(1) == ShortOption;
            }
            return false;
        }

        /// <summary>
        /// Get help on the parameter
        /// </summary>
        /// <returns></returns>
        public string GetHelp() {
            return string.Format("-{0}\t--{1}\r\n\t\t{2}", ShortOption, LongOption, HelpText);
        }
    }

    /// <summary>
    /// Attribute to mark an options as error message
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    public class ErrorMessageAttribute : Attribute { }

    /// <summary>
    /// Command line parser
    /// </summary>
    class CommandLineOptions {
        /// <summary>
        /// Parse command line
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <param name="type">Type of a class containing program options</param>
        /// <returns>Instance of the type initialized with command line arguments</returns>
        public static object Get(string[] args, Type type) {
            object result = type.GetConstructor(new Type[] { }).Invoke(new object[] { });

            int index = 0;
            PropertyInfo[] pis = type.GetProperties();

            // Foreach command line key
            while (index < args.Length) {
                string k = args[index].ToLower();
                bool match = false;
                foreach (PropertyInfo p in pis) {
                    // Find an option matching to the command line key
                    Type pt = p.PropertyType;
                    if (Matches(p, k)) {
                        if (pt == typeof(bool)) {
                            // Boolean property - set to true
                            p.SetValue(result, true, null);
                        } else {
                            // Other property type - consume next command line word as the key's parameter
                            ++index;
                            if (index >= args.Length) {
                                InvokeHelp(result, "Нет параметра для ключа " + k);
                                return result;
                            }

                            if (pt == typeof(string)) {
                                // String value
                                p.SetValue(result, args[index], null);
                            } else if (pt == typeof(int)) {
                                // Integer value
                                int i;
                                if (int.TryParse(args[index], out i)) {
                                    p.SetValue(result, i, null);
                                } else {
                                    InvokeHelp(result, string.Format("Параметр {0} ожидает целое число, введено '{1}'", k, args[index]));
                                    return result;
                                }
                            } else if (pt == typeof(DateTime)) {
                                // DateTime value
                                p.SetValue(result, DateHelper.Parse(args[index]), null);
                            }
                        }
                        match = true;
                        break;
                    }
                }

                // No option found for the key
                if (!match) {
                    InvokeHelp(result, "Неизвестный параметр: " + k);
                    break;
                }

                ++index;
            }

            return result;
        }

        /// <summary>
        /// Mark options as requesting help
        /// </summary>
        /// <param name="obj">Instance of a command line options class</param>
        /// <param name="message">Message to paste to the options instance</param>
        static void InvokeHelp(object obj, string message) {
            int done = 2;
            PropertyInfo[] pis = obj.GetType().GetProperties();
            foreach (PropertyInfo info in pis) {
                Attribute[] attrs = System.Attribute.GetCustomAttributes(info);
                foreach (Attribute a in attrs) {
                    if (a is CommandLineOptionAttribute) {
                        if ((a as CommandLineOptionAttribute).IsHelp) {
                            info.SetValue(obj, true, null);
                            done--;
                        }
                    } else if (a is ErrorMessageAttribute) {
                        info.SetValue(obj, message, null);
                        done--;
                    }
                    if (done == 0) return;
                }
                if (done == 0) return;
            }
        }

        /// <summary>
        /// Compares command line class property with the given command line option
        /// </summary>
        /// <param name="info">Discriptor of a property to compare option with</param>
        /// <param name="k">Option value taken from command line</param>
        /// <returns>True if command line option matches the property</returns>
        static bool Matches(PropertyInfo info, string k) {
            Attribute[] attrs = System.Attribute.GetCustomAttributes(info);
            foreach (Attribute a in attrs) {
                if (a is CommandLineOptionAttribute) {
                    if ((a as CommandLineOptionAttribute).Matches(k)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Show options help and error message (if any)
        /// </summary>
        /// <param name="obj">Command options class instance</param>
        public static string ShowHelp(object obj) {
            string error = null;
            StringBuilder sb = new StringBuilder();

            PropertyInfo[] pis = obj.GetType().GetProperties();
            foreach (PropertyInfo info in pis) {
                Attribute[] attrs = System.Attribute.GetCustomAttributes(info);
                foreach (Attribute a in attrs) {
                    if (a is CommandLineOptionAttribute) {
                        sb.Append("\r\n");
                        sb.Append((a as CommandLineOptionAttribute).GetHelp());
                    } else if (a is ErrorMessageAttribute) {
                        error = (string) info.GetValue(obj, null);
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
                sb.Append("\r\n\r\n" + error);

            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}
