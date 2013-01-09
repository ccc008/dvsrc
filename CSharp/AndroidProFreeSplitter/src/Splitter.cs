using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace apfsplitter {
    class Splitter {
        VersionManager m_Vm;
        String m_PackageDirectory;
        public Splitter(VersionManager vm, String srcDir) {
            m_Vm = vm;
            m_PackageDirectory = vm.OriginalPackageName.Replace(".", "\\").ToLower();
        }

        public void Split(String srcDir, String destDir) {
            foreach (String src_file in Directory.GetFiles(srcDir, "*.*")) {
                if (m_Vm.SkipCopyFile(srcDir, Path.GetFileName(src_file))) continue;

                String fndest = destDir + "\\" + Path.GetFileName(src_file);
                File.Copy(src_file, fndest, true);

                if (!m_Vm.SkipProcessFile(srcDir, Path.GetFileName(src_file))) {
                    process_file(fndest);
                }
            }

            String new_packagename = m_Vm.getNewPackageName().Replace(".", "\\");

            foreach(String src_dir in Directory.GetDirectories(srcDir, "*.*")) {
                if (m_Vm.SkipCopyDirectory(srcDir, Path.GetFileName(src_dir))) continue;

                String dest_dir = destDir + "\\" + Path.GetFileName(src_dir);
                if (dest_dir.ToLower().EndsWith(m_PackageDirectory)) {
                    dest_dir = dest_dir.Substring(0, dest_dir.Length - m_PackageDirectory.Length) + new_packagename;
                }

                Directory.CreateDirectory(dest_dir);

                Split(src_dir, dest_dir);
            }
        }

        void process_file(String fileName) {
            //modify file content
            String content = File.ReadAllText(fileName);

            foreach (ContentReplaceInfo cri in m_Vm.GetReplaceExpressions()) {
                if (cri.IsFileApplicable(fileName)) {
                    content = replace_content(cri, content);
                }
            }

            File.WriteAllText(fileName, content);
        }

        String replace_content(ContentReplaceInfo cri, string srcContent) {
            switch (cri.Kind) {
                case treplace_kinds.REGEX:
                    Regex r = new Regex(cri.SearchString); //!TODO: create it once
                    return r.Replace(srcContent, cri.ReplaceString);
                case treplace_kinds.DIRECT:
                    return srcContent.Replace(cri.SearchString, cri.ReplaceString);
                case treplace_kinds.XML:
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(srcContent);

                    XmlNamespaceManager xmlsm = new XmlNamespaceManager(xml.NameTable); 

                    String url = cri.GetParam("xmlns_url");
                    if (url != null) {
                        String xmlns_name = cri.GetParam("xmlns_name");
                        System.Diagnostics.Debug.Assert(xmlns_name != null);

                        xmlsm.AddNamespace(xmlns_name, url);
                    }

                    foreach (XmlNode node in xml.SelectNodes(cri.SearchString, xmlsm)) {
                        node.InnerText = cri.ReplaceString;
                    }
                    return xml.InnerXml;
            }
            return srcContent;
        }
    }
}
