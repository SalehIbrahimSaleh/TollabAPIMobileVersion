using Dapper;
using DataAccess.Entities;
using DataAccess.Entities.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentExamRepository : GenericRepository<StudentExam>
    {
        public async Task<IEnumerable<StudentExamsToCorrect>> GetStuedntExams(long studentId,long? CourseId,long? SolveStatusId, int Page)
        {
            Page = Page * 30;
            var sql = @"select * from (
select distinct StudentCourse.*,Student.Name as StudentName,Course.Name as CourseName,Course.NameLT as CourseNameLT
,Track.Name as TrackName,Track.NameLT as TrackNameLT,Exam.Id as ExamId,Exam.StartDate,Exam.Name as ExamName,Exam.Publish,
IsNull((select top 1 SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and ExamId=Exam.Id) ,0) as SolveStatusId,
IsNull((select top 1 Name from SolveStatus where Id=(select Top 1 SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and ExamId=Exam.Id))  ,'Not Solved')as SolveStatusName,
IsNull((select top 1 NameLT from SolveStatus where Id=(select Top 1 SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and ExamId=Exam.Id)),N'لم يحل') as SolveStatusNameLT,
IsNull((select top 1 Color from SolveStatus where Id=(select Top 1 SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and StudentExam.ExamId=Exam.Id)),N'#f90a0a') as SolveStatusColor,

(case when  IsNull( (select SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and ExamId=Exam.Id) ,0)=0
then 
    (select 'Lost' from Exam InsideExam where InsideExam.Id=Exam.Id and InsideExam.StartDate<(getutcdate())) 
   else 
   ''
   end) as DeadlineDateStatus
   ,
   (case when  IsNull( (select SolveStatusId from StudentExam where StudentId=StudentCourse.StudentId and ExamId=Exam.Id) ,0)=0
then 
    (select N'تأخرت في تسليمه' from Exam InsideExam where InsideExam.Id=Exam.Id and InsideExam.StartDate<(getutcdate())) 
   else 
   ''
   end) as DeadlineDateStatusLT
from StudentCourse join Exam  on Exam.CourseId=StudentCourse.CourseId 
join Student on Student.Id=StudentCourse.StudentId
join Course on Course.Id=StudentCourse.CourseId
join Track on Course.TrackId=Track.Id 
 )  as OuterTable where  StudentId=" + studentId + " and Publish=1 ";
            if (CourseId>0)
            {
                sql = sql + " And  CourseId=" + CourseId + "";
            }
            if (SolveStatusId !=null)
            {
                
                sql = sql + " And SolveStatusId=" + SolveStatusId + "";
            }
            sql = sql + " order by ExamId desc  OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY";
            var StudentExams = await _connectionFactory.GetConnection.QueryAsync<StudentExamsToCorrect>(sql);
            return StudentExams;
        }

        public async Task<bool> UpdateTotalDegreeAndStatusOfSolveing(long studentExamId,long SolveStatusId)
        {
            var StudentExamData = await Get(studentExamId);
            if (StudentExamData!=null)
            {
                var total = await _connectionFactory.GetConnection.QueryFirstAsync<int>("select Sum(Degree) from StudentAnswer where StudentExamId=" + studentExamId + " ");
                StudentExamData.TotalScore = total;
                StudentExamData.SolveStatusId = SolveStatusId;
               var update= await Update(StudentExamData);
                if (update)
                {
                    return true;
                }
            }

            return false;
        }
 
    }
}
