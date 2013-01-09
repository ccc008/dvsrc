using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace apfsplitter {
    class Program {
        static void help() {
            System.Console.WriteLine("Android Pro Free Splitter. Using:");
            System.Console.WriteLine("apfsplitter <src project directory> <dest project directory> <config.xml> <version tag>");
            System.Console.WriteLine("apfsplitter --copy <src project directory> <dest project directory> <config.xml>");
        }
        static void Main(string[] args) {
            if (args.Length < 4) {
                help();
                return;
            }
            if (args[0] == "--copy") {
                copy_dirs(args[1], args[2], new FileProcessor(args[3]));
                return;
            }

            String tag_version = args[3];
            bool breverse = tag_version.StartsWith("!");
            if (breverse) tag_version = tag_version.Substring(1, tag_version.Length - 1);

            VersionManager vm = new VersionManager(args[2], tag_version, extract_package_name(args[0] + "\\AndroidManifest.xml"), breverse);

            clear_directory(args[1], vm);

            Splitter sp = new Splitter(vm, args[0]);
            sp.Split(args[0], args[1]);
        }

        static String extract_package_name(String fnAndroidManifest) {
            XmlDocument xml = new XmlDocument();
            xml.Load(fnAndroidManifest);
            return xml.SelectSingleNode("./manifest/@package").InnerText;
        }

        static void clear_directory(String destDir, VersionManager vm) {
            foreach (String dir in System.IO.Directory.GetDirectories(destDir)) {
                if (vm.SkipClearDestDirDirectory(destDir, System.IO.Path.GetFileName(dir))) continue;
                clear_directory(dir, vm);
                if (System.IO.Directory.GetDirectories(dir).Length == 0 && System.IO.Directory.GetFiles(dir).Length == 0) {
                    System.IO.Directory.Delete(dir, true);
                }
            }

            foreach (String sfile in System.IO.Directory.GetFiles(destDir)) {
                if (vm.SkipClearDestDirFile(destDir, System.IO.Path.GetFileName(sfile))) continue;
                System.IO.File.Delete(sfile);
            }
        }
       

        /// <summary>
        /// Copy all files&folders from srcDir to destDir, skip files and folders that match to exception patterns
        /// </summary>
        static void copy_dirs(String srcDir, String destDir, FileProcessor processor) {
            if (!System.IO.Directory.Exists(destDir)) {
                System.IO.Directory.CreateDirectory(destDir);
            }

            foreach (String src_file in Directory.GetFiles(srcDir, "*.*")) {
                if (processor.SkipCopyFile(destDir, Path.GetFileName(src_file))) {
                    continue;
                }

                String fndest = destDir + "\\" + Path.GetFileName(src_file);
                File.Copy(src_file, fndest, true);
                processor.Modify(fndest);
            }

            foreach (String src_dir in Directory.GetDirectories(srcDir, "*.*")) {
                if (processor.SkipCopyDirectory(destDir, Path.GetFileName(src_dir))) {
                    continue;
                }

                String dest_dir = destDir + "\\" + Path.GetFileName(src_dir);
                Directory.CreateDirectory(dest_dir);

                copy_dirs(src_dir, dest_dir, processor);
            }
        }
    }
}
