using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace ALF_Mine
{
    internal class Program
    {
        private static void ReadCodeXml(String path)
        {
            XmlDocument temp = new XmlDocument();
            temp.Load(path);
            XDocument doc = XDocument.Parse(temp.OuterXml);
            ReadCodeXml(doc);
        }

        //Just here to show how to use linq with XDocument
        private static void ReadCodeXml(XDocument doc)
        {
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
        }

        private static string GetImportDir()
        {
            XDocument settingsDoc = LoadSettingFile();
            if (settingsDoc != null)
            {
                string importDir = settingsDoc.Element("customImportDir")?.ToString();
                if (importDir != null)
                {
                    return importDir;
                }
            }

            return @".\import";

        }

        private static string GetExportDir()
        {
            XDocument settingsDoc = LoadSettingFile();
            if (settingsDoc != null)
            {
                string importDir = settingsDoc.Element("customImportDir")?.ToString();
                if (importDir != null)
                {
                    return importDir;
                }
            }

            return @".\export";
        }

        private static Boolean SettingsFileExist()
        {
            string[] files = Directory.GetFiles(@".\import", "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string filePath in files)
            {
                XDocument doc = XDocument.Load(filePath);
                string docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                if (docType != null && docType.Equals("settings"))
                {
                    return true;
                }
            }

            return false;
        }

        //TODO as of right now, it just loads settings file from import dir since we don't have any default settings
        private static XDocument LoadSettingFile()
        {
            if (SettingsFileExist())
            {
                string settingsPath = null;
                string[] files = Directory.GetFiles(@".\import", "*.xml", SearchOption.TopDirectoryOnly);
                foreach (string filePath in files)
                {
                    XDocument doc = XDocument.Load(filePath);
                    string docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                    if (docType != null && docType.Equals("settings"))
                    {
                        settingsPath = filePath;
                    }
                }

                if (settingsPath != null)
                {
                    return XDocument.Load(settingsPath);
                }
            }
            return null;
        }

        //Will load codes file from default path or if settings file contains a customImportDir, go to that dir and check if there is codes xml
        private static XDocument LoadCodeFile()
        {
            string codesPath = @".\default\Default_Codes.xml";
            string importDir = GetImportDir();
            if (importDir.Equals(@"./import"))
            {
                //ignore
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (importDir != null)
            {
                FileAttributes attr = File.GetAttributes(codesPath);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    string[] files = Directory.GetFiles(importDir, "*.xml", SearchOption.TopDirectoryOnly);
                    foreach (string filePath in files)
                    {
                        XDocument doc = XDocument.Load(filePath);
                        string docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                        if (docType != null && docType.Equals("codes"))
                        {
                            if (codesPath.Equals(@".\default\Default_Codes.xml"))
                            {
                                codesPath = filePath;
                            }
                            else
                            {
                                codesPath = MostRecentFile(codesPath, filePath);
                            }
                        }
                    }
                }
            }

            return XDocument.Load(codesPath);
        }

        //Loads the config and code files in an ara, ara[0] = xmlCode (never null), ara[1] = xmlSettings (can be null).
        private static XDocument[] LoadFiles()
        {
            XDocument xmlCodes;
            XDocument xmlSettings = null;
            XDocument[] outAra = new XDocument[2];

            if (DirectoryEmpty(@".\import"))
            {
                if (File.Exists(@".\default\Default_Codes.xml"))
                {
                    xmlCodes = XDocument.Load(@".\default\Default_Codes.xml");
                }
                else
                {
                    CreateInitCodeXml(@".\default\Default_Codes.xml");
                    xmlCodes = XDocument.Load(@".\default\Default_Codes.xml");
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
                    XDocument doc = XDocument.Load(filePath);
                    string docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                    switch (docType)
                    {
                        case "codes":
                            codesPath = MostRecentFile(codesPath, filePath);
                            break;
                        case "settings":
                            settingsPath = MostRecentFile(settingsPath, filePath);
                            break;
                    }
                }

                if (codesPath != null)
                {
                    xmlCodes = XDocument.Load(codesPath);
                }
                else
                {
                    if (File.Exists(@".\default\Default_Codes.xml"))
                    {
                        xmlCodes = XDocument.Load(@".\default\Default_Codes.xml");
                    }
                    else
                    {
                        CreateInitCodeXml(@".\default\Default_Codes.xml");
                        xmlCodes = XDocument.Load(@".\default\Default_Codes.xml");
                    }
                }

                if (settingsPath != null)
                {
                    xmlSettings = XDocument.Load(settingsPath);
                }

                outAra[0] = xmlCodes;

                if (xmlSettings != null)
                {
                    outAra[1] = xmlSettings;
                }
            }

            return outAra;
        }



        private static string MostRecentFile(string current, string toTest)
        {
            if (current == null)
            {
                return toTest;
            }

            DateTime curTime = File.GetCreationTime(current);
            DateTime testTime = File.GetCreationTime(toTest);
            if (DateTime.Compare(curTime, testTime) > 0)
            {
                return current;
            }

            return toTest;
        }

        private static void CreateInitFolders()
        {
            //This must be run on program init
            Directory.CreateDirectory(@".\import");
            Directory.CreateDirectory(@".\export");
            Directory.CreateDirectory(@".\default");
            String defPath = @".\default\Default_Codes.xml";
            if (!File.Exists(defPath))
            {
                CreateInitCodeXml(defPath);
            }
        }

        private static bool DirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        //Writes current application settings to folder, either given or default path
        private static void ExportCurrentSettings(string path)
        {
            String fileName = "Settings_Export_" + DateTime.Now + ".xml";

            //TODO write application settings
            XDocument doc = new XDocument(
                new XElement("configuration", new XAttribute("type", "settings"),
                    new XElement("customImportDir", ""),
                    new XElement("customExportDir", "")
                )
            );

            if (path != null)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(path);

                    //If path is just a dir
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        if (path.EndsWith(@"\"))
                        {
                            doc.Save(path + fileName);
                        }
                        else
                        {
                            doc.Save(path + @"\" + fileName);
                        }
                    }

                    //Check if is .xml path then save
                    String ext = Path.GetExtension(path);
                    if (ext.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        doc.Save(path);
                    }
                    else
                    {
                        doc.Save(@".\export\" + fileName);
                    }

                }
                catch (Exception)
                {
                    doc.Save(@".\export\" + fileName);
                }
            }
            else
            {
                doc.Save(@".\export\" + fileName);
            }

        }

        //Creates initial code definitons to path
        private static void CreateInitCodeXml(string path)
        {
            XDocument doc = new XDocument(
                new XElement("configuration", new XAttribute("type", "codes"),
                    new XElement("codes",
                        new XElement("code",
                            new XElement("name", "NO"),
                            new XElement("desc", "Current licensed ALF with no deficiencies or consultation-only on current visit."),
                            new XElement("minMonth", "16"),
                            new XElement("maxMonth", "18")),
                        new XElement("code",
                            new XElement("name", "NO24"),
                            new XElement("desc", "Current licensed ALF that has had three consecutive inspections with no written notice of violations and has received no written notice of violations resulting from complaint investigation during that same time period."),
                            new XElement("minMonth", "16"),
                            new XElement("maxMonth", "24")),
                        new XElement("code",
                            new XElement("name", "ENF"),
                            new XElement("desc", "Current licensed ALF with enforcement (fines, stop placement, conditions, revocation, summary suspension) in the past year for either full licensing inspection or complaint investigation."),
                            new XElement("minMonth", "9"),
                            new XElement("maxMonth", "12")),
                        new XElement("code",
                            new XElement("name", "YES"),
                            new XElement("desc", "Current licensed ALF with deficiencies on current visit that did not result in enforcement."),
                            new XElement("minMonth", "13"),
                            new XElement("maxMonth", "15")),
                        new XElement("code",
                            new XElement("name", "CHOWN"),
                            new XElement("desc", "Change of ownership (CHOW) with a new licensee."),
                            new XElement("minMonth", "6"),
                            new XElement("maxMonth", "9")
                        )
                    )
                )
            );
            doc.Save(path);
        }
    }
}
