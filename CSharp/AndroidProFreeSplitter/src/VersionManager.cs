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
        private List<ContentReplaceInfo> m_ListReplacements = new List<ContentReplaceInfo>();
        enum tregex {
            X_DIRS_COPY
            , X_FILES_COPY
            , I_DIRS_COPY
            , I_FILES_COPY
            , X_FILES_PROCESS
            , I_FILES_PROCESS
            , I_FILES_POSTFIX
        }
        const int COUNT_REGEX = 7;
        Regex[] m_Regex = new Regex[COUNT_REGEX];

        public VersionManager(String xmlConfig, String tagVersion, String packageName) {
            m_TagVersion = tagVersion;

            m_XmlConfig = new XmlDocument();
            m_XmlConfig.Load(xmlConfig);
            String[] xpath_tags = new string[COUNT_REGEX] {
                "exclude_directories", "exclude_files", "include_directories", "include_files"
                , "exclude_files", "include_files"
                , "include_files"
            };
            String[] xpath_types = new string[COUNT_REGEX] {
                "copy", "copy", "copy", "copy"
                , "modify_content", "modify_content"
                , "package"
            };
            for (int i = 0; i < COUNT_REGEX; ++i) {
                String sfile_mask = m_XmlConfig.SelectSingleNode(
                    String.Format("./config/version[@type='{0}']/{1}/{2}", tagVersion, xpath_types[i], xpath_tags[i])).InnerText;
                if (sfile_mask == "") sfile_mask = ",";
                m_Regex[i] = new Regex(Utils.FileMask2Regexp(sfile_mask));
            }

            this.AdditionalPrefixForPackageName = m_XmlConfig.SelectSingleNode(
                String.Format("./config/version[@type='{0}']/package/postfix", tagVersion)).InnerText;
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

        internal List<ContentReplaceInfo> GetReplaceExpressions() {
            return m_ListReplacements;
        }

        private void generate_list_replacements() {
            m_ListReplacements = new List<ContentReplaceInfo>();
            foreach (XmlNode node in get_version_node().SelectNodes("content/replace")) {
                m_ListReplacements.Add(new ContentReplaceInfo(
                    node.SelectSingleNode("files").InnerText
                    , node.SelectSingleNode("search").InnerText
                    , node.SelectSingleNode("replace").InnerText
                    , get_kind(node, "kind", treplace_kinds.REGEX)
                ));
            }
            m_ListReplacements.Add(new ContentReplaceInfo(
                get_version_node().SelectSingleNode("package/include_files").InnerText
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

        private XmlNode get_version_node() {
            XmlNode version_node = m_XmlConfig.SelectSingleNode(String.Format("//version[@type='{0}']", m_TagVersion));
            if (version_node == null) throw new Exception("Couldn't find version tag for version " + m_TagVersion);
            return version_node;
        }
    }
}
