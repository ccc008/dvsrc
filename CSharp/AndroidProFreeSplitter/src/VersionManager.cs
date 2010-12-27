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
        enum tregex {
            X_DIRS_COPY
            , X_FILES_COPY
            , I_DIRS_COPY
            , I_FILES_COPY
            , X_FILES_PROCESS
            , I_FILES_PROCESS
        }
        const int COUNT_REGEX = 6;
        Regex[] m_Regex = new Regex[COUNT_REGEX];

        public VersionManager(String xmlConfig, String tagVersion) {
            m_TagVersion = tagVersion;

            m_XmlConfig = new XmlDocument();
            m_XmlConfig.Load(xmlConfig);
            String[] xpath_tags = new string[COUNT_REGEX] {
                "exclude_directories", "exclude_files", "include_directories", "include_files"
                , "exclude_files", "include_files"
            };
            String[] xpath_types = new string[COUNT_REGEX] {
                "copy", "copy", "copy", "copy", "modify_content", "modify_content"
            };
            for (int i = 0; i < COUNT_REGEX; ++i) {
                String sregex = m_XmlConfig.SelectSingleNode(String.Format("./config/version[@type='{0}']/{1}/{2}", tagVersion, xpath_types[i], xpath_tags[i])).InnerText;
                if (sregex == "") sregex = ",";
                m_Regex[i] = new Regex(sregex);
            }
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

        internal List<ContentReplaceInfo> GetReplaceExpressions() {
            List<ContentReplaceInfo> dest = new List<ContentReplaceInfo>();
            foreach (XmlNode node in get_version_node().SelectNodes("content/replace")) {
                dest.Add(new ContentReplaceInfo(
                    node.SelectSingleNode("files").InnerText
                    , node.SelectSingleNode("regexp_search").InnerText
                    , node.SelectSingleNode("regexp_replace").InnerText
                ));
            }
            return dest;
        }

        private XmlNode get_version_node() {
            XmlNode version_node = m_XmlConfig.SelectSingleNode(String.Format("//version[@type='{0}']", m_TagVersion));
            if (version_node == null) throw new Exception("Couldn't find version tag for version " + m_TagVersion);
            return version_node;
        }
    }
}
