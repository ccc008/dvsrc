//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DxGetTextLangSwapper {
    /// <summary>
    /// Class to read and modify content of Delphi text dfm files
    /// </summary>
    public class DFMWrapper {
        private static Regex m_RegexObjectStart = new Regex("\\s*object\\s*([^:]+):.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex m_RegexObjectEnd = new Regex("$\\s*end\\s*^", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex m_RegexPathSplitter = new Regex("\\.\\.", RegexOptions.Compiled);
        private static Regex m_RegexPropertyExtractor = new Regex("\\s*([^\\s]+)\\s*=\\s*(.+)\\s*", RegexOptions.Compiled);
        private static Regex m_RegexUnqoute = new Regex("'(.+?)'", RegexOptions.Compiled);
        private readonly String[] m_Lines;
        public DFMWrapper(String srcFileName) {
            m_Lines = Utils.LoadStringsFromFile(srcFileName);
        }

        /// <summary>
        /// Typical dfm structure is:
        /// object A: TA
        ///     object B: TB
        ///     
        ///     end
        ///     object C: TC
        ///         Property = 'string_value'  //variant #1
        ///         Property2 = 10             //variant #2
        ///         Memo.UTF8W = (             //variant #3  
        ///           'memotext')
        ///     end
        /// end
        /// "path" has format like "A..B..C"
        /// Replaces propertyValue from msgId to msgStr IF AND ONLY IF the value equals to msgId
        /// </summary>        
        public int Replace(String path, String propertyName, String msgId, String msgStr) {
            if (msgStr == "") {
                return 0; //there is no tranlsation; skip it
            }
            String[] paths = m_RegexPathSplitter.Split(path);
            int i = find_line(paths);
            if (i == -1) {
                System.Console.WriteLine("Warning: path not found {0} {1}", path, propertyName);
                return 0; //path not found
            }

            //search line with property
            ++i;
            while (i < m_Lines.Length) {
                if (!is_propertyline(m_Lines[i])) {
                    System.Console.WriteLine("Warning: property not found {0} {1}", path, propertyName);
                    return 0; //property not found
                }
                KeyValuePair<String, String> kvp = extract_property_name_value(m_Lines[i]);
                if (kvp.Key == propertyName) {
                    String value = kvp.Value;
                    if (value.Length != 0 && value[0] == '\'') { //string value #1
                        value = m_RegexUnqoute.Replace(value, "$1");
                        if (value == msgId) {
                            m_Lines[i] = m_Lines[i].Replace(msgId, msgStr);
                            return 1;
                        } else {
                            System.Console.WriteLine("Warning: value is different, skip it; {0} {1}: '{2}' != '{3}'", path, propertyName, value, msgId);
                            return 0; //value is different; don't change it
                        }
                    } else if (value.StartsWith("(")) { //memo value, #3
                        if (find_memo_line(msgId, ref i)) {
                            m_Lines[i] = m_Lines[i].Replace(msgId, msgStr);
                            return 1;
                        } else {
                            System.Console.WriteLine("Warning: can't line string in memo array, skip it; {0} {1}: '{2}'", path, propertyName, msgId);
                            return 0; //value is different; don't change it
                        }
                    }
                }

                ++i;
            }
            return 0; 
        }

        /// <summary>
        /// Save modified content to file
        /// </summary>
        public void Save(String destFileName) {
            Utils.SaveStringsToFile(destFileName, m_Lines);
        }

        private KeyValuePair<String, String> extract_property_name_value(String line) {
            Match m = m_RegexPropertyExtractor.Match(line);
            if (! m.Success) throw new Exception("Can't parse key = 'value' pair from: " + line);
            return new KeyValuePair<String, String>(m.Groups[1].Captures[0].ToString()
                , m.Groups[2].Captures[0].ToString().Trim());
        }

        private int find_line(String[] paths) {
            int line = 0;
            foreach (String path in paths) {
                if (path == "") continue;
                if (!find_object(path, ref line)) return -1;
            }
            return line;
        }

        private bool find_object(String path, ref int line) {
            int stack = 0;
            while (line < m_Lines.Length) {
                String s = "";
                if (is_object_start(m_Lines[line], ref s)) {
                    if (s == path) return true;
                    ++stack;
                } else if (is_object_end(m_Lines[line])) {
                    if (stack == 0) throw new Exception("Wrong 'end' in dfm file, line " + line.ToString());
                    --stack;
                }
                ++line;
            }
            return false;
        }

        private static bool is_object_start(String line, ref String objectName) {
            Match m = m_RegexObjectStart.Match(line);
            if (!m.Success) return false;
            objectName = m.Groups[1].Captures[0].ToString();
            return true;
        }
        private static bool is_object_end(String line) {
            return m_RegexObjectEnd.Match(line).Success;
        }
        private static bool is_propertyline(String line) {
            String dummy = "";
            return !is_object_end(line) && !is_object_start(line, ref dummy) 
                && line.Contains('=')
                // Example:
                //   Memo.UTF8W = (
                //    'Navn')   // this line is not key=value line. 

            ; 
        }

        /// <summary>
        /// Some dfm properties have "memo" format like:
        /// Memo.UTF8W = (
        ///   'Navn') 
        /// Find line with 'msgid' or return false if end bracket is reached
        /// </summary>
        private bool find_memo_line(String msgid, ref int line) {
            String dummy = "";
            while (line < m_Lines.Length) {
                if (is_object_end(m_Lines[line])) return false;
                if (is_object_start(m_Lines[line], ref dummy)) return false;
                if (m_Lines[line].Contains(msgid)) return true;
                if (m_Lines[line].Trim().EndsWith(")")) return false; //end of memo 
                ++line;
            }
            return false;
        }
    }
}
