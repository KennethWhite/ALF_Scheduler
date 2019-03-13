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

        public static ObservableCollection<Code> CodesList { get; private set; }

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
    }
}