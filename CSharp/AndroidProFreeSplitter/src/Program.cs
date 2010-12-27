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
            VersionManager vm = new VersionManager(args[2], args[3]);

            Splitter sp = new Splitter(vm);
            sp.Split(args[0], args[1]);
        }
    }
}
