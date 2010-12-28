using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace apfsplitter {
    class Utils {
        public static String FileMask2Regexp(String fileMask) {
            String[] masks = fileMask.Replace("*.*", "*").Split(';');
            StringBuilder sb = new StringBuilder();

            Regex esc = new Regex("([\\^\\.\\$\\|\\(\\)\\+\\/\\\\])"); //dont' escape \\[\\]\\*\\? 
            foreach (String mask in masks) {
                String res = esc.Replace(mask.Trim(), "\\$1") //see http://stackoverflow.com/questions/1252992/how-to-escape-a-string-for-use-in-boost-regex
                    .Replace("*", ".*")
                    .Replace("?", ".?");
                if (sb.Length != 0) sb.Append("|");
                sb.Append(res);
            }
            return sb.ToString();
        }
    }
}
