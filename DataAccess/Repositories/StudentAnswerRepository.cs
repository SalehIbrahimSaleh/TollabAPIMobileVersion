using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace DataAccess.Repositories
{
    public class StudentAnswerRepository : GenericRepository<StudentAnswer>
    {
        //public async Task<IEnumerable<StudentAnswer>> GetStudentAnswers(long StudentExamId)
        //{
        //    var sql = @"";

        //    return studentAnswers;
        //}
        public async Task<bool> AddBulk(List<StudentAnswer> studentAnswersToSave)
        {
            try
            {
                foreach (var studentAnswer in studentAnswersToSave)
                {
                  var d= await Add(studentAnswer);
                }
                return true;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async  Task<int> DeleteOldAnswer(long? examQuestionId)
        {
         var delete= await  _connectionFactory.GetConnection.ExecuteAsync("delete from StudentAnswer where ExamQuestionId=" + examQuestionId + "");
            return delete;
        }

        internal async Task<IEnumerable<StudentAnswer>> GetStudentAnswers(long ExamId,long StudentId)
        {
           var sql= @"select StudentAnswer.* from StudentAnswer join StudentExam on StudentAnswer.StudentExamId = StudentExam.Id where StudentExam.ExamId = "+ExamId+ " and StudentExam.StudentId = " + StudentId + "";
            var data = await _connectionFactory.GetConnection.QueryAsync<StudentAnswer>(sql);
            return data;
        }
    }
}
