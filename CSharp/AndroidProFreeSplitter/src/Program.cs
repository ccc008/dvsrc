using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace apfsplitter {
    class Program {
        static void help() {
            System.Console.WriteLine("Android Pro Free Splitter. Using:");
            System.Console.WriteLine("apfsplitter <src project directory> <dest project directory> <config.xml> <version tag>");
        }
        static void Main(string[] args) {
            if (args.Length < 4) {
                help();
                return;
            }
            VersionManager vm = new VersionManager(args[2], args[3], extract_package_name(args[0] + "\\AndroidManifest.xml"));

            Splitter sp = new Splitter(vm, args[0]);
            sp.Split(args[0], args[1]);
        }

        static String extract_package_name(String fnAndroidManifest) {
            XmlDocument xml = new XmlDocument();
            xml.Load(fnAndroidManifest);
            return xml.SelectSingleNode("./manifest/@package").InnerText;
        }
    }
}
