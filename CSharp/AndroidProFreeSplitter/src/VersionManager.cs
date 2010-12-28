using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace apfsplitter {
    class VersionManager {
        XmlDocument m_XmlConfig;
        readonly String m_TagVersion;
        public readonly String OriginalPackageName;
        public readonly String AdditionalPrefixForPackageName;
        private readonly String m_OppositeMacrosValue;
        private readonly XmlNode m_VersionNode;
        private readonly bool m_bReverse;
        private List<ContentReplaceInfo> m_ListReplacements = new List<ContentReplaceInfo>();
        enum tregex {
            X_DIRS_COPY
            , X_FILES_COPY
            , I_DIRS_COPY
            , I_FILES_COPY
            , X_FILES_PROCESS
            , I_FILES_PROCESS
            , I_FILES_POSTFIX
            , X_DIRS_CLEAR_DEST_DIR
            , X_FILES_CLEAR_DEST_DIR
        }
        const int COUNT_REGEX = 9;
        Regex[] m_Regex = new Regex[COUNT_REGEX];

        public VersionManager(String xmlConfig, String tagVersion, String packageName, bool bReverseTargetAndOpposite) {
            m_bReverse = bReverseTargetAndOpposite;
            m_TagVersion = tagVersion;

            m_XmlConfig = new XmlDocument();
            m_XmlConfig.Load(xmlConfig);

            m_VersionNode = m_XmlConfig.SelectSingleNode(String.Format("./config/version[@target='{0}']", tagVersion));
            if (m_VersionNode == null) throw new Exception("Couldn't find version tag for version " + m_TagVersion);
            
            m_OppositeMacrosValue = m_VersionNode.SelectSingleNode("@opposite").InnerText;
            if (m_bReverse) {
                String tag = m_TagVersion;
                m_TagVersion = m_OppositeMacrosValue;
                m_OppositeMacrosValue = tag;
            }

            String[] xpath_tags = new string[COUNT_REGEX] {
                "exclude_directories", "exclude_files", "include_directories", "include_files"
                , "exclude_files", "include_files"
                , "include_files"
                , "exclude_directories", "exclude_files"
            };
            String[] xpath_types = new string[COUNT_REGEX] {
                "copy", "copy", "copy", "copy"
                , "modify_content", "modify_content"
                , "package"
                , "clear_dest_directory"
                , "clear_dest_directory"
            };
            for (int i = 0; i < COUNT_REGEX; ++i) {
                String sfile_mask = m_VersionNode.SelectSingleNode(
                    String.Format("{0}/{1}", xpath_types[i], xpath_tags[i])).InnerText;
                if (sfile_mask == "") sfile_mask = ",";
                m_Regex[i] = new Regex(Utils.FileMask2Regexp(expand_macros(sfile_mask)));
            }

            this.AdditionalPrefixForPackageName = expand_macros(m_VersionNode.SelectSingleNode("package/postfix").InnerText);
            this.OriginalPackageName = packageName;


            generate_list_replacements();
        }

        internal bool SkipCopyDirectory(string parentDir, string subDir) {
            return (m_Regex[(int)tregex.X_DIRS_COPY].Match(subDir).Success
                || !m_Regex[(int)tregex.I_DIRS_COPY].Match(subDir).Success);
        }

        internal bool SkipCopyFile(string parentDir, string fileName) {
            return (m_Regex[(int)tregex.X_FILES_COPY].Match(fileName).Success
                || !m_Regex[(int)tregex.I_FILES_COPY].Match(fileName).Success);
        }

        internal bool SkipProcessFile(string parentDir, string fileName) {
            return (m_Regex[(int)tregex.X_FILES_PROCESS].Match(fileName).Success
                || !m_Regex[(int)tregex.I_FILES_PROCESS].Match(fileName).Success);
        }

        internal bool SkipPostfixFile(string parentDir, string fileName) {
            return !m_Regex[(int)tregex.I_FILES_POSTFIX].Match(fileName).Success;
        }

        internal bool SkipClearDestDirFile(string parentDir, string fileName) {
            return (m_Regex[(int)tregex.X_FILES_CLEAR_DEST_DIR].Match(fileName).Success);
        }

        internal bool SkipClearDestDirDirectory(string parentDir, string subDir) {
            return (m_Regex[(int)tregex.X_DIRS_CLEAR_DEST_DIR].Match(subDir).Success);
        }

        internal List<ContentReplaceInfo> GetReplaceExpressions() {
            return m_ListReplacements;
        }

        private void generate_list_replacements() {
            m_ListReplacements = new List<ContentReplaceInfo>();
            foreach (XmlNode node in m_VersionNode.SelectNodes("content/replace")) {
                if (!is_replace_rule_applicable(node)) continue;
                m_ListReplacements.Add(new ContentReplaceInfo(
                    node.SelectSingleNode("files").InnerText
                    , expand_macros(node.SelectSingleNode("search").InnerText)
                    , expand_macros(node.SelectSingleNode("replace").InnerText)
                    , get_kind(node, "kind", treplace_kinds.REGEX)
                ));
            }
            m_ListReplacements.Add(new ContentReplaceInfo(
                m_VersionNode.SelectSingleNode("package/include_files").InnerText
                , OriginalPackageName
                , AdditionalPrefixForPackageName.Length != 0 
                    ? OriginalPackageName + "." + AdditionalPrefixForPackageName
                    : OriginalPackageName
                , treplace_kinds.DIRECT
            ));
        }

        private treplace_kinds get_kind(XmlNode parentNode, string subNodeName, treplace_kinds defaultValue) {
            XmlNode node = parentNode.SelectSingleNode(subNodeName);
            if (node == null) return defaultValue;
            switch (node.InnerText) {
                case "regex": return treplace_kinds.REGEX;
                case "direct": return treplace_kinds.DIRECT;
                case "xml": return treplace_kinds.XML;
                default: return defaultValue;
            }
        }

        /// <summary>
        /// replace #target# and #opposite# by pro/free or free/pro
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        private String expand_macros(String srcStr) {
            String dest = srcStr.Replace("#target#", m_TagVersion)
                .Replace("#opposite#", m_OppositeMacrosValue)
                .Replace("#TARGET#", m_TagVersion.ToUpper())
                .Replace("#OPPOSITE#", m_OppositeMacrosValue.ToUpper());
            if (dest.Contains("#target?")) {
                Regex r = new Regex("#target\\?([^:#]*):([^#]*)#", RegexOptions.Compiled);
                Match m = r.Match(dest);
                if (m.Success) {
                    if (! m_bReverse) {
                        return m.Groups[1].Captures[0].ToString();
                    } else {
                        return m.Groups[2].Captures[0].ToString();
                    }
                }
            }
            return dest;
        }

        private bool is_replace_rule_applicable(XmlNode srcNode) {
            XmlNode node = srcNode.SelectSingleNode("@if");
            if (node == null) return true;

            return node.InnerText == String.Format("target:{0}", m_TagVersion);
        }
    }
}
