//Copyright 2012 by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//blog: http://derevyanko.blogspot.com

//"Z:\projects\AnimatedWidget.git\AnimatedWidget\translations\CrowdinSpreaderRules.xml" "Z:\projects\AnimatedWidget.git\AnimatedWidget\translations\crowdin-unzip" "Z:\projects\AnimatedWidget.git\AnimatedWidget\sources"
//"C:\projects\W3D\translations\CrowdinSpreaderRules.xml" "C:\projects\W3D\translations\crowdin_unzip" "C:\projects\W3D\sources\w3d"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

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
		//      remove_untranslated_strings_in_final_file="true" (XML only: remove untranslated android strings)
		//      ignore_spaces="true" (remove all spaces of any kind from source file BEFORE calculating crc)
        //      auto_replace_before_comparing="replace1" (make some replacements BEFORE calculating crc)
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
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
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

                var fn_node = node.SelectSingleNode("@filename");
                this.Filename = fn_node == null
                    ? System.IO.Path.GetFileName(this.Origin)
                    : fn_node.InnerText;
                var node_rt = node.SelectSingleNode("@remove_untranslated_strings_in_final_file");
                this.RemoveUntranslatedStrings = node_rt == null
                    ? false
                    : Int32.Parse(node_rt.InnerText) != 0;
                this.Replacements = new Dictionary<string,string>();
                var node_rbc = node.SelectSingleNode("@auto_replace_before_comparing");
                if (node_rbc != null) {
                    foreach (XmlNode nr in node.ParentNode.SelectNodes(String.Format("auto_replace[@id='{0}']/item", node_rbc.InnerXml))) {
                        String sfrom = nr.Attributes["from"].InnerText;
                        String sto= nr.Attributes["to"].InnerText;
                        this.Replacements.Add(sfrom, sto);
                    }
                }
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
            /// <summary>
            /// Strings like:
            /// Origin: &lt;string name="label_save"&gt;Save&lt;/string&gt;
            /// Translated (the same): &lt;string name="label_save"&gt;Save&lt;/string&gt;
            /// should be removed from dest file after crc-comparing 
            /// </summary>
            public bool RemoveUntranslatedStrings { get; private set; }
            /// <summary>
            /// Set of replacements, that should be made before calculating CRC
            /// </summary>
            public Dictionary<String, String> Replacements { get; private set; }

            public String Filename { get; private set; }
        }

        private static void apply_rule(Rule r, string sourceDir, string targetDir, Dictionary<String, String> langs, StringBuilder log) {
            bool is_format_android_string_resources = System.IO.Path.GetExtension(r.Origin).ToLower() == ".xml";

            String source_path = sourceDir + '\\' + r.Origin; //i.e. CrowdinUnpackedArchive/en/strings.xml
            String original_file_content = getTextFileContent(source_path, r.Replacements, is_format_android_string_resources);
            Int32 origin_crc = getCrc(original_file_content, is_format_android_string_resources);
            String filename = r.Filename;
            log.Append(filename + ':');

            RemoverUntranslatedStrings remover_untranslated_strings = r.RemoveUntranslatedStrings
                ? new RemoverUntranslatedStrings(original_file_content)
                : null;

            foreach (String lang_dir in System.IO.Directory.GetDirectories(sourceDir)) {
               
                String lang_code = System.IO.Path.GetFileName(lang_dir); //i.e. de, ru, du, hu, etc.
                String renamed_lang_code;
                if (langs.TryGetValue(lang_code, out renamed_lang_code)) {
                    if (renamed_lang_code.Length == 0) { //language should be ignored
                        Console.WriteLine(String.Format("Language {0} was ignored", lang_code));
                        continue; 
                    }
                    lang_code = renamed_lang_code;
                }

                String translated_path = lang_dir + '\\' + filename; //i.e. CrowdinUnpackedArchive/ru/strings.xml
                if (! System.IO.File.Exists(translated_path)) {
                    Console.WriteLine(String.Format("File '{0}' isn't found", translated_path));
                    continue;
                }
                String translated_content = getTextFileContent(translated_path, r.Replacements, is_format_android_string_resources); //file with modified content (all replacements are made)
                Int32 translated_crc = getCrc(translated_content, is_format_android_string_resources);
                if (origin_crc != translated_crc) {
                    String dest_content = remover_untranslated_strings == null
                        ? translated_content
                        : remover_untranslated_strings.apply(translated_content);
                    if (is_format_android_string_resources) {
                        dest_content = make_beautify_xml(dest_content);
                    }
                    Int32 dest_crc = getCrc(dest_content, is_format_android_string_resources);

                    String copyto_path = targetDir + '\\' + r.CopyTo.Replace("##LANG##", lang_code); //i.e. AndroidProject/res/values-ru/strings.xml
                    if (System.IO.File.Exists(copyto_path)) {
                        Int32 copyto_crc = getCrc(getTextFileContent(copyto_path, r.Replacements, is_format_android_string_resources), is_format_android_string_resources);
                        if (copyto_crc != dest_crc) { //AndroidProject/res/values-ru/strings.xml and CrowdinUnpackedArchive/ru/strings.xml are equal
                            System.IO.File.Delete(copyto_path);
                        }
                    }
                    if (!System.IO.File.Exists(copyto_path)) {
                        String copyto_dir = System.IO.Path.GetDirectoryName(copyto_path);
                        if (! System.IO.Directory.Exists(copyto_dir)) {
                            System.IO.Directory.CreateDirectory(copyto_dir); //i.e. create directory AndroidProject/res/values-ru/
                        }
                        //i.e. copy CrowdinUnpackedArchive/ru/strings.xml to AndroidProject/res/values-ru/strings.xml
                        save_utf8_file(dest_content, copyto_path);
                        //System.IO.File.Copy(translated_path, copyto_path);
                        log.Append(lang_code + ' ');
                    }
                }
            }
        }

