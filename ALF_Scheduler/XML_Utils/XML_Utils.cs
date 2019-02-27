using System;
using System.IO;
using System.Xml.Linq;


namespace XML_Utils
{
    public static class XML_Utils
    {
        /// <summary>
        /// Should be run on program start. Creates neccessary files/folders for proper execution
        /// </summary>
        public static void Init()
        {
            //This must be run on program init
            Directory.CreateDirectory("./import");
            Directory.CreateDirectory("./export");
            Directory.CreateDirectory("./default");
            var defPath = "./default/Default_Codes.xml";
            if (!File.Exists(defPath)) CreateInitCodeXml(defPath);
            try
            {
                //Basically checks if there is a settings file in .\default
                GetConfigPathFromDir("./default", "settings");
            }
            catch (FileNotFoundException)
            {
                CreateGenericSettingsFile("./default");
            }
        }

        /// <summary>
        /// Returns the preferred import directory. Default is .\import, otherwise gets from settings file
        /// </summary>
        /// <returns>Returns the preferred import directory.</returns>
        public static string GetImportDir()
        {
            try
            {
                var settingsDoc = LoadSettingFile();
                if (settingsDoc != null)
                {
                    var importDir = settingsDoc.Element("customImportDir")?.ToString();
                    if (importDir != null) return importDir;
                }
            }
            catch (FileNotFoundException)
            {
                //No custom import directory exists
            }

            return "./import";
        }

        /// <summary>
        /// Returns the preferred export directory. Default is .\export, otherwise gets from settings file
        /// </summary>
        /// <returns>Returns the preferred export directory.</returns>
        public static string GetExportDir()
        {
            var settingsDoc = LoadSettingFile();
            if (settingsDoc != null)
            {
                var importDir = settingsDoc.Element("customImportDir")?.ToString();
                if (importDir != null) return importDir;
            }

            return "./export";
        }

