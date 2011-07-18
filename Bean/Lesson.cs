using System;

namespace GPATool.Bean
{
    public class Lesson
    {
        public const double A = 4;
        public const double AM = 3.7;
        public const double BP = 3.3;
        public const double B = 3;
        public const double BM = 2.7;
        public const double CP = 2.3;
        public const double C = 2;
        public const double CM = 1.7;
        public const double D = 1.3;
        public const double DM = 1;
        public const double F = 0;
        private double score = 0;
        private bool isStar = false;

        public bool IsStar
        {
            get { return isStar; }
            set { isStar = value; }
        }
        private String detailCode = null;

        public String DetailCode
        {
            get { return detailCode; }
            set { detailCode = value; }
        }

        private int id = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public double Score
        {
            get { return score; }
            set { score = value; }
        }
        private String name = null;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        private double credit = 0;

        public double Credit
        {
            get { return credit; }
            set { credit = value; }
        }
        private String code = null;

        public String Code
        {
            get { return code; }
            set { code = value; }
        }
        private String scoreString = null;

        public String ScoreString
        {
            get { return scoreString; }
            set { scoreString = value; }
        }
        private String semester = null;

        public String Semester
        {
            get { return semester; }
            set { semester = value; }
        }
        public override string ToString()
        {
            return id + "\t" + semester + "\t" + code + "\t" + name + "\t" + credit + "\t" + scoreString;
        }
        public double getScoreValue()
        {
            if (scoreString.Equals("A"))
            {
                return A;
            }
            else if (scoreString.Equals("A-"))
            {
                return AM;
            }
            else if (scoreString.Equals("B+"))
            {
                return BP;
            }
            else if (scoreString.Equals("B"))
            {
                return B;
            }
            else if (scoreString.Equals("B-"))
            {
                return BM;
            }
            else if (scoreString.Equals("C+"))
            {
                return CP;
            }
            else if (scoreString.Equals("C"))
            {
                return C;
            }
            else if (scoreString.Equals("C-"))
            {
                return CM;
            }
            else if (scoreString.Equals("D"))
            {
                return D;
            }
            else if (scoreString.Equals("D-"))
            {
                return DM;
            }
            else if (scoreString.Equals("F"))
            {
                return F;
            }
            else if (scoreString.Equals("X"))
            {
                return F;
            }
            return 0;
        }
        public static double getScoreValue(String scoreString)
        {
            if (scoreString.Equals("A"))
            {
                return A;
            }
            else if (scoreString.Equals("A-"))
            {
                return AM;
            }
            else if (scoreString.Equals("B+"))
            {
                return BP;
            }
            else if (scoreString.Equals("B"))
            {
                return B;
            }
            else if (scoreString.Equals("B-"))
            {
                return BM;
            }
            else if (scoreString.Equals("C+"))
            {
                return CP;
            }
            else if (scoreString.Equals("C"))
            {
                return C;
            }
            else if (scoreString.Equals("C-"))
            {
                return CM;
            }
            else if (scoreString.Equals("D"))
            {
                return D;
            }
            else if (scoreString.Equals("D-"))
            {
                return DM;
            }
            else if (scoreString.Equals("F"))
            {
                return F;
            }
            else if (scoreString.Equals("X"))
            {
                return F;
            }
            return 0;
        }
    }
}
