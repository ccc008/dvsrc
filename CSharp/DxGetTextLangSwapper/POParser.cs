//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DxGetTextLangSwapper
{
    class POParser {
        private readonly List<SubstitutionInfo> m_Substitutions;
        private readonly String[] m_Lines;
        private static Regex m_rDot = new Regex(@"#\.\s*(.+)", RegexOptions.Compiled);
        private static Regex m_rColumn = new Regex(@"#\:\s*(.+)", RegexOptions.Compiled);
        private static Regex m_rMsgid = new Regex("msgid\\s+\"(.*)\"", RegexOptions.Compiled);
        private static Regex m_rMsgstr = new Regex("msgstr\\s+\"(.*)\"", RegexOptions.Compiled);
        private static Regex m_rQuotedString= new Regex("^\"(.+?)\"$", RegexOptions.Compiled);

        public POParser(String fnPO) {
            m_Lines = Utils.LoadStringsFromFile(fnPO, Program.PO_FILE_ENCODING);
            m_Substitutions = load_po_content(fnPO, m_Lines);
        }

        public void ApplySubstitutions(String fileName) {
            if (System.IO.Path.GetExtension(fileName) == ".dfm") {
                apply_dfm(fileName);
            } else {
                apply_txt(fileName);
            }
        }
        private void apply_dfm(String fileName) {
            System.Console.WriteLine("File: " + fileName);
            DFMWrapper dfm = new DFMWrapper(fileName);

            int count_replacements = 0;
            foreach (SubstitutionInfo si in m_Substitutions) {
                int c = si.Substitution.Apply(fileName, dfm);
                if (-1 != c) {
                    si.SuccessfullyApplied = c != 0;
                    count_replacements += c;
                }
            }
            if (count_replacements != 0) {
                dfm.Save(fileName);
                System.Console.WriteLine(fileName + String.Format(" saved; {0} replacements", count_replacements));
            } else {
                System.Console.WriteLine(fileName + String.Format(": save is skipped, there were no replacements"));
            }
        }
        private void apply_txt(String fileName) {
            String[] lines = Utils.LoadStringsFromFile(fileName, Program.PAS_FILES_ENCODING);
            //String content = System.IO.File.ReadAllText(fileName);

            int count_replacements = 0;
            foreach (SubstitutionInfo si in m_Substitutions) {
                int c = si.Substitution.Apply(fileName, lines);
                if (-1 != c) {
                    si.SuccessfullyApplied = c != 0;
                    count_replacements += c;
                }
            }
            if (count_replacements != 0) {
                Utils.SaveStringsToFile(fileName, lines);
                //System.IO.File.WriteAllText(fileName, content);
                System.Console.WriteLine(fileName + String.Format(" saved; {0} replacements", count_replacements));
            } else {
                System.Console.WriteLine(fileName + String.Format(": save is skipped, there were no replacements"));
            }
        }

        public void CreateNewPOFile(String destPOFileName) {
            //swap msgid and msgstr for all substitutions that were successfully applied at least one time
            foreach (SubstitutionInfo si in m_Substitutions) {
                if (si.SuccessfullyApplied) {
                    m_Lines[si.MsgidLine] = "msgid = \"" + si.Substitution.Msgstr + "\"";
                    m_Lines[si.MsgStrLine] = "msgstr = \"" + si.Substitution.Msgid + "\"";
                }
            }

            Utils.SaveStringsToFile(destPOFileName, m_Lines);
        }

        /// <summary>
        /// PO format:
        /// for dfm file:
        /// #.a11
        /// #.a12
        /// #:b11
        /// #:b12
        /// #.c12   
        /// #:c12
        /// msgid "c1"
        /// msgid "d1"
        /// 
        /// #.a12
        /// .....
        /// 
        /// for PAS files:
        /// #:t1
        /// msgid "pas file "
        /// msgid "pas file"
        /// there are no #.xxx in pas files
        /// 
        /// Function read it to list of "substitutions". Each substitution replaces dX by cX in files bXY at places aXY
        /// <returns></returns>
        private List<SubstitutionInfo> load_po_content(string fnPO, string[] lines) {
            List<SubstitutionInfo> dest = new List<SubstitutionInfo>();
            int nline = 0;
            while (nline < lines.Length) {
                String content;
                while (nline < lines.Length && !is_line_dot(lines[nline], out content) && !is_line_column(lines[nline], out content)) ++nline;
                List<String> dots = new List<String>();
                List<String> columns = new List<String>();
                do {
                    if (! (nline < lines.Length)) break;

                    if (is_line_dot(lines[nline], out content)) {
                        dots.Add(content);
                        ++nline;
                        continue;
                    } 
                    if (is_line_column(lines[nline], out content)) {
                        columns.Add(content);
                        ++nline;
                        continue;
                    }
                    break;
                } while (true);
// Strange example:
// #. Nu skal vi gemme
// #: frmLines.pas:257
// #: frmLines.pas:259
// msgid "Der opstod desvР¶rre en fejl under gem"
// msgstr "There was an error while saving."
                if (dots.Count != columns.Count  //dfm files
                    && dots.Count != 0 //pas files
                 ) {
                     dots.Clear();
                     Console.WriteLine(String.Format("Count of #. and #: must be equal, file '{0}' line {1}; dots are ignored", fnPO, nline));
                }
                if (columns.Count == 0) break; //end of file is reached
                String msgid;
                if (!is_line_msgid(lines[nline++], out msgid)) {
                    throw new Exception(String.Format("Can't find msgid, file '{0}' line {1}", fnPO, nline));
                }
                msgid = read_multiline_string(msgid, lines, ref nline);
                String msgstr;
                if (! is_line_msgstr(lines[nline++], out msgstr)) {
                    throw new Exception(String.Format("Can't find msgstr, file '{0}' line {1}", fnPO, nline));
                }
                msgstr = read_multiline_string(msgstr, lines, ref nline);
                dest.Add(new SubstitutionInfo(nline - 2, nline - 1, new Substitution(msgid, msgstr, dots, columns) ));
                while (nline < lines.Length && lines[nline].Trim().Length == 0) ++nline; //skip empty lines                
            }
            return dest;
        }

        /// <summary>
        /// example of multiline msgid:
        /// #. LDM..Rap....ScriptText.Strings
        /// #: LinesMod.dfm:407
        /// msgid "procedure ReportTitle1OnAfterCalcHeight(Sender: "
        /// "TfrxComponent);"
        /// msgstr ""
        /// This function reads second line and returns whole message; lines are divided by \n character.
        /// </summary>
        private static String read_multiline_string(String srcMsg, String[] lines, ref int nline) {
            Match m = m_rQuotedString.Match(lines[nline]);
            if (!m.Success) return srcMsg;

            String dest = srcMsg;
            do {
                dest += "\n" + m.Groups[1].Captures[0].ToString();
                ++nline;
                m = m_rQuotedString.Match(lines[nline]);
            } while (m.Success);

            return dest;
        }

        private bool is_line_dot(string line, out String content) {
            return matched(m_rDot, line, out content);
        }
        private bool is_line_column(string line, out String content) {
            return matched(m_rColumn, line, out content);
        }
        private bool is_line_msgid(string line, out String content) {
            return matched(m_rMsgid, line, out content);
        }
        private bool is_line_msgstr(string line, out String content) {
            return matched(m_rMsgstr, line, out content);
        }
        private bool matched(Regex r, String srcLine, out String content) {
            Match m = r.Match(srcLine);
            if (!m.Success) {
                content = "";
                return false;
            }
            content = m.Groups[1].Captures[0].ToString();
            return true;
        }

        private class SubstitutionInfo {
            public int MsgidLine { get; private set; }
            public int MsgStrLine { get; private set; }
            public Substitution Substitution { get; private set; }
            public Boolean SuccessfullyApplied { get; set; }
            public SubstitutionInfo(int msgidLine, int msgstrLine, Substitution subs) {
                this.MsgidLine = msgidLine;
                this.MsgStrLine = msgstrLine;
                this.Substitution = subs;
                this.SuccessfullyApplied = false; //at least one substitution was made, so lines MsgidLine and MsgStrLine should be swapped.
            }
        }
    }
}
