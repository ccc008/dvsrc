using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlexBuildIncrementer {
    class Program {

        static void help() {
            System.Console.WriteLine("FlexBuildIncrementer 1.1, see http://code.google.com/p/dvsrc/");
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
            System.Console.WriteLine("");
            System.Console.WriteLine("FlexBuildIncrementer --patch <path of version file> <path of patched file> <regexp> [encoding]");
            System.Console.WriteLine(" Copy versoin from version file to another file according regexp ");
            System.Console.WriteLine(" Example for innosetup script: ");
            System.Console.WriteLine("      Innosetup script contains line: #define APP_VER \"2012.12.20.2\"");
            System.Console.WriteLine(" FlexBuildIncrementer --patch version.h setup.iss \"#define\\s+APP_VER\\s+\\\"(\\d+\\.\\d+\\.\\d+\\.\\d+)\\\"");
        }

        static void Main(string[] args) {
            if (args.Length < 1) {
                help();
                return;
            }
            if (args[0] == "--patch") {
                patch_file(args);
                return;
            }

            String path = args[0];
            String mode = args.Length > 1 ? args[1] : null;

            Match m;
            String file_content;
            parse_version_file_content(path, out file_content, out m);

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
            SaveStringToFile(file_content, path, "utf-8");

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
            Int32 code_page;
            Encoding encode = Int32.TryParse(SrcEncoding, out code_page)
                ? System.Text.Encoding.GetEncoding(code_page)
                : System.Text.Encoding.GetEncoding(SrcEncoding);
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

        public static void SetFileBody(String FileName, byte[] bytes) {
            using (System.IO.FileStream f = new System.IO.FileStream(FileName, System.IO.FileMode.Create)) {
                f.Write(bytes, 0, bytes.Length);
            }
        }

        public static void SaveStringToFile(String srcStr, String fileName, String srcEncoding) {
            Int32 code_page;
            Encoding encode = Int32.TryParse(srcEncoding, out code_page)
                ? System.Text.Encoding.GetEncoding(code_page)
                : System.Text.Encoding.GetEncoding(srcEncoding);
            SetFileBody(fileName, encode.GetBytes(srcStr));
        }

        private static void parse_version_file_content(string path, out string file_content, out Match m) {
            //load file content
            file_content = System.IO.File.Exists(path)
                ? GetFileBody(path, "utf-8")
                : "#define APPLICATION_VERSION \"1.0.0.0\" //[inc] inc, sec70, date, date_sec70";

            //parse file content
            Regex r = new Regex(".*\\\"(\\d+\\.\\d+\\.\\d+\\.\\d+)\\\"\\s*//(\\[([\\w_]+)\\])");
            m = r.Match(file_content);
            if (!m.Success) {
                throw new Exception("File content doesn't match to pattern: \"#define APPLICATION_VERSION \"1.0.0.0\" //[DEFAULT_MODE]\", " + file_content);
            }
        }

        private static void patch_file(string[] args) {
            if (args.Length < 4) {
                help();
                return;
            }
            String path = args[1];
            String path_dest = args[2];
            String sregexp = args[3];
            String encoding = args.Length >= 4
                ? args[4]
                : "utf-8";

        //load version from version.h file
            Match m;
            String file_content;
            parse_version_file_content(path, out file_content, out m);
            String sversion = m.Groups[1].Captures[0].ToString();
        //parse dest file 
            if (! System.IO.File.Exists(path_dest)) {
                throw new Exception("File doesn't exist: " + path_dest);
            }
            String dest_file_content = GetFileBody(path_dest, encoding);
            Regex r_replace = new Regex(sregexp);
            MatchCollection mc = r_replace.Matches(dest_file_content);
            for (int i = mc.Count - 1; i >= 0; --i) {
                int index = mc[i].Groups[1].Captures[0].Index;
                dest_file_content = dest_file_content.Remove(index, mc[i].Groups[1].Captures[0].Length);
                dest_file_content = dest_file_content.Insert(index, sversion);
            }
            SaveStringToFile(dest_file_content, path_dest, encoding);
        }

    }
}

