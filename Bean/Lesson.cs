using System;
using System.Collections.Generic;

namespace GPATool.Bean
{
    public class LessonScore
    {
        public String DisplayScore { get; set; }
        public double Value { get; set; }
        public LessonScore(String d, double v)
        {
            DisplayScore = d;
            Value = v;
        }
    }

    public class Lesson
    {
        private static LessonScore[] lessonScoreTypes = new LessonScore[] {
            new LessonScore("A",4.0),
            new LessonScore("A-",3.7),
            new LessonScore("B+",3.3),
            new LessonScore("B",3.0),
            new LessonScore("B-",2.7),
            new LessonScore("C+",2.3),
            new LessonScore("C",2.0),
            new LessonScore("C-",1.7),
            new LessonScore("D",1.3),
            new LessonScore("D-",1.0),
            new LessonScore("F",0.0)
        };
        public bool IsStar { get; set; }
        public String DetailCode { get; set; }
        public int Id { get; set; }
        public double Score { get; set; }
        public String Name { get; set; }
        public double Credit { get; set; }
        public String Code { get; set; }
        public String ScoreString { get; set; }
        public String Semester { get; set; }

        public override string ToString()
        {
            return Id + "\t" + Semester + "\t" + Code + "\t" + Name + "\t" + Credit + "\t" + ScoreString;
        }

        public double GetScoreValue()
        {
            return GetScoreValue(this.ScoreString);
        }

        public double CalculateGPA(List<Lesson> list)
        {
            double sum = 0;
            double creditPointsSum = 0;
            foreach (Lesson l in list)
            {
                if (!l.IsStar)
                {
                    sum = sum + (l.Credit * l.Score);
                    creditPointsSum += l.Credit;
                }
            }
            return (double)(sum / creditPointsSum);
        }

        public static String GetScoreDetailString(double score, double delta = 0.05)
        {
            String result = null;
            double lowerBoundDelta = double.MaxValue;
            String lowerBound = null;
            double upperBoundDelta = double.MaxValue;
            String upperBound = null;
            foreach (LessonScore ls in lessonScoreTypes)
            {
                if (Math.Abs(score - ls.Value) < delta)
                {
                    return ls.DisplayScore;
                }
                if (score > ls.Value && score - ls.Value < lowerBoundDelta)
                {
                    lowerBound = ls.DisplayScore;
                    lowerBoundDelta = score - ls.Value;
                }
                if (score < ls.Value && ls.Value - score < upperBoundDelta)
                {
                    upperBound = ls.DisplayScore;
                    upperBoundDelta = ls.Value - score;
                }
            }
            result = lowerBound + "到" + upperBound;
            return result;
        }

        public static double GetScoreValue(String scoreString)
        {
            foreach (LessonScore ls in lessonScoreTypes)
            {
                if (ls.DisplayScore.Equals(scoreString))
                {
                    return ls.Value;
                }
            }
            return 0;
        }
    }
}
