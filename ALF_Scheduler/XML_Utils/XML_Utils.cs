using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace ALF_Mine
{
    internal class Program
    {
        public static void Main()
        {
            createInitCodeXML(@".\import\codes.xml");
            loadFiles();
        }

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

        //Loads the config and code files in an ara, ara[0] = xmlCode (never null), ara[1] = xmlSettings (can be null).
        private static XmlDocument[] loadFiles()
        {
            XmlDocument xmlCodes = null;
            XmlDocument xmlSettings = null;
            XmlDocument[] outAra = new XmlDocument[2];

            if (directoryEmpty(@".\import"))
            {
                if (File.Exists(@".\default\Default_Codes.xml"))
                {
                    xmlCodes = new XmlDocument();
                    xmlCodes.Load(@".\default\Default_Codes.xml");
                }
                else
                {
                    createInitCodeXML(@".\default\Default_Codes.xml");
                    xmlCodes = new XmlDocument();
                    xmlCodes.Load(@".\default\Default_Codes.xml");
                }

                outAra[0] = xmlCodes;
            }
            else
            {
                string[] files = Directory.GetFiles(@".\import", "*.xml", SearchOption.TopDirectoryOnly);
                string codesPath = null;
                string settingsPath = null;

                foreach (string filePath in files)
                {
                    XmlDocument temp = new XmlDocument();
                    temp.Load(filePath);
                    XmlNodeList nodeList = temp.GetElementsByTagName("configuration");
                    string type = null;
                    try
                    {
                        XmlNode node = nodeList.Item(0);
                        type = node.Attributes["type"].Value;
                    }
                    catch (Exception e)
                    {

                    }

                    switch (type)
                    {
                        case "codes":
                            codesPath = mostRecentFile(codesPath, filePath);
                            break;
                        case "settings":
                            settingsPath = mostRecentFile(settingsPath, filePath);
                            break;
                    }
                }

                if (codesPath != null)
                {
                    xmlCodes = new XmlDocument();
                    xmlCodes.Load(codesPath);
                }
                else
                {
                    if (File.Exists(@".\default\Default_Codes.xml"))
                    {
                        xmlCodes = new XmlDocument();
                        xmlCodes.Load(@".\default\Default_Codes.xml");
                    }
                    else
                    {
                        createInitCodeXML(@".\default\Default_Codes.xml");
                        xmlCodes = new XmlDocument();
                        xmlCodes.Load(@".\default\Default_Codes.xml");
                    }
                }

                if (settingsPath != null)
                {
                    xmlSettings = new XmlDocument();
                    xmlSettings.Load(settingsPath);
                }

                outAra[0] = xmlCodes;

                if (xmlSettings != null)
                {
                    outAra[1] = xmlSettings;
                }
            }

            return outAra;
        }



        private static string mostRecentFile(string current, string toTest)
        {
            if (current == null)
            {
                return toTest;
            }
            else
            {
                DateTime curTime = File.GetCreationTime(current);
                DateTime testTime = File.GetCreationTime(toTest);
                if (DateTime.Compare(curTime, testTime) > 0)
                {
                    return current;
                }
                else
                {
                    return toTest;
                }
            }
        }

        private static void createInitFolders()
        {
            //This must be run on program init
            DirectoryInfo import = Directory.CreateDirectory(@".\import");
            DirectoryInfo export = Directory.CreateDirectory(@".\export");
            DirectoryInfo defaultFolder = Directory.CreateDirectory(@".\default");
            String defPath = @".\default\Default_Codes.xml";
            if (!File.Exists(defPath))
            {
                createInitCodeXML(defPath);
            }
        }

        private static bool directoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        //Writes current application settings to folder, either given or default path
        private static void exportCurrentConfiguration(string path)
        {
            String fileName = "Config_Export_" + DateTime.Now;

            XmlWriter writer = null;

            if (path != null)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(path);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        writer = XmlWriter.Create(path);
                    }

                }
                catch (Exception e)
                {
                    writer = XmlWriter.Create(@".\export\" + fileName);
                }
            }
            else
            {
                writer = XmlWriter.Create(@".\export\" + fileName);
            }

            if (writer != null)
            {

                writer.WriteStartDocument();

                //Write current application settings here

                writer.WriteEndDocument();
                writer.Flush();
            }

        }

        //Creates initial code definitons to path
        private static void createInitCodeXML(string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("configuration");
                writer.WriteAttributeString("type", "codes");
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
