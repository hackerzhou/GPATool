using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using GPATool.Bean;

namespace GPATool.Util
{
    public class ParseHTML
    {
        public static List<Lesson> TryParseHTML(String html, GPAInfo info, bool isNormal)
        {
            try
            {
                if (isNormal)
                {
                    return Parse(html, info);
                }
                else
                {
                    Assembly asm = Assembly.Load("GPAToolPro");
                    Type util = asm.GetType("GPAToolPro.AdminFunctionLib");
                    Object result = util.InvokeMember("ParseAdminHTMLString", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod
                        , null, null, new object[] { html, info });
                    return (result == null && result is List<Lesson>) ? null : (List<Lesson>)result;
                }
            }
            catch
            {
                return null;
            }
        }

        private static List<Lesson> Parse(String html, GPAInfo info)
        {
            List<Lesson> lessons = new List<Lesson>();
            Regex nameReg = new Regex(@"姓名：.+\s+.+nowrap>(?'name'\w+.+)<", RegexOptions.IgnoreCase);
            Match nameMatch = nameReg.Match(html);
            if (nameMatch.Success)
            {
                info.Name = nameMatch.Groups["name"].Captures[0].Value;
            }
            Regex majorReg = new Regex(@"专业：.+\s+.+nowrap>(?'major'\w+.+)<", RegexOptions.IgnoreCase);
            Match majorMatch = majorReg.Match(html);
            if (majorMatch.Success)
            {
                info.Major = majorMatch.Groups["major"].Captures[0].Value;
            }
            Regex creditReg = new Regex(@"总学分：.+\s+.+nowrap>(?'credit'\w+.+)<", RegexOptions.IgnoreCase);
            Match creditMatch = creditReg.Match(html);
            if (creditMatch.Success)
            {
                info.TotalCredit = creditMatch.Groups["credit"].Captures[0].Value;
            }
            Regex gpaReg = new Regex(@"绩点：.+\s+.+nowrap>(?'gpa'\w+.+)<", RegexOptions.IgnoreCase);
            Match gpaMatch = gpaReg.Match(html);
            if (gpaMatch.Success)
            {
                info.Gpa = gpaMatch.Groups["gpa"].Captures[0].Value;
            }
            Regex lessonReg = new Regex(@"scoreMouseOut.+\s+<td.+\s+(?'id'\d+)\s+.+\s+.+<td.+\s+(?'semester'.+)\s+.+\s+<td.+\s+(?'detailcode'.+)\s+.+\s+<td.+\s+(?'code'.+)\s+.+\s+.+<td.+\s+(?'name'.+)\s+.+\s+<td.+\s+(?'credit'.+)\s+.+\s+<td.+\s+(?'score'.+)\s+</td", RegexOptions.Multiline);
            MatchCollection lessonMatches = lessonReg.Matches(html);
            foreach (Match m in lessonMatches)
            {
                Lesson l = new Lesson();
                l.Code = m.Groups["code"].Value.Replace("\r", "");
                l.Credit = double.Parse(m.Groups["credit"].Value);
                l.Id = int.Parse(m.Groups["id"].Value);
                l.Name = m.Groups["name"].Value.Replace("\r", "");
                l.ScoreString = m.Groups["score"].Value.Replace("\r", "");
                if (l.ScoreString.Contains("*") || l.ScoreString.Contains("?") || l.ScoreString.Contains("P"))
                {
                    l.IsStar = true;
                }
                l.Semester = m.Groups["semester"].Value.Replace("~", "～").Replace("\r", "");
                l.DetailCode = m.Groups["detailcode"].Value.Replace("\r", "");
                l.Score = l.GetScoreValue();
                lessons.Add(l);
            }
            return lessons;
        }
    }
}
