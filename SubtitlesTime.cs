using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Subrip
{
    public class SubtitlesTime
    {
        private static Match matchParts;
        private static Regex timeParts = new Regex(@"(\d+):(\d+):(\d+),(\d+)");
        private static int t1, t2, t3, t4;
        private static TimeSpan ts;

        public static void start(string filePath, string time, string options)
        {
            Regex timeGroup = new Regex(@"^([\d:,]+) --> ([\d:,]+)$");
            Match matchGroup;
            string ts1, ts2;
            TimeSpan offset = convertToTimeSpan(time);

            List<string> lines = new List<string>();
            List<string> lines2 = new List<string>();
            lines = File.ReadAllLines(filePath).ToList();

            foreach (String line in lines)
            {
                matchGroup = timeGroup.Match(line);

                if (matchGroup.Success)
                {
                    ts1 = includeOffset(matchGroup.Groups[1], offset, options);
                    ts2 = includeOffset(matchGroup.Groups[2], offset, options);

                    lines2.Add($"{ts1} --> {ts2}");
                }
                else
                {
                    lines2.Add(line);
                }
            }

            File.WriteAllLines(filePath, lines2);
        }

        private static string includeOffset(Group group, TimeSpan offset, string options)
        {
            TimeSpan ts = convertToTimeSpan(group.ToString());

            if(options == "Add")
            {
                ts = ts.Add(offset);
            }
            else
            {
                ts = ts.Subtract(offset);
            }

            return ts.ToString("hh\\:mm\\:ss\\,fff");
        }

        private static TimeSpan convertToTimeSpan(string offset)
        {
            matchParts = timeParts.Match(offset);

            t1 = int.Parse(matchParts.Groups[1].ToString());
            t2 = int.Parse(matchParts.Groups[2].ToString());
            t3 = int.Parse(matchParts.Groups[3].ToString());
            t4 = int.Parse(matchParts.Groups[4].ToString());
            ts = new TimeSpan(0, t1, t2, t3, t4);

            return ts;
        }
    }
}
