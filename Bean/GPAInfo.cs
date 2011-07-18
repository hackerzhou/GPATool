using System;

namespace GPATool.Bean
{
    public class GPAInfo
    {
        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        private String gpa;

        public String Gpa
        {
            get { return gpa; }
            set { gpa = value; }
        }
        private String major;

        public String Major
        {
            get { return major; }
            set { major = value; }
        }
        private String totalCredit;

        public String TotalCredit
        {
            get { return totalCredit; }
            set { totalCredit = value; }
        }
    }
}
