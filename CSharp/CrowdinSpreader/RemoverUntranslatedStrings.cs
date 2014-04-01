using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CrowdinSpreader {
    /// <summary>
    /// Remove all untranslated android strings from XML file. I.e.:
    /// Origin: &lt;string name="label_save"&gt;Save&lt;/string&gt;
    /// Translated (the same): &lt;string name="label_save"&gt;Save&lt;/string&gt;
    /// such string should be removed from translated file
    /// </summary>
    public class RemoverUntranslatedStrings {
        private readonly XmlDocument _Origin = new XmlDocument();
        public RemoverUntranslatedStrings(String originalXmlContent) {
            _Origin.LoadXml(originalXmlContent);
        }

        public String apply(String translatedXmlContent) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(translatedXmlContent);

            foreach (XmlNode ns0 in _Origin.SelectNodes("resources/string")) {
                String name = ns0.Attributes["name"].InnerText;
                String svalue0 = ns0.InnerText.Replace("\r\n", "\n");

                XmlNode ns1 = xml.SelectSingleNode(String.Format("resources/string[@name='{0}']", name));
                if (ns1 != null) {
                    String svalue1 = ns1.InnerText.Replace("\r\n", "\n");
                    if (svalue0 == svalue1) {
                        ns1.ParentNode.RemoveChild(ns1);
                    }
                }
            }
            return xml.InnerXml;
        }
    }
}
