//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DxGetTextLangSwapper {
    static class Utils {
        public static String[] LoadStringsFromFile(String fileName, Int32 encoding) {
            List<String> list = new List<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName, System.Text.Encoding.GetEncoding(encoding))) {
                while (!sr.EndOfStream) {
                    list.Add(sr.ReadLine());
                }
            }
            return list.ToArray();
        }

        public static void SaveStringsToFile(String fileName, String[] lines) {
            List<String> list = new List<string>();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false)) {
                foreach (String line in lines) {
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
    }
}
