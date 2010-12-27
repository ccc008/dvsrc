using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace apfsplitter {
    class Splitter {
        private VersionManager m_Vm;
        public Splitter(VersionManager vm) {
            m_Vm = vm;
        }

        public void Split(String srcDir, String destDir) {
            foreach (String src_file in Directory.GetFiles(srcDir, "*.*")) {
                if (m_Vm.SkipCopyFile(srcDir, Path.GetFileName(src_file))) continue;

                String fndest = destDir + "\\" + Path.GetFileName(src_file);
                File.Copy(src_file, fndest, true);

                if (!m_Vm.SkipProcessFile(srcDir, src_file)) {
                    process_file(fndest);
                }
            }

            foreach(String src_dir in Directory.GetDirectories(srcDir, "*.*")) {
                if (m_Vm.SkipCopyDirectory(srcDir, Path.GetFileName(src_dir))) continue;

                String dest_dir = destDir + "\\" + Path.GetFileName(src_dir);
                Directory.CreateDirectory(dest_dir);

                Split(src_dir, dest_dir);
            }
        }

        void process_file(String fileName) {
            //modify file content
            String content = File.ReadAllText(fileName);

//             foreach (ContentDeleteInfo cdi in m_Vm.GetDeleteExpressions(versionTag)) {
//                 if (cdi.IsFileApplicable(fileName)) {
//                     content = delete_content(cdi, content);
//                 }
//             }
            foreach (ContentReplaceInfo cri in m_Vm.GetReplaceExpressions()) {
                if (cri.IsFileApplicable(fileName)) {
                    content = replace_content(cri, content);
                }
            }

            File.WriteAllText(fileName, content);
        }

        String replace_content(ContentReplaceInfo cri, string srcContent) {
            Regex r = new Regex(cri.SearchString);
            return r.Replace(srcContent, cri.ReplaceString);
        }

//         String delete_content(ContentDeleteInfo cdi, string srcContent) {
//             Regex r = new Regex(cri.SearchString);
//             return r.Replace(srcContent, "");
//         }
    }
}
