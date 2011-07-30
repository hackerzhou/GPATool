using System;
using System.Collections.Generic;
using System.Collections;

namespace GPATool.Bean
{
    public class ScoreDistributionItem : Comparer<ScoreDistributionItem>
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public String Semester { get; set; }
        public String LessonCode { get; set; }
        public String LessonName { get; set; }
        public String Teacher { get; set; }
        public double Credits { get; set; }
        public int StudentCount { get; set; }
        public List<ScoreItem> Scores { get; set; }
        public double AverageScore { get; set; }
        public String Remark { get; set; }

        public void CalculateBenchMark()
        {
            AverageScore = CalculateAverageScore(Scores);
            Remark = GetScoreRemark(this);
        }

        public static double CalculateAverageScore(ICollection<ScoreItem> scores)
        {
            double stuCount = 0;
            double sum = 0;
            foreach (ScoreItem s in scores)
            {
                if (!s.DisplayValue.Contains("*"))
                {
                    stuCount += s.StudentCount;
                    sum += s.StudentCount * Lesson.GetScoreValue(s.DisplayValue);
                }
            }
            return (double)(sum / stuCount);
        }

        public static String GetScoreRemark(ScoreDistributionItem sd)
        {
            ScoreItem[] tempArray = new ScoreItem[sd.Scores.Count];
            sd.Scores.CopyTo(tempArray, 0);
            Array.Sort(tempArray, new ScoreItemStudentComparer());
            String result = "";
            for (int i = 0; i < Math.Min(2, tempArray.Length); i++)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += "; ";
                }
                result += tempArray[i].DisplayValue 
                    + " (" + ((double)tempArray[i].StudentCount / (double)sd.StudentCount).ToString("P") + ")";
            }
            return result;
        }

        public static List<ScoreDistributionItem> MergeSameTeacherSemester(List<ScoreDistributionItem> list)
        {
            Hashtable ht = new Hashtable();
            List<ScoreDistributionItem> removeList = new List<ScoreDistributionItem>();
            foreach (ScoreDistributionItem sdi in list)
            {
                String key = sdi.Teacher + "@" + sdi.LessonName + "@" + sdi.Semester;
                if (!ht.ContainsKey(key))
                {
                    ht.Add(key, sdi);
                }
                else
                {
                    ScoreDistributionItem temp = ht[key] as ScoreDistributionItem;
                    temp.LessonCode += ";" + sdi.LessonCode;
                    temp.Scores.AddRange(sdi.Scores);
                    temp.StudentCount += sdi.StudentCount;
                    removeList.Add(sdi);
                }
            }
            foreach (ScoreDistributionItem sdi in removeList)
            {
                list.Remove(sdi);
            }
            return list;
        }

        public override int Compare(ScoreDistributionItem x, ScoreDistributionItem y)
        {
            return x.Teacher.CompareTo(y.Teacher);
        }
    }

    public class ScoreItemStudentComparer : Comparer<ScoreItem>
    {
        public override int Compare(ScoreItem x, ScoreItem y)
        {
            return -1 * x.StudentCount.CompareTo(y.StudentCount);
        }
    }

    public class ScoreItem : Comparer<ScoreItem>
    {
        public String DisplayValue { get; set; }
        public int StudentCount { get; set; }

        public override int Compare(ScoreItem x, ScoreItem y)
        {
            if (string.IsNullOrEmpty(x.DisplayValue) || string.IsNullOrEmpty(y.DisplayValue) || (x.DisplayValue[0] != y.DisplayValue[0] && Char.IsLetter(x.DisplayValue[0]) && Char.IsLetter(y.DisplayValue[0])))
            {
                return x.DisplayValue.CompareTo(y.DisplayValue);
            }
            else
            {
                return getOrder(x.DisplayValue) - getOrder(y.DisplayValue);
            }
        }

        private int getOrder(String value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return -2;
            }
            else if(!Char.IsLetter(value[0]))
            {
                return 10;
            }
            else if (value.Length == 1)
            {
                return 1;
            }
            switch (value[1])
            {
                case '+': return 0;
                case '-': return 3;
                default: return 2;
            }
        }
    }
}
