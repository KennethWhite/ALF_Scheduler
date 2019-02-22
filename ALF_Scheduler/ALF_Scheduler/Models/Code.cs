﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ALF_Scheduler.Models
{
    public class Code : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinMonth { get; set; }
        public int MaxMonth { get; set; }

        public static List<Code> CodesList { get; private set; }

        public static List<Code> getCodes()
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

            var outList = new List<Code>();

            foreach (var code in codes)
            {
                var newCode = new Code();

                newCode.Name = code.name;
                newCode.Description = code.desc;
                newCode.MinMonth = int.Parse(code.minMonth);
                newCode.MaxMonth = int.Parse(code.maxMonth);

                outList.Add(newCode);
            }

            CodesList = outList;
            return outList;
        }

        public static Code getCodeByName(string name)
        {
            var codes =
                (from code in CodesList
                    where code.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select code).ToList();

            if (codes.Count > 0) return codes.ElementAt(0);
            throw new KeyNotFoundException("Could not find a code with that name");
        }
    }
}