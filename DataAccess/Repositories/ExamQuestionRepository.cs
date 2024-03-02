using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class ExamQuestionRepository:GenericRepository<ExamQuestion>
    {
        public async Task<List<ExamQuestion>> GetAllExamQuestionWithAnswers(long examId,int Page)
        {
            //Page = Page * 30;
            if (Page>0)
            {
                var ExamQuestionEmptyList = new List<ExamQuestion>();
                return ExamQuestionEmptyList;

            }
            string sql = @"select * from ExamQuestion left join ExamAnswer on ExamQuestion.Id=ExamAnswer.ExamQuestionId where ExamQuestion.ExamId=" + examId + "";

            var ExamQuestionDictionary = new Dictionary<long, ExamQuestion>();
            var list = _connectionFactory.GetConnection.Query<ExamQuestion, ExamAnswer, ExamQuestion>(
                sql,
                (examQuestion, examanswer) =>
                {
                    ExamQuestion ExamQuestionEntry;

                    if (!ExamQuestionDictionary.TryGetValue(examQuestion.Id, out ExamQuestionEntry))
                    {
                        ExamQuestionEntry = examQuestion;
                        ExamQuestionEntry.ExamAnswers = new List<ExamAnswer>();
                        ExamQuestionDictionary.Add(ExamQuestionEntry.Id, ExamQuestionEntry);
                    }
                    if (examanswer!=null)
                    {
                        ExamQuestionEntry.ExamAnswers.Add(examanswer);

                    }

                    return ExamQuestionEntry;
                },
                splitOn: "Id")
                .OrderBy(i => i.OrderNumber)
            .Distinct() 
            .ToList();
            return list;
        }

        public async Task<List<ExamQuestion>> GetAllExamQuestionWithAnswersForStudent(long examId)
        {
            string sql = @"select * from ExamQuestion left join ExamAnswer on ExamQuestion.Id=ExamAnswer.ExamQuestionId where ExamQuestion.ExamId=" + examId + "";

            var ExamQuestionDictionary = new Dictionary<long, ExamQuestion>();
            var list = _connectionFactory.GetConnection.Query<ExamQuestion, ExamAnswer, ExamQuestion>(
                sql,
                (examQuestion, examanswer) =>
                {
                    ExamQuestion ExamQuestionEntry;

                    if (!ExamQuestionDictionary.TryGetValue(examQuestion.Id, out ExamQuestionEntry))
                    {
                        ExamQuestionEntry = examQuestion;
                        // hide the ideal file from question in start exam
                        ExamQuestionEntry.IdealAnswerFilePath = null;
                        ExamQuestionEntry.ExamAnswers = new List<ExamAnswer>();
                        ExamQuestionDictionary.Add(ExamQuestionEntry.Id, ExamQuestionEntry);
                    }
                    if (examanswer != null)
                    {
                        if (ExamQuestionEntry.ExamQuestionTypeId!=3)
                        {
                            ExamQuestionEntry.ExamAnswers.Add(examanswer);

                        }
                    }
                    return ExamQuestionEntry;
                },
                splitOn: "Id")
                .OrderBy(i => i.OrderNumber)
            .Distinct()
            .ToList();
            return list;
        }

    }
}
