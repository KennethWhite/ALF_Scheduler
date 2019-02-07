using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XML_Utils;

namespace ALF_Scheduler.Models
{
    public class Code : Entity
    {
        public string name { get; set; }
        public string description { get; set; }
        public int minMonth { get; set; }
        public int maxMonth { get; set; }

        public static List<Code> getCodes()
        {
            XDocument doc = XML_Utils.XML_Utils.LoadCodeFile();

            var codes = from c in doc.Descendants("code")
                        select new
                        {
                            name = c.Descendants("name").First().Value,
                            desc = c.Descendants("desc").First().Value,
                            minMonth = c.Descendants("minMonth").First().Value,
                            maxMonth = c.Descendants("maxMonth").First().Value
                        };

            int size = codes.ToArray().Length;

            List<Code> outList = new List<Code>();

            foreach (var code in codes)
            {
                Code newCode = new Code();

                newCode.name = code.name;
                newCode.description = code.desc;
                newCode.minMonth = int.Parse(code.minMonth);
                newCode.maxMonth = int.Parse(code.maxMonth);

                outList.Add(newCode);
            }

            return outList;
        }

        public static Code getCodeByName(string name, List<Code> codeList)
        {
            IEnumerable<Code> codes =
                from code in codeList
                where code.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                select code;

            if (codes.Count() > 0)
            {
                return codes.ElementAt(0);
            }
            throw new KeyNotFoundException("Could not find a code with that name");
        }
    }
    }
