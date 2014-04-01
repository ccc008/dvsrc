using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowdinSpreader {
    class Utils {
        public static void SaveStringToFile(String srcStr, String fileName) {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(fileName)) {
                sw.Write(srcStr);
            }
        }
        public static String[] LoadStringsFromFile(String fileName) {
            List<String> list = new List<string>();
            using (System.IO.StreamReader sr = System.IO.File.OpenText(fileName)) {
                while (!sr.EndOfStream) {
                    list.Add(sr.ReadLine());
                }
            }
            return list.ToArray();
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
    }
}
