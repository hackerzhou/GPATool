using System;
using System.Collections.Generic;

namespace GPATool.Bean
{
    public class ScoreDistributionItem
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
    }

    public class ScoreItem : Comparer<ScoreItem>
    {
        public String DisplayValue { get; set; }
        public int StudentCount { get; set; }

        public override int Compare(ScoreItem x, ScoreItem y)
        {
            if (string.IsNullOrEmpty(x.DisplayValue) || string.IsNullOrEmpty(y.DisplayValue) || x.DisplayValue[0] != y.DisplayValue[0])
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
                return -1;
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
