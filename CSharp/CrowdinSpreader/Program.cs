//Copyright 2012 by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//blog: http://derevyanko.blogspot.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace CrowdinSpreader {
    /// <summary>
    /// This application is helper for http://crowdin.net
    /// Crowdin service allows to download translated resources as ZIP with structure:
    /// subdir (=language code, i.e. af, ar, az, etc)
    ///    translated files 
    /// 
    /// Example of ZIP content:
    /// af\strings.xml
    /// en\strings.xml
    /// ru\strings.xml
    /// ...
    /// zh\strings.xml
    /// 
    /// CrowdinSpreader helps to copy files from unpacked ZIP archive to source Android project.
    /// It copies only files that has been translated and are different from original untranslated file.
    /// 
    /// 
    /// Example of xml rules file:
        // <?xml version="1.0" encoding="UTF-8"?>
        // <rules>
        // 	<rule origin="en\strings.xml"
        // 		copyto="res\values-##LANG##\strings.xml"
        // 	/>
        // 	<rule origin="en\quick_start.htm"
        // 		copyto="assets\quick_start_##LANG##.htm"
        // 	/>
        // 
        // 	<lang id="thl-AA" renameTo=""/> <!-- = ignore this language-->
        // 	<lang id="zh-CN" renameTo="zh"/>
        // </rules>
    /// </summary>
    class Program {
        static void help() {
            Console.WriteLine("CrowdinSpreader.exe <rules.xml> <source diretory> <target directory>");
            Console.WriteLine("  source directory contains unpacked translated files from crowdin.net");
        }
        static void Main(string[] args) {
            if (args.Length < 3) {
                help();
                return;
            }
            String source_dir = args[1].Replace("\"", "");
            String target_dir = args[2].Replace("\"", "");

            XmlDocument xml_rules = new XmlDocument();
            xml_rules.Load(args[0]);

        //read list of languages that should be ignored or renamed
            Dictionary<String, String> langs = new Dictionary<String, String>();
            foreach (XmlNode node in xml_rules.SelectNodes("//lang")) {
                langs.Add( node.SelectSingleNode("@id").InnerText,  node.SelectSingleNode("@renameTo").InnerText);
            }

        //read list of rules and apply each rule
            foreach (XmlNode node in xml_rules.SelectNodes("//rule")) {
                Rule r = new Rule(node);
                StringBuilder sb = new StringBuilder();
                apply_rule(r, source_dir, target_dir, langs, sb);
                Console.WriteLine(sb.ToString());
            }
        }

        class Rule {
            public Rule(XmlNode node) {
                this.Origin = node.SelectSingleNode("@origin").InnerText;
                this.CopyTo = node.SelectSingleNode("@copyto").InnerText;
            }
            /// <summary>
            /// Original file with language code equals "en".
            /// Example: en\strings.xml
            /// </summary>
            public String Origin { get; private set; }
            /// <summary>
            /// Target path where translated file should be put
            /// Example: assets\quick_start_##LANG##.htm
            /// ##LANG## will be replaced by appropriate language code.
            /// </summary>
            public String CopyTo { get; private set; }
        }

        private static void apply_rule(Rule r, string sourceDir, string targetDir, Dictionary<String, String> langs, StringBuilder log) {
            String source_path = sourceDir + '\\' + r.Origin; //i.e. CrowdinUnpackedArchive/en/strings.xml
            Int32 origin_crc = CRC.GetCrc32(GetBinaryFileContent(source_path));
            String filename = System.IO.Path.GetFileName(r.Origin); //i.e. strings.xml
            log.Append(filename + ':');

            foreach (String lang_dir in System.IO.Directory.GetDirectories(sourceDir)) {
                String lang_code = System.IO.Path.GetFileName(lang_dir); //i.e. de, ru, du, hu, etc.
                String renamed_lang_code;
                if (langs.TryGetValue(lang_code, out renamed_lang_code)) {
                    if (renamed_lang_code.Length == 0) continue; //language should be ignored
                    lang_code = renamed_lang_code;
                }

                String translated_path = lang_dir + '\\' + filename; //i.e. CrowdinUnpackedArchive/ru/strings.xml
                if (! System.IO.File.Exists(translated_path)) {
                    Console.WriteLine(String.Format("File '{0}' isn't found", translated_path));
                    continue;
                }
                Int32 translated_crc = CRC.GetCrc32(GetBinaryFileContent(translated_path));
                if (origin_crc != translated_crc) {
                    String copyto_path = targetDir + '\\' + r.CopyTo.Replace("##LANG##", lang_code); //i.e. AndroidProject/res/values-ru/strings.xml
                    if (System.IO.File.Exists(copyto_path)) {
                        Int32 copyto_crc = CRC.GetCrc32(GetBinaryFileContent(copyto_path));
                        if (copyto_crc != translated_crc) { //AndroidProject/res/values-ru/strings.xml and CrowdinUnpackedArchive/ru/strings.xml are equal
                            System.IO.File.Delete(copyto_path);
                        }
                    }
                    if (!System.IO.File.Exists(copyto_path)) {
                        String copyto_dir = System.IO.Path.GetDirectoryName(copyto_path);
                        if (! System.IO.Directory.Exists(copyto_dir)) {
                            System.IO.Directory.CreateDirectory(copyto_dir); //i.e. create directory AndroidProject/res/values-ru/
                        }
                        //i.e. copy CrowdinUnpackedArchive/ru/strings.xml to AndroidProject/res/values-ru/strings.xml
                        System.IO.File.Copy(translated_path, copyto_path);
                        log.Append(lang_code + ' ');
                    }
                }
            }
        }

        public static byte[] GetBinaryFileContent(String FileName) {
            using (System.IO.FileStream f = new System.IO.FileStream(FileName, System.IO.FileMode.Open)) {
                byte[] bytes = new byte[f.Length];
                Int32 readbytes = f.Read(bytes, 0, (Int32)f.Length);
                System.Diagnostics.Debug.Assert(f.Length == readbytes);
                return bytes;
            }
        }

    }
}
