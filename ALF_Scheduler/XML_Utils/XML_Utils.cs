using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace XML_Utils
{
    public static class XML_Utils
    {
        //Just here to show how to use Linq with XDocument
        public static void ReadCodeXml(XDocument doc)
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

        //Returns the preferred import directory. Default is .\import, otherwise gets from settings file
        public static string GetImportDir()
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

        //Returns the preferred export directory. Default is .\export, otherwise gets from settings file
        public static string GetExportDir()
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

        //Checks if settings file exists in default directory. If we choose to implement settings file, there will need to be a settings file here by default to at least point to custom import directories, to load imported settings.
        public static Boolean SettingsFileExist()
        {
            string[] files = Directory.GetFiles(@".\default", "*.xml", SearchOption.TopDirectoryOnly);
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

        //Gets a settings file from where ever one is located, either default directory, or from custom import dir
        public static XDocument LoadSettingFile()
        {
            if (SettingsFileExist())
            {
                string settingsPath = getConfigPathFromDir(@".\default", "settings");

                if (settingsPath != null)
                {
                    XDocument defSettings = XDocument.Load(settingsPath);
                    string customImportDir = defSettings.Element("customImportDir")?.ToString();
                    if (customImportDir != null && Directory.Exists(customImportDir))
                    {
                        try
                        {
                            string newPath = getConfigPathFromDir(customImportDir, "settings");
                            return XDocument.Load(newPath);
                        }
                        catch (FileNotFoundException e)
                        {
                            return defSettings;
                            //TODO Log it?
                        }                        
                    }
                }
            }
            throw new FileNotFoundException("Could not find the settings file in 'default' directory");
        }

        //Gets path of most recent config file given a directory to search and a type of config xml
        public static string getConfigPathFromDir(string directory, string type)
        {
            string[] files = Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);
            string returnPath = null;
            foreach (string filePath in files)
            {
                XDocument doc = XDocument.Load(filePath);
                string docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                if (docType != null && docType.Equals(type))
                {
                    returnPath = MostRecentFile(returnPath, filePath);
                }
            }
            if (returnPath != null)
            {
                return returnPath;
            }
            throw new FileNotFoundException("Could not find settings file in directory specified");
        }

        //Will load codes file from default path or if settings file contains a customImportDir, go to that dir and check if there is codes xml
        public static XDocument LoadCodeFile()
        {
            string importDir = GetImportDir();
            string codePath = null;

            try
            {
                codePath = getConfigPathFromDir(importDir, "codes");
            }
            catch (FileNotFoundException)
            {
                //ignore, means it couldn't find codes file in .\import nor customImportDir
                codePath = null;
            }

            if (codePath == null)
            {
                try
                {
                    codePath = getConfigPathFromDir(@".\default", "codes");
                }
                catch (FileNotFoundException)
                {
                    //TODO Should we handle this somehow? I like create code file then recurse back through this method?
                    throw new NotImplementedException("Couldn't load codes file from any directory");
                }
            }

            return XDocument.Load(codePath);
        }

        //Returns the path of the string whose file was last written to.
        private static string MostRecentFile(string current, string toTest)
        {
            if (current == null)
            {
                return toTest;
            }

            DateTime curTime = File.GetLastWriteTime(current);
            DateTime testTime = File.GetLastWriteTime(toTest);
            if (DateTime.Compare(curTime, testTime) > 0)
            {
                return current;
            }

            return toTest;
        }

        //Should be run on program start. Creates neccessary files/folders for proper execution
        public static void CreateInitFolders()
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

        //Writes current application settings to folder, either given or default path. Can be given folder or .xml path.
        public static void ExportCurrentSettings(string path)
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

        //Creates initial code definitons xml file to specified path
        public static void CreateInitCodeXml(string path)
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
