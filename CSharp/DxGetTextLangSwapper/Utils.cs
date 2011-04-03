//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DxGetTextLangSwapper {
    static class Utils {
        public static String[] LoadStringsFromFile(String fileName) {
            List<String> list = new List<string>();
            using (System.IO.StreamReader sr = System.IO.File.OpenText(fileName)) {
                while (!sr.EndOfStream) {
                    list.Add(sr.ReadLine());
                }
            }
            return list.ToArray();
        }

        public static void SaveStringsToFile(String fileName, String[] lines) {
            List<String> list = new List<string>();
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(fileName)) {
                foreach (String line in lines) {
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
    }
}
