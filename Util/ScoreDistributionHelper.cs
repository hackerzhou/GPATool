using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using GPATool.Bean;
using System.Text.RegularExpressions;

namespace GPATool.Util
{
    public class ScoreDistributionHelper
    {
        private static SQLiteConnection conn;

        public static SQLiteConnection GetSqliteConnection()
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                SQLiteConnectionStringBuilder connStr = new SQLiteConnectionStringBuilder();
                connStr.DataSource = "data.s3db";
                connStr.Password = "GPAToolScoreDistributionDb2011";
                conn = new SQLiteConnection(connStr.ConnectionString);
                conn.Open();
            }
            return conn;
        }

        public static List<String> LoadAllSemesters()
        {
            List<String> result = new List<String>();
            try
            {
                SQLiteConnection conn = GetSqliteConnection();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select name from Semester order by name desc;";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            catch
            {
            }
            return result;
        }

        public static void QueryScoreDistribution(ScoreDistributionItem item)
        {
            int courseId = item.CourseId;
            SQLiteConnection conn = GetSqliteConnection();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select scoreValue, studentCount from score where course_id = " + courseId + " order by scoreValue";
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (item.Scores == null)
            {
                item.Scores = new List<ScoreItem>();
            }
            else
            {
                item.Scores.Clear();
            }

            while (reader.Read())
            {
                ScoreItem si = new ScoreItem();
                si.DisplayValue = reader.GetString(0);
                si.StudentCount = reader.GetInt32(1);
                item.Scores.Add(si);
            }
            item.Scores.Sort(new ScoreItem());
        }

        public static List<ScoreDistributionItem> QueryTeacherSDChangeBySemester(ScoreDistributionItem item)
        {
            String sql = @"select
                               c.id,
                               s.name,
                               l.lessonCode,
                               l.lessonName,
                               t.name,
                               l.creditPoint,
                               c.totalStudentNumber
                           from
                               course c, lesson l, semester s, teacher t
                           where
                               c.lesson_id = l.id
                               and c.semester_id = s.id
                               and c.teacher_id = t.id
                               and t.name = '" + item.Teacher + "'"
                          + @" and l.lessonName = '" + item.LessonName + "'"
                          + @" and t.name != 'Unknown'
                               order by s.name";
            List<ScoreDistributionItem> result = new List<ScoreDistributionItem>();
            SQLiteConnection conn = GetSqliteConnection();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                ScoreDistributionItem sdItem = new ScoreDistributionItem();
                sdItem.Id = i++;
                sdItem.CourseId = reader.GetInt32(0);
                sdItem.Semester = reader.GetString(1);
                sdItem.LessonCode = reader.GetString(2);
                sdItem.LessonName = reader.GetString(3);
                sdItem.Teacher = reader.GetString(4);
                sdItem.Credits = reader.GetDouble(5);
                sdItem.StudentCount = reader.GetInt32(6);
                QueryScoreDistribution(sdItem);
                result.Add(sdItem);
            }
            result = ScoreDistributionItem.MergeSameTeacherSemester(result);
            foreach (ScoreDistributionItem sdi in result)
            {
                sdi.CalculateBenchMark();
            }
            return result;
        }

        public static List<ScoreDistributionItem> QueryLessonSDChangeByTeacher(ScoreDistributionItem item)
        {
            String sql = @"select
                               c.id,
                               s.name,
                               l.lessonCode,
                               l.lessonName,
                               t.name,
                               l.creditPoint,
                               c.totalStudentNumber
                           from
                               course c, lesson l, semester s, teacher t
                           where
                               c.lesson_id = l.id
                               and c.semester_id = s.id
                               and c.teacher_id = t.id
                               and l.lessonName = '" + item.LessonName + "'"
                          + @" and s.name = '" + item.Semester + "'"
                          + @" and t.name != 'Unknown' 
                               order by t.name";
            List<ScoreDistributionItem> result = new List<ScoreDistributionItem>();
            SQLiteConnection conn = GetSqliteConnection();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                ScoreDistributionItem sdItem = new ScoreDistributionItem();
                sdItem.Id = i++;
                sdItem.CourseId = reader.GetInt32(0);
                sdItem.Semester = reader.GetString(1);
                sdItem.LessonCode = reader.GetString(2);
                sdItem.LessonName = reader.GetString(3);
                sdItem.Teacher = reader.GetString(4);
                sdItem.Credits = reader.GetDouble(5);
                sdItem.StudentCount = reader.GetInt32(6);
                QueryScoreDistribution(sdItem);
                result.Add(sdItem);
            }
            result = ScoreDistributionItem.MergeSameTeacherSemester(result);
            foreach (ScoreDistributionItem sdi in result)
            {
                sdi.CalculateBenchMark();
            }
            result.Sort(new ScoreDistributionItem());
            return result;
        }

        public static List<ScoreDistributionItem> Query(String semester, String lessonCodeContains
            , String lessonNameContains, String teacherNameContains, int ignoreLessThan, int limit
            , bool ignoreImcompleteInfo, String orderBy)
        {
            List<ScoreDistributionItem> result = new List<ScoreDistributionItem>();
            SQLiteConnection conn = GetSqliteConnection();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = createSql(semester, lessonCodeContains, lessonNameContains
                                , teacherNameContains, ignoreLessThan, limit, ignoreImcompleteInfo, orderBy);
            SQLiteDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                ScoreDistributionItem item = new ScoreDistributionItem();
                item.Id = i++;
                item.CourseId = reader.GetInt32(0);
                item.Semester = reader.GetString(1);
                item.LessonCode = reader.GetString(2);
                item.LessonName = reader.GetString(3);
                item.Teacher = reader.GetString(4);
                item.Credits = reader.GetDouble(5);
                item.StudentCount = reader.GetInt32(6);
                result.Add(item);
            }
            return result;
        }

        private static String createSql(String semester, String lessonCodeContains
            , String lessonNameContains, String teacherNameContains, int ignoreLessThan, int limit, bool ignoreImcompleteInfo, String orderBy)
        {
            String sql = @"select
                               c.id,
                               s.name,
                               l.lessonCode,
                               l.lessonName,
                               t.name,
                               l.creditPoint,
                               c.totalStudentNumber
                           from
                               course c, lesson l, semester s, teacher t
                           where
                               c.lesson_id = l.id
                               and c.semester_id = s.id
                               and c.teacher_id = t.id";
            if (!string.IsNullOrEmpty(semester))
            {
                sql += " and s.name = '" + semester + "'";
            }
            if (!string.IsNullOrEmpty(lessonCodeContains))
            {
                sql += " and l.lessonCode like '%" + lessonCodeContains + "%'";
            }
            if (!string.IsNullOrEmpty(lessonNameContains))
            {
                sql += " and l.lessonName like '%" + lessonNameContains + "%'";
            }
            if (!string.IsNullOrEmpty(teacherNameContains))
            {
                sql += " and t.name like '%" + teacherNameContains + "%'";
            }
            if (ignoreLessThan > 0)
            {
                sql += " and c.totalStudentNumber >= " + ignoreLessThan;
            }
            if (ignoreImcompleteInfo)
            {
                sql += @" and t.name != 'Unknown'";
            }
            sql += string.IsNullOrEmpty(orderBy) ? " order by lessonCode" : " " + orderBy;
            if (limit > 0)
            {
                sql += " limit " + limit;
            }
            return sql;
        }
    }
}