        /// <summary>
        /// Checks if settings file exists in default directory. If we choose to implement settings file, there will need to be a settings file here by default to at least point to custom import directories, to load imported settings.
        /// </summary>
        /// <returns>Will return the default settings path if it exists.</returns>
        /// <exception cref="FileNotFoundException">Throws exception if no default settings file exists.</exception>
        public static string DefaultSettingsFileExist()
        {
            try
            {
                return GetConfigPathFromDir("./default", "settings");
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a settings file from where ever one is located, either default directory, or from custom import directory.
        /// </summary>
        /// <returns>Returns the settings file as an XDocument</returns>
        /// <exception cref="FileNotFoundException">Throws exception if no root settings file in the default directory.</exception>
        public static XDocument LoadSettingFile()
        {
            var settingsPath = DefaultSettingsFileExist();
            if (settingsPath != null)
            {
                var defSettings = XDocument.Load(settingsPath);
                var customImportDir = defSettings.Element("customImportDir")?.ToString();
                if (Directory.Exists(customImportDir))
                {
                    try
                    {
                        var newPath = GetConfigPathFromDir(customImportDir, "settings");
                        return XDocument.Load(newPath);
                    }
                    catch (FileNotFoundException)
                    {
                        if (defSettings != null) return defSettings;
                    }
                }
                else
                {
                    if (defSettings != null) return defSettings;
                }
            }

            
            throw new FileNotFoundException("No root settings file in default directory");
        }

        /// <summary>
        /// Gets path of most recent config file given a directory to search and a type of config xml
        /// </summary>
        /// <param name="directory">Folder path to look for conifiguration file in.</param>
        /// <param name="type">Type of configuration file to look for.</param>
        /// <returns>Returns the path of specified configuration file.</returns>
        /// <exception cref="FileNotFoundException">Throws exception if it could not find type of configuration file specified.</exception>
        public static string GetConfigPathFromDir(string directory, string type)
        {
            var files = Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);
            string returnPath = null;
            foreach (var filePath in files)
            {
                var doc = XDocument.Load(filePath);
                var docType = doc.Element("configuration")?.Attribute("type")?.ToString();
                docType = docType.Replace("\"", "");
                docType = docType.Replace("\\", "");
                docType = docType.Remove(0, 5);
                if (docType != null && docType.Equals(type)) returnPath = MostRecentFile(returnPath, filePath);
            }

            if (returnPath != null) return returnPath;
            throw new FileNotFoundException("Could not find " + type + " file in directory specified");
        }

        /// <summary>
        /// Will load codes file from default path or if settings file contains a customImportDir, go to that dir and check if there is codes xml
        /// </summary>
        /// <returns>Returns the code file from either custom or default directory. Prefers custom directory.</returns>
        public static XDocument LoadCodeFile()
        {
            var importDir = GetImportDir();
            string codePath = null;

            try
            {
                codePath = GetConfigPathFromDir(importDir, "codes");
            }
            catch (FileNotFoundException)
            {
                //ignore, means it couldn't find codes file in .\import nor customImportDir
                codePath = null;
            }

            if (codePath == null)
                try
                {
                    codePath = GetConfigPathFromDir("./default", "codes");
                }
                catch (FileNotFoundException)
                {
                    //TODO Should we handle this somehow? I like create code file then recurse back through this method?
                    throw new NotImplementedException("Couldn't load codes file from any directory");
                }

            return XDocument.Load(codePath);
        }

        /// <summary>
        /// Returns the path of the string whose file was last written to.
        /// </summary>
        /// <param name="current">Path containing the current most recent file.</param>
        /// <param name="toTest">Path to file to see how it compares to current most recent file.</param>
        /// <returns>Returns the path of the file that was last written to.</returns>
        private static string MostRecentFile(string current, string toTest)
        {
            if (current == null) return toTest;

            var curTime = File.GetLastWriteTime(current);
            var testTime = File.GetLastWriteTime(toTest);
            if (DateTime.Compare(curTime, testTime) > 0) return current;

            return toTest;
        }

        /// <summary>
        /// Generates a settings file with no custom data in it
        /// </summary>
        /// <param name="path">Folder path to create an empty settings file.</param>
        public static void CreateGenericSettingsFile(string path)
        {
            var doc = new XDocument(
                new XElement("configuration", new XAttribute("type", "settings"),
                    new XElement("customImportDir", ""),
                    new XElement("customExportDir", "")
                )
            );

            doc.Save(path + "/Default_Settings.xml");
        }

        /// <summary>
        /// Writes current application settings to folder, either given or default path. Can be given folder or .xml path.
        /// </summary>
        /// <param name="path">Path to export current application settings to. If given just a folder, will create a timestamped name.</param>
        public static void ExportCurrentSettings(string path)
        {
            var fileName = "Settings_Export_" + DateTime.Now + ".xml";

            //TODO write application settings
            var doc = new XDocument(
                new XElement("configuration", new XAttribute("type", "settings"),
                    new XElement("customImportDir", ""),
                    new XElement("customExportDir", "")
                )
            );

            if (path != null)
                try
                {
                    var attr = File.GetAttributes(path);

                    //If path is just a dir
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        if (path.EndsWith("/") || path.EndsWith("\\"))
                            doc.Save(path + fileName);
                        else
                            doc.Save(path + "/" + fileName);
                    }

                    //Check if is .xml path then save
                    var ext = Path.GetExtension(path);
                    if (ext.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                        doc.Save(path);
                    else
                        doc.Save("./export/" + fileName);
                }
                catch (Exception)
                {
                    doc.Save("./export/" + fileName);
                }
            else
                doc.Save("./export/" + fileName);
        }

        /// <summary>
        /// Creates initial code definitons xml file to specified path
        /// </summary>
        /// <param name="path">Path to create default code file at. Needs to include a .xml file name.</param>
        public static void CreateInitCodeXml(string path)
        {
            var doc = new XDocument(
                new XElement("configuration", new XAttribute("type", "codes"),
                    new XElement("codes",
                        new XElement("code",
                            new XElement("name", "NO"),
                            new XElement("desc",
                                "Current licensed ALF with no deficiencies or consultation-only on current visit."),
                            new XElement("minMonth", "16"),
                            new XElement("maxMonth", "18")),
                        new XElement("code",
                            new XElement("name", "NO24"),
                            new XElement("desc",
                                "Current licensed ALF that has had three consecutive inspections with no written notice of violations and has received no written notice of violations resulting from complaint investigation during that same time period."),
                            new XElement("minMonth", "16"),
                            new XElement("maxMonth", "24")),
                        new XElement("code",
                            new XElement("name", "ENF"),
                            new XElement("desc",
                                "Current licensed ALF with enforcement (fines, stop placement, conditions, revocation, summary suspension) in the past year for either full licensing inspection or complaint investigation."),
                            new XElement("minMonth", "9"),
                            new XElement("maxMonth", "12")),
                        new XElement("code",
                            new XElement("name", "YES"),
                            new XElement("desc",
                                "Current licensed ALF with deficiencies on current visit that did not result in enforcement."),
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