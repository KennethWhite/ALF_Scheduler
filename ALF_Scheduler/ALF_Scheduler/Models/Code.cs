using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;

namespace ALF_Scheduler.Models
{
    public class Code : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinMonth { get; set; }
        public int MaxMonth { get; set; }

        public static ObservableCollection<Code> CodesList { get; set; }

        /// <summary>
        /// Returns a list of Codes objects. Codes are coming from the code configuration xml file.
        /// </summary>
        /// <returns>Returns list of code objects</returns>
        public static ObservableCollection<Code> GetCodes()
        {
            var doc = XML_Utils.XML_Utils.LoadCodeFile();

            var codes = from c in doc.Descendants("code")
                select new
                {
                    name = c.Descendants("name").First().Value,
                    desc = c.Descendants("desc").First().Value,
                    minMonth = c.Descendants("minMonth").First().Value,
                    maxMonth = c.Descendants("maxMonth").First().Value
                };

            var size = codes.ToArray().Length;

            var outList = new ObservableCollection<Code>();

            foreach (var code in codes)
            {
                var newCode = new Code {
                    Name = code.name,
                    Description = code.desc,
                    MinMonth = int.Parse(code.minMonth),
                    MaxMonth = int.Parse(code.maxMonth)
                };

                outList.Add(newCode);
            }

            CodesList = outList;
            return outList;
        }

        /// <summary>
        /// Will return code object by given name. Checks member variable CodesList if there are any if not then it will get available codes. If run statically, will always need to grab from file.
        /// </summary>
        /// <param name="name">String containing the code's name</param>
        /// <returns>Returns a code object matching code name, or null if no code was found.</returns>
        public static Code GetCodeByName(string name)
        {
            if (CodesList == null || !CodesList.Any())
            {
                GetCodes();
            }
            var codes =
                (from code in CodesList
                    where code.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select code).ToList();

            if (codes.Count > 0) return codes.ElementAt(0);

            return null;
            //TODO do we want to return null or exception?
            //throw new KeyNotFoundException("Could not find a code with that name");
        }

        /// <summary>
        /// Adds given code to code file. Creates new file.
        /// </summary>
        /// <param name="code">Code to add.</param>
        /// <param name="fileName">Filename to name new file.</param>
        public static void AddCodeToFile(Code code, string fileName)
        {
            Regex regex = new Regex(@"^.*\.(?i)xml(?-i)$"); //Match .xml

            if (regex.IsMatch(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
            {
                string dirToSave = XML_Utils.XML_Utils.GetImportDir();

                XDocument doc = XML_Utils.XML_Utils.LoadCodeFile();

                XElement root = new XElement("code");
                root.Add(new XElement("name", code.Name));
                root.Add(new XElement("desc", code.Description));
                root.Add(new XElement("minMonth", code.MinMonth));
                root.Add(new XElement("maxMonth", code.MaxMonth));

                doc.Element("configuration").Element("codes").Add(root);

                doc.Save(dirToSave + "/" + fileName);
                return;
            }

            throw new FileFormatException("Given filename is not a valid xml filename.");
        }

        /// <summary>
        /// Removes given code from document. And saves new code file.
        /// </summary>
        /// <param name="code">Code to remove.</param>
        /// <param name="fileName">Optional filename</param>
        public static void RemoveCode(Code code, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "Codes_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HHmmss") + ".xml";
            }
            else if (!fileName.EndsWith(".xml", System.StringComparison.InvariantCultureIgnoreCase))
            {
                fileName = fileName + ".xml";
            }

            XDocument doc = XML_Utils.XML_Utils.LoadCodeFile();
            doc.Element("configuration")
                .Element("codes")
                .Descendants("code")
                .Where(e => e.Element("name").Value.Equals(code.Name))
                .Remove();

            doc.Save(XML_Utils.XML_Utils.GetImportDir() + "/" + fileName);
        }

        /// <summary>
        /// Will update oldCode to newCode's information. Save info to new file.
        /// </summary>
        /// <param name="oldCode">Code to update.</param>
        /// <param name="newCode">Updated Code.</param>
        /// <param name="fileName">Optional file name.</param>
        public static void UpdateCode(Code oldCode, Code newCode, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "Codes_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HHmmss") + ".xml";
            }
            else if (!fileName.EndsWith(".xml", System.StringComparison.InvariantCultureIgnoreCase))
            {
                fileName = fileName + ".xml";
            }

            XDocument doc = XML_Utils.XML_Utils.LoadCodeFile();

            XElement code = doc
                .Element("configuration")
                .Element("codes")
                .Elements("code")
                .Where(e => e.Element("name").Value.Equals(oldCode.Name))
                .Single();

            code.Element("name").Value = newCode.Name;
            code.Element("desc").Value = newCode.Description;
            code.Element("minMonth").Value = newCode.MinMonth.ToString();
            code.Element("maxMonth").Value = newCode.MaxMonth.ToString();

            doc.Save(XML_Utils.XML_Utils.GetImportDir() + "/" + fileName);
        }
    }
}