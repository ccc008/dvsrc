using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlexBuildIncrementer {
    class Program {

        static void help() {
            System.Console.WriteLine("FlexBuildIncrementer, see http://code.google.com/p/dvsrc/");
            System.Console.WriteLine("Generates version for C++ application according specified rules");
            System.Console.WriteLine("FlexBuildIncrementer <path> [mode]");
            System.Console.WriteLine("Source file should contain single line: ");
            System.Console.WriteLine("   #define APPLICATION_VERSION \"1.0.0.0\" //[DEFAULT_MODE]");           
            System.Console.WriteLine("Supported modes: inc, sec70, date_inc, date_sec70");
            System.Console.WriteLine("  inc: 1.0.0.1 -> 1.0.0.2");
            System.Console.WriteLine("  sec70: 1.0.0.1 -> 1.0.0.COUNT_SECONDS_SINCE_1970");
            System.Console.WriteLine("  date: 1.0.0.1 -> CURRENT_YEAR.CURRENT_MONTH.CURRENT_DAY.2");
            System.Console.WriteLine("  date_sec70: 1.0.0.1 -> CURRENT_YEAR.CURRENT_MONTH.CURRENT_DAY.COUNT_SECONDS_SINCE_1970");
            System.Console.WriteLine("Command line parameter \"mode\" is optional. If it isn't specified");
            System.Console.WriteLine("then DEFAULT_MODE from comment is used as mode. If [DEFAULT_MODE] is unspecified");
            System.Console.WriteLine("then inc mode is used.");
        }

        static void Main(string[] args) {
            if (args.Length < 1) {
                help();
                return;
            }

            String path = args[0];
            String mode = args.Length > 1 ? args[1] : null;

        //load file content
            String file_content = System.IO.File.Exists(path)
                ? GetFileBody(path, "utf-8")
                : "#define APPLICATION_VERSION \"1.0.0.0\" //[inc] inc, sec70, date, date_sec70";

        //parse file content
            Regex r = new Regex(".*\\\"(\\d+\\.\\d+\\.\\d+\\.\\d+)\\\"\\s*//(\\[([\\w_]+)\\])");
            Match m = r.Match(file_content);
            if (!m.Success) {
                throw new Exception("File content doesn't match to pattern: \"#define APPLICATION_VERSION \"1.0.0.0\" //[DEFAULT_MODE]\", " + file_content);
            }

       //set mode to DEFAULT MODE if mode is not specified as command line param
            if (mode == null) {
                if (m.Groups.Count >= 3) {
                    mode = m.Groups[3].Captures[0].ToString();
                }
            }

        //replace version according mode
            Regex r_replace = new Regex("\\d+\\.\\d+\\.\\d+\\.\\d+");
            Replacer replacer = new Replacer(mode);
            System.Diagnostics.Debug.Assert(m != null);

            file_content = r_replace.Replace(file_content, new MatchEvaluator(replacer.Replace));
        //save results
            SaveStringToFile(file_content, path);

        }
        private class Replacer {
            private readonly String _Mode;
            private readonly Regex _R = new Regex("(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)");
            public Replacer(String mode) {
                _Mode = mode;
            }
            public String Replace(Match m) {
                Match m2 = _R.Match(m.Value);
                return m2.Success
                    ? replace_version(Int32.Parse(m2.Groups[1].Captures[0].ToString())
                        , Int32.Parse(m2.Groups[2].Captures[0].ToString())
                        , Int32.Parse(m2.Groups[3].Captures[0].ToString())
                        , Int32.Parse(m2.Groups[4].Captures[0].ToString())
                    )
                    : m.Value;
            }
            private String replace_version(int majour, int minour, int rev, int build) {                
                switch (_Mode) {
                    case "sec70":
                        return String.Format("{0}.{1}.{2}.{3}", majour, minour, rev, get_sec70());
                    case "date":
                        return String.Format("{0}.{1}.{2}.{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, build + 1);
                    case "date_sec70":
                        return String.Format("{0}.{1}.{2}.{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, get_sec70());
                    default: //inc
                        if (_Mode != "inc") {
                            System.Console.WriteLine(String.Format("Mode {0} is unknown. Inc mode is used", _Mode));
                        }
                        return String.Format("{0}.{1}.{2}.{3}", majour, minour, rev, build + 1);
                }
            }
        }

        public static long get_sec70() {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = DateTime.UtcNow - origin;
            return (long)diff.TotalSeconds;
        }

        public static String GetFileBody(String FileName, String SrcEncoding) {
            Encoding encode = System.Text.Encoding.GetEncoding(SrcEncoding);
            return encode.GetString(GetFileBody(FileName));
        }
        public static byte[] GetFileBody(String FileName) {
            using (System.IO.FileStream f = new System.IO.FileStream(FileName, System.IO.FileMode.Open)) {
                byte[] bytes = new byte[f.Length];
                Int32 readbytes = f.Read(bytes, 0, (Int32)f.Length);
                System.Diagnostics.Debug.Assert(f.Length == readbytes);
                return bytes;
            }
        }

        public static void SaveStringToFile(String srcStr, String fileName) {
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(fileName)) {
                sw.Write(srcStr);
            }
        }

    }
}

