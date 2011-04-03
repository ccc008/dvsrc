//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DxGetTextLangSwapper {
    public class Substitution {
        private Dictionary<String, List<LocationInfo>> m_Files = new Dictionary<String, List<LocationInfo>>();
        private static Regex m_RegexFilename = new Regex("(.+):(.*)", RegexOptions.Compiled); //CustomerFrame.dfm:64
        private static Regex m_RegexLocation = new Regex("(.+)\\.\\.(.+)\\s*", RegexOptions.Compiled); //Settings_Frame..SpeedButton2..Font.Name
        private static Regex m_RegexLocationResourceStrings = new Regex("Programmer's name for it:\\s*(.+)\\s*", RegexOptions.Compiled); //Programmer's name for it: LoginFrm_CancelBtn

        public String Msgid { get; private set; }
        public String Msgstr { get; private set; }
            
        public Substitution(String msgid, String msgstr, List<String> listDots, List<String> listColumns) {
            this.Msgid = msgid;
            this.Msgstr = msgstr;

            System.Diagnostics.Debug.Assert(listDots.Count == listColumns.Count || listDots.Count == 0);
            int i = 0;
            foreach (String column_string in listColumns) {
                String dot = listDots.Count == 0 ? null : listDots[i];
                ++i;

                KeyValuePair<String, String> filename_line = extract_filename(column_string);

                String filename = filename_line.Key.ToLower();
                if (!m_Files.ContainsKey(filename)) {
                    m_Files.Add(filename, new List<LocationInfo>());
                }
                List<LocationInfo> entries = m_Files[filename];
                bool is_pas_file = System.IO.Path.GetExtension(filename).ToLower() == ".pas";
                if (!is_pas_file && dot == null) {
                    throw new Exception("Location isn't specified: " + filename + " " + column_string);
                }
                KeyValuePair<String, String> path_propertyname = is_pas_file
                    ? new KeyValuePair<String, String>(null, null)
                    : extract_location_from_dot(dot);
                int line;
                if (!Int32.TryParse(filename_line.Value, out line)) {
                    System.Console.WriteLine("Warning: wrong line number {0}, {1} {2} {3}", column_string, filename_line.Value, msgid, msgstr);
                } else {
                    entries.Add(new LocationInfo(path_propertyname.Key, path_propertyname.Value, line));
                }
            }
        }

        public int Apply(String fileName, String[] fileLines) {
            String fn = System.IO.Path.GetFileName(fileName).ToLower();
            if (! m_Files.ContainsKey(fn)) return -1;

            int counter_replaces = 0;
            List<LocationInfo> entries = m_Files[fn];
            foreach (LocationInfo info in entries) {
                String srcStr = String.Format("'{0}'", this.Msgid);
                if (fileLines[info.Line - 1].Contains(srcStr)) {
                    fileLines[info.Line - 1] = fileLines[info.Line - 1].Replace(srcStr, String.Format("'{0}'", this.Msgstr));
                    counter_replaces += 1;
                } 
            }
            return counter_replaces;
        }

        public int Apply(String fileName, DFMWrapper dfm) {
            String fn = System.IO.Path.GetFileName(fileName).ToLower();
            if (!m_Files.ContainsKey(fn)) return -1;

            int counter_replaces = 0;
            List<LocationInfo> entries = m_Files[fn];
            foreach (LocationInfo info in entries) {
                counter_replaces += dfm.Replace(info.Path, info.PropertyName, Msgid, Msgstr);
            }
            return counter_replaces;
        }

        /// <summary>
        /// Source lines is like: "itemframe.dfm:412"
        /// extracts pair "filename"-"value", i.e. "itemframe.dfm" - "412"
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private KeyValuePair<String, String> extract_filename(string column) {
            Match m = m_RegexFilename.Match(column);
            if (!m.Success) throw new Exception("Incorrect filename: " + column);
            return new KeyValuePair<String, String>(m.Groups[1].Captures[0].ToString(), m.Groups[2].Captures[0].ToString());
        }

        /// <summary>        
        /// Returns path - propernyName pair
        /// i.e. PricelistFrm..Rap..Page1..ReportTitle1..Memo3....Memo.UTF8W
        /// is converted to
        /// "PricelistFrm..Rap..Page1..ReportTitle1..Memo3.." and "Memo.UTF8W"
        /// </summary>
        private KeyValuePair<String, String> extract_location_from_dot(String dot) {
            Match m = m_RegexLocation.Match(dot);
            if (m.Success) {
                return new KeyValuePair<String, String>(m.Groups[1].Captures[0].ToString(), m.Groups[2].Captures[0].ToString());
            } else {
                m = m_RegexLocationResourceStrings.Match(dot);
                if (m.Success) {
                    return new KeyValuePair<String, String>("Programmer's name for it" //!TODO: manage resource strings
                        , m.Groups[1].Captures[0].ToString());
                } else {
                    throw new Exception("Incorrect location format: " + dot);
                }
            }
        }

        private class LocationInfo {
            public String Path { get; private set; }
            public String PropertyName { get; private set; }
            public int Line;
            public LocationInfo(String path, String propertyName, int line) {
                this.Path = path;
                this.PropertyName = propertyName;
                this.Line = line;
            }
        }
    }
}
