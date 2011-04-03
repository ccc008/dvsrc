//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DxGetTextLangSwapper
{
    class Program
    {
        static void help() {
            System.Console.WriteLine("DxGetTextLangSwapper <source directory> <source PO file> <target PO file>");
            System.Console.WriteLine("Helper for GNU Gettext for Delphi (http://dxgettext.po.dk/)");
            System.Console.WriteLine("Swap languages in source files");
            System.Console.WriteLine("F.e. you have sources with GUI on Danish + PO file with DK->ENG translation.");
            System.Console.WriteLine("Using DxGetTextLangSwapper you can make conversion.");
            System.Console.WriteLine("You will get sources with GUI on English + PO file with ENG->DK translation.");
            System.Console.WriteLine("UTF-8 is assumed for all files");
        }

        static void Main(string[] args) {
            if (args.Length < 3) {
                help();
                return;
            }
            //read PO file and generate list of substitutions
            POParser parser = new POParser(args[1]);            

            //one by one read all source files and make changes
            foreach (String f in Directory.EnumerateFiles(args[0], "*.pas")) {
                parser.ApplySubstitutions(f);
            }
            foreach (String f in Directory.EnumerateFiles(args[0], "*.dfm")) {
                parser.ApplySubstitutions(f);
            }

            //save result PO file 
            parser.CreateNewPOFile(args[2]);
        }
    }
}
