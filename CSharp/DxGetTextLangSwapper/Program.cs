//DxGetTextLangSwapper application
//Copyright by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;

namespace DxGetTextLangSwapper
{
    class Program
    {
        public static int PO_FILE_ENCODING = 65001;
        public static int PAS_FILES_ENCODING = 1252;
        public static int DFM_FILES_ENCODING = 1252;

        /// <summary>
        /// Entries in PO file and source files can be different during encoding problem.
        /// if BRUTOREPLACER turned on, then entry at specified text line is replaced always independent if actual value is equal to value from PO or isn't equal
        /// and program outputs warning with detailed info.
        /// </summary>
        public static Boolean USE_BRUTOREPLACER = true;
        
        static void help() {
            System.Console.WriteLine("DxGetTextLangSwapper, see http://code.google.com/p/dvsrc/"); 
            System.Console.WriteLine("Helper for GNU Gettext for Delphi (http://dxgettext.po.dk/)");
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("DxGetTextLangSwapper <source directory> <source PO file> <target PO file>");
            System.Console.WriteLine("  Swaps languages");
            System.Console.WriteLine("  F.e. if your sources are on Danish, and you have PO file with DK->ENG,");
            System.Console.WriteLine("  then DxGetTextLangSwapper will convert sources on English + create PO: ENG->DK");
            System.Console.WriteLine("");
//             System.Console.WriteLine("DxGetTextLangSwapper -get_comments <source directory> <target PO file>");
//             System.Console.WriteLine("  Extracts all comments //.... from pas files and place them to PO file");
//             System.Console.WriteLine("");
//             System.Console.WriteLine("DxGetTextLangSwapper -put_comments <source directory> <source PO file>");
            //             System.Console.WriteLine("  Replaces all comments //.... in pas files by their translations from PO file");
        }
        static void load_config_params() {
            try {
                string s_po_file_encoding = System.Configuration.ConfigurationManager.AppSettings["PO_FILE_ENCODING"];
                string s_pas_file_encoding = System.Configuration.ConfigurationManager.AppSettings["PAS_FILES_ENCODING"];
                string s_dfm_file_encoding = System.Configuration.ConfigurationManager.AppSettings["DFM_FILES_ENCODING"];
                string use_brutoreplacer = System.Configuration.ConfigurationManager.AppSettings["USE_BRUTOREPLACER"];

                if (s_po_file_encoding != null) {
                    PO_FILE_ENCODING = Int32.Parse(s_po_file_encoding);
                }
                if (s_pas_file_encoding != null) {
                    PAS_FILES_ENCODING = Int32.Parse(s_pas_file_encoding);
                }
                if (s_dfm_file_encoding != null) {
                    DFM_FILES_ENCODING = Int32.Parse(s_dfm_file_encoding);
                }
                if (use_brutoreplacer != null) { 
                    USE_BRUTOREPLACER = use_brutoreplacer != "0" && use_brutoreplacer.ToLower() != "false";
                }
            } catch (Exception ex) {
                Console.WriteLine("Failed to load app config file: " + ex.ToString());
            }
        }

        static void Main(string[] args) {
            load_config_params();

            if (args.Length < 4) {
                help();
                return;
            }
//             switch (args[0]) {
//                 case "-swap":
//                     swap_languages(args[1], args[2], args[3]);
//                     break;
//                 case "-get_comments":
//                     extract_comments(args[1], args[2]);
//                     break;
//                 case "-put_comments":
//                     replace_comments(args[1], args[2]);
//                     break;
//              }
            swap_languages(args[0], args[1], args[2]);
        }

        static void swap_languages(String projectDirectory, String srcPO, String destPO) {
            //read PO file and generate list of substitutions
            POParser parser = new POParser(srcPO);

            //one by one read all source files and make changes
            foreach (String f in Directory.GetFiles(projectDirectory, "*.pas", SearchOption.AllDirectories)) {
                parser.ApplySubstitutions(f);
            }
            foreach (String f in Directory.GetFiles(projectDirectory, "*.dfm", SearchOption.AllDirectories)) {
                parser.ApplySubstitutions(f);
            }

            //save result PO file 
            parser.CreateNewPOFile(destPO);
        }

//         private static void extract_comments(String projectDirectory, String destPO) {
//             POCreator creator = new POCreator(destPO);
//             foreach (String f in Directory.EnumerateFiles(projectDirectory, "*.pas")) {
//                 creator.ExtractComments(f);
//             }
//             creator.CreateNewPOFile(destPO);
//         }
// 
//         private static void replace_comments(String projectDirectory, String srcPO) {
//             POParser parser = new POParser(srcPO);
//             foreach (String f in Directory.EnumerateFiles(projectDirectory, "*.pas")) {
//                 parser.ReplaceComments(f);
//             }
//         }

    }
}
