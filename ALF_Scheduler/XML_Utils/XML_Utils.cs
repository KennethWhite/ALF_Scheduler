using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XML_Utils
{
    internal class Program
    {


        private static void readXML(String path)
        {
            //This was just for testing mostly, but once we get a file tree set up, we can have it point to our program settings XML
            XDocument doc = XDocument.Load(path);

            var codes = from c in doc.Descendants("code")
                        select new
                        {
                            name = c.Descendants("name").First().Value,
                            desc = c.Descendants("desc").First().Value,
                            minMonth = c.Descendants("minMonth").First().Value,
                            maxMonth = c.Descendants("maxMonth").First().Value
                        };

            foreach (var code in codes)
            {
                Console.WriteLine("Code {0} has range ({1} - {2}) and description: {3}", code.name, code.minMonth,
                    code.maxMonth, code.desc);
            }

            string configImport = doc.Descendants("configImportDir").First().Value;
            string defaultExport = doc.Descendants("defaultExportDir").First().Value;

            Console.WriteLine("in: {0}, out {1}", configImport, defaultExport);
        }

        private static void createInitXML()
        {
            using (XmlWriter writer = XmlWriter.Create("init.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");
                writer.WriteStartElement("codes");
                writer.WriteStartElement("code");
                writer.WriteElementString("name", "NO");
                writer.WriteElementString("desc", "Current licensed ALF with no deficiencies or consultation-only on current visit.");
                writer.WriteElementString("minMonth", "16");
                writer.WriteElementString("maxMonth", "18");
                writer.WriteEndElement();
                writer.WriteStartElement("code");
                writer.WriteElementString("name", "NO24");
                writer.WriteElementString("desc", "Current licensed ALF that has had three consecutive inspections with no written notice of violations and has received no written notice of violations resulting from complaint investigation during that same time period");
                writer.WriteElementString("minMonth", "16");
                writer.WriteElementString("maxMonth", "24");
                writer.WriteEndElement();
                writer.WriteStartElement("code");
                writer.WriteElementString("name", "ENF");
                writer.WriteElementString("desc", "Current licensed ALF with enforcement (fines, stop placement, conditions, revocation, summary suspension) in the past year for either full licensing inspection or complaint investigation.");
                writer.WriteElementString("minMonth", "9");
                writer.WriteElementString("maxMonth", "12");
                writer.WriteEndElement();
                writer.WriteStartElement("code");
                writer.WriteElementString("name", "YES");
                writer.WriteElementString("desc", "Current licensed ALF with deficiencies on current visit that did not result in enforcement.");
                writer.WriteElementString("minMonth", "13");
                writer.WriteElementString("maxMonth", "15");
                writer.WriteEndElement();
                writer.WriteStartElement("code");
                writer.WriteElementString("name", "CHOWN");
                writer.WriteElementString("desc", "Change of ownership (CHOW) with a new licensee.");
                writer.WriteElementString("minMonth", "6");
                writer.WriteElementString("maxMonth", "9");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteElementString("configImportDir", "");
                writer.WriteElementString("defaultExportDir", "");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

    }
}
