using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace apfsplitter {
    class Utils {
        public static String FileMask2Regexp(String fileMask) {
        	Regex esc = new Regex("([\\^\\.\\$\\|\\(\\)\\+\\/\\\\])"); //dont' escape \\[\\]\\*\\? 
	        String res = esc.Replace(fileMask.Replace("*.*", "*"), "\\$1"); //see http://stackoverflow.com/questions/1252992/how-to-escape-a-string-for-use-in-boost-regex
            
            String s = res 
                .Replace(";", "|")
                .Replace("*", ".*")
                .Replace("?", ".?");
            return s;
        }
    }
}
