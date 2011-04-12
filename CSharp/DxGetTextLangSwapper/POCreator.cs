using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DxGetTextLangSwapper {
    /// <summary>
    /// TODO: THis class is NOT IMPLEMENTED
    /// </summary>
    class POCreator {
        private static Regex m_SingleLineComment = new Regex(".+?//(.+)");
        private static Regex m_MultyLineCommentStartEnd = new Regex("[^{]+{([^}]+)}.*");
        private static Regex m_MultyLineCommentStart = new Regex("[^{]+{(.*)");
        private static Regex m_MultyLineCommentEnd = new Regex("([^}]*)}");
        private readonly String m_FileName;
        private class Entry {
            public readonly String Msgid;
            public readonly String FileName;
            public readonly int LineNumber;
            public Entry(String msgid, String fileName, int lineNumber) {
                this.Msgid = msgid;
                this.FileName = fileName;
                this.LineNumber = lineNumber;
            }
        }
        private Dictionary<String, List<Entry>> m_Entries = new Dictionary<String, List<Entry>>();
        public POCreator(String fileName) {
            m_FileName = fileName;
        }
        internal void ExtractComments(String srcFileName) {
            String[] m_Lines = Utils.LoadStringsFromFile(srcFileName, Program.DFM_FILES_ENCODING);
            int nline = 0;
            while (nline < m_Lines.Length) {
                int step = 0;
                String comment = extract_singleline_comment(m_Lines, nline);
                if (comment == null) {
                    comment = extract_multyline_comment(m_Lines, nline, ref step);
                } else {
                    step = 1;
                }
                if (comment != null) {
                    if (! m_Entries.ContainsKey(comment)) {
                        m_Entries.Add(comment, new List<Entry>());
                    } else {
                        m_Entries[comment].Add(new Entry(comment, srcFileName, nline));                    
                    }
                }
                nline += step;
            }
        }

        internal void CreateNewPOFile(String destPO) {
            throw new NotImplementedException();
        }

        private String extract_singleline_comment(String[] lines, int line) {
            Match m = m_SingleLineComment.Match(lines[line]);
            if (m.Success) {
                return m.Groups[1].Captures[0].ToString();
            }
            return null;
        }
        private String extract_multyline_comment(String[] lines, int line, ref int destStep) {
            destStep = 1;
            Match m = m_MultyLineCommentStartEnd.Match(lines[line]);
            if (m.Success) {
                return m.Groups[1].Captures[0].ToString();
            } else {
                m = m_MultyLineCommentStart.Match(lines[line]);
                if (! m.Success) return null;
                StringBuilder sb = new StringBuilder();
                sb.Append(m.Groups[1].Captures[0].ToString());
                while (line + destStep < lines.Length) {
                    //!TODO
                }
            }
            return null;
        }
    }
}
