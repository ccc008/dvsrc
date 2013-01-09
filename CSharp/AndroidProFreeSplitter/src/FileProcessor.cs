using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace apfsplitter {
    /// <summary>
    /// Manage list of files and folders that should be excluded from copy process.
    /// </summary>
    public class FileProcessor {
        private List<Regex> _NamesToIgnoreRE = new List<Regex>();
        private List<ModifyContent> _ModifyContent = new List<ModifyContent>();
        private List<String> _FoldersToIgnore = new List<String>();
        private List<String> _SkipIfExists = new List<String>();
        public FileProcessor(String fnXmlConfig) {
            XmlDocument xml = new XmlDocument();
            xml.Load(fnXmlConfig);
            XmlNode root = xml.SelectSingleNode("data");
            foreach (XmlNode node in root.SelectNodes("//modify_content")) {
                _ModifyContent.Add(new ModifyContent(node));
            }
            XmlNode node_exclude = root.SelectSingleNode("//exclude_names");
            if (node_exclude != null) {
                String[] list_e = node_exclude.InnerText.Split('\n');
                foreach(String s in list_e) {
                    if (s.Trim() == "") continue;
                    _NamesToIgnoreRE.Add(new Regex(Utils.FileMask2Regexp(s)));
                }
            }
            XmlNode node_exclude_paths = root.SelectSingleNode("//exclude_paths");
            if (node_exclude_paths != null) {
                String[] ignores = node_exclude_paths.InnerText.Split('\n');
                foreach (String s in ignores) {
                    if (s.Trim() == "") continue;
                    _FoldersToIgnore.Add(s.ToLower().Trim());
                }
            }
            XmlNode node_skip_if_exists = root.SelectSingleNode("//skip_if_exists");
            if (node_skip_if_exists != null) {
                String[] ignores = node_skip_if_exists.InnerText.Split('\n');
                foreach (String s in ignores) {
                    if (s.Trim() == "") continue;
                    _SkipIfExists.Add(s.ToLower().Trim());
                }
            }
        }
        public bool SkipCopyFile(String destDir, String fileName) {
            String dest_fn = destDir + "\\" + fileName;
            return match_to_regex(System.IO.Path.GetFileName(fileName))
                || exclude_path(dest_fn, _FoldersToIgnore, false)
                || exclude_path(dest_fn, _SkipIfExists, true);
        }
        public bool SkipCopyDirectory(String destDir, String dirName) {
            return SkipCopyFile(destDir, dirName);
        }
        public void Modify(String fileName) {
            foreach (var mc in _ModifyContent) {
                if (mc.Match(fileName)) {
                    mc.Modify(fileName);
                }
            }
        }
        private bool match_to_regex(String fileName) {
            foreach (Regex r in _NamesToIgnoreRE) {
                if (r.Match(fileName).Success) {
                    return true;
                }
            }
            return false;
        }
        private bool exclude_path(String path, List<String> paths, bool onlyIfExists) {
            String low_path = path.Replace("\\", "/").ToLower();
            foreach (String p in paths) {
                if (low_path.EndsWith(p)) {
                    return onlyIfExists
                        ? (System.IO.File.Exists(path) || System.IO.Directory.Exists(path))
                        : true;
                }
            }
            return false;
        }

        private class ModifyContent {
            private readonly List<Regex> _FileMasks = new List<Regex>();
            private readonly String _sfrom;
            private readonly String _sto;
            public ModifyContent(XmlNode node) {
                foreach (String s in node.InnerText.Split('\n')) {
                    if (s.Trim() == "") continue;
                    _FileMasks.Add(new Regex(Utils.FileMask2Regexp(s)));
                }
                _sfrom = node.Attributes["from"].InnerText;
                _sto = node.Attributes["to"].InnerText;
            }
            public bool Match(String fileName) {
                foreach (Regex r in _FileMasks) {
                    if (r.Match(fileName).Success) {
                        return true;
                    }
                }
                return false;

            }
            public void Modify(String fileName) {
                String content = File.ReadAllText(fileName);
                Regex r = new Regex(_sfrom);
                String dest = r.Replace(content, _sto);
                if (content != dest) {
                    File.WriteAllText(fileName, dest);
                }
            }
        }
    }
}
