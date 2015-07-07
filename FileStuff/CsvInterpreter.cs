using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.Text;

namespace FileStuff
{
    public class CsvInterpreter
    {
        public static string[] CSVRowToStringArray(string line, char fieldSeparator = ',', char stringSep = '\"')
        {
            bool bolQuote = false;
            var bld = new StringBuilder();
            List<string> retAry = new List<string>();

            foreach (char c in line.ToCharArray())
                if ((c == fieldSeparator && !bolQuote))
                {
                    retAry.Add(bld.ToString());
                    bld.Clear();
                }
                else
                    if (c == stringSep)
                        bolQuote = !bolQuote;
                    else
                        bld.Append(c);

            return retAry.ToArray();
        }

        public IEnumerable<string[]> Deserialize(Stream textstream)
        {
            using (var input = new StreamReader(textstream))
            {
                while (!input.EndOfStream)
                {
                    var line = input.ReadLine();
                    var items = CSVRowToStringArray(line, ';');
                    yield return items;
                }
            }
        }
    }
}