//         public static XmlDocument make_beautify_xml(String sxml) {
//             XmlDocument xml = new XmlDocument();
//             xml.LoadXml(sxml);
//             var n = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//             return Xslt.applyTransformation(xml, "CrowdinSpreader.Resources.identity.xslt");
//         }
        public static String make_beautify_xml(String sxml) {
            XmlDocument doc = new XmlDocument();
            sxml = sxml.Replace("<!--Generated by crowdin.net-->", "");
            doc.LoadXml(sxml);

            Encoding utf8noBOM = new UTF8Encoding(false);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = utf8noBOM;
            using (MemoryStream output = new MemoryStream()) {
                using (XmlWriter writer = XmlWriter.Create(output, settings)) {
                    doc.Save(writer);
                }
                String dest = Encoding.UTF8.GetString(output.ToArray());
                XmlDocument test = new XmlDocument();
                test.LoadXml(dest);
                return dest;
            }
        }

        private static void save_utf8_file(string destContent, string destPath) {
            Utils.SaveStringToFile(destContent, destPath);
        }
        public static String getTextFileContent(String fileName, Dictionary<String, String> autoReplacements, bool androidStringResources) {

            String s = null;
            if (androidStringResources) {
                XmlDocument xml = new XmlDocument();
                xml.Load(fileName);
                s = xml.InnerXml;
            } else {
                s = Utils.GetFileBody(fileName, "UTF-8");
            }           
            
            if (autoReplacements != null) {
                foreach (var kvp in autoReplacements) {
                    s = s.Replace(kvp.Key, kvp.Value);
                }
            }
//             if (androidStringResources) {
//                 XmlDocument xml = new XmlDocument();
//                 xml.LoadXml(s);
//                 return xml.InnerXml;
//             }
            return s;
        }
        public static byte[] GetBinaryFileContent(String FileName) {
            using (System.IO.FileStream f = new System.IO.FileStream(FileName, System.IO.FileMode.Open)) {
                byte[] bytes = new byte[f.Length];
                Int32 readbytes = f.Read(bytes, 0, (Int32)f.Length);
                System.Diagnostics.Debug.Assert(f.Length == readbytes);
                return bytes;
            }
        }
        static String temp;
        private static Int32 getCrc(String fileContent, bool androidStringResources) {
            if (androidStringResources) {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(fileContent);
                xml.CreateXmlDeclaration("1.0", "utf-8", null);

                Dictionary<String, String> ss = new Dictionary<String, String>();
                foreach (XmlNode string_node in xml.SelectNodes("resources/string")) {
                    ss.Add(string_node.Attributes["name"].InnerText, string_node.InnerText.Trim());
                }
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in ss) {
                    sb.Append(kvp.Key + ":" + kvp.Value + "\n");
                }

                //!TODO: по непонятной причине путаются два разных двоеточия. 85 - из win1251 откуда-то лезет, хотя везде UTF-8
                //кроме того, иногда \r\n преобразуется в \n, иногда нет
                sb.Replace(char.ConvertFromUtf32(8230), "...");
                sb.Replace(char.ConvertFromUtf32(85), "...");
                sb.Replace("\r\n", "\n");

//                 String temp2 = sb.ToString();
//                 save_utf8_file(temp, "d:/t/!1.xml");
//                 save_utf8_file(temp2, "d:/t/!2.xml");
//                 if (temp == null) {
//                     temp = sb.ToString();
//                 }

                Int32 dest = CRC.GetCrc32(Encoding.UTF8.GetBytes(sb.ToString()));
                return dest;
            } else {
                return CRC.GetCrc32(Encoding.UTF8.GetBytes(fileContent));
            }
        }
    }
}
