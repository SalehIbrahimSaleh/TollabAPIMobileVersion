using DataAccess.DTOs;
using DataAccess.Entities;
using DataAccess.Entities.Views;
using DataAccess.QueryResults;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class ExamService
    {

        public const int SolveStatus_NotComplete = 1;
        public const int Late = 2;
        public const int On_Time = 3;
        ExamUnit _examUnit;
        public ExamService()
        {
            _examUnit = new ExamUnit();
        }

        public async Task<Exam> AddExam(Exam exam)
        {
            var id = await _examUnit.ExamRepository.Add(exam);
            if (id>0)
            {
                return exam;
            }
            return null;
        }
        public async Task<Exam> UpdateExam(Exam exam)
        {
            var update = await _examUnit.ExamRepository.Update(exam);
            if (update)
            {
                return exam;
            }
            return null;
        }
        public async Task<Exam> GetExamById(long ExamId)
        {
            var ExamData = await _examUnit.ExamRepository.Get(ExamId);
            return ExamData;

        }
        public async Task<IEnumerable<StudentCourseExamsResults>> GetStudentsResults(string searchTerm, long? courseId, long teacherId)
        {
            IEnumerable<StudentExamResult> studentsResults = new List<StudentExamResult>();
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm.All(char.IsNumber))
                studentsResults = await _examUnit.ExamRepository.GetStudentExamsResultsWithPhone(searchTerm, courseId, teacherId);
            else
                studentsResults = await _examUnit.ExamRepository.GetStudentExamsResultsWithName(searchTerm, courseId, teacherId);
            var results = studentsResults.GroupBy(a => new { a.StudentId, a.StudentName })
                .Select(a => new StudentCourseExamsResults()
                {
                    StudentName = a.Key.StudentName,
                    StudentId = a.Key.StudentId,
                    StudentTotalResult = a.Sum(i => i.StudentResult),
                    TotalExamsDegree = a.Sum(i => i.ExamDegree),
                    ExamResults = a.Select(r => new ExamResult()
                    {
                        ExamName = r.ExamName,
                        StudentResult = r.StudentResult,
                        ExamDegree = r.ExamDegree
                    }).ToList()
                });
            return results;
        }

        public async Task<ExamView> GetExamDeatailsForTeacher(long ExamId)
        {
            var ExamData = await _examUnit.ExamRepository.GetExamDeatailsForTeacher(ExamId);
            return ExamData;

        }
        public async Task<ExamQuestion> AddOrUpdateExamQuestion(ExamQuestion examQuestion)
        {
            if (examQuestion.Id>0)
            {
             var OldQuestion = await _examUnit.ExamQuestionRepository.Get(examQuestion.Id);
                if (string.IsNullOrEmpty( examQuestion.FilePath))
                {
                    examQuestion.FilePath = OldQuestion.FilePath;
                    examQuestion.IdealAnswerFilePath = OldQuestion.IdealAnswerFilePath;
                }
             var update=   await _examUnit.ExamQuestionRepository.Update(examQuestion);
                if (update)
                {
                    return examQuestion;
                }
                return null;
            }
            var id = await _examUnit.ExamQuestionRepository.Add(examQuestion);
            if (id > 0)
            {
                return examQuestion;
            }
            return null;

        }
        public async Task<ExamQuestion> AddExamQuestionWithAnswers(ExamQuestion examQuestion)
        {
            List<ExamAnswer> examAnswers = new List<ExamAnswer>();
            var id = await _examUnit.ExamQuestionRepository.Add(examQuestion);
            if (id > 0)
            {
                foreach (var answer in examQuestion.ExamAnswers)
                {
                    answer.ExamQuestionId = id;
                    var  answerId= await _examUnit.ExamAnswerRepository.Add(answer);
                    if (answerId>0)
                    {
                        examAnswers.Add(answer);
                    }
                }
                examQuestion.ExamAnswers = examAnswers;
                return examQuestion;
            }
            return null;

        }

        public async Task<ExamQuestion> GetExamQuestionById(long Id)
        {
            var examQuestion = await _examUnit.ExamQuestionRepository.Get(Id);
            if (examQuestion!=null)
            {
                var Answers = await _examUnit.ExamAnswerRepository.GetAll(" where ExamQuestionId=" + examQuestion.Id + "");
                examQuestion.ExamAnswers = Answers.ToList();
            }
            return examQuestion;

        }
        public async Task<bool> DeleteExamQuestion(ExamQuestion examQuestion)
        {
            var delete =_examUnit.ExamQuestionRepository.Delete(examQuestion);
            return delete;
        }

        public async Task<ExamQuestion> UpdateExamQuestionWithAnswers(ExamQuestion examQuestion)
        {
            List<ExamAnswer> examAnswers = new List<ExamAnswer>();
            var update = await _examUnit.ExamQuestionRepository.Update(examQuestion);
            if (update)
            {
                var deleteAllOldAnswers =  _examUnit.ExamAnswerRepository.DeleteWhere(" Where ExamQuestionId=" + examQuestion.Id + "");
                foreach (var answer in examQuestion.ExamAnswers)
                {
                    answer.ExamQuestionId = examQuestion.Id;
                    var answerId = await _examUnit.ExamAnswerRepository.Add(answer);
                    if (answerId > 0)
                    {
                        examAnswers.Add(answer);
                    }
                }
                examQuestion.ExamAnswers = examAnswers;
                return examQuestion;
            }
            return null;
        }

        public async Task<bool> DeleteExamQuestionWithAnswers(ExamQuestion examQuestion)
        {
            var delete = false;
            if (examQuestion.ExamAnswers.Count()>0)
            {
                var deleteAllOldAnswers = _examUnit.ExamAnswerRepository.DeleteWhere(" Where ExamQuestionId=" + examQuestion.Id + "");
            }

             
            delete = _examUnit.ExamQuestionRepository.Delete(examQuestion);
            return delete;
        }

        public async Task<List<ExamQuestion>> GetAllExamQuestionWithAnswers(long examId,int Page)
        {
            var ExamQuestionWithAnswers = await _examUnit.ExamQuestionRepository.GetAllExamQuestionWithAnswers(examId, Page);
            return ExamQuestionWithAnswers;
        }
        public async Task<List<ExamQuestion>> GetAllExamQuestionWithAnswersForStudent(long examId)
        {
            var ExamQuestionWithAnswers = await _examUnit.ExamQuestionRepository.GetAllExamQuestionWithAnswersForStudent(examId);
            return ExamQuestionWithAnswers;
        }


        public async Task<ExamQuestion> GetExamFileByExamId(long ExamId)
        {
            var ExamFile = await _examUnit.ExamQuestionRepository.GetWhere(" where ExamId=" + ExamId + "");
            return ExamFile;
        }

        public async Task<IEnumerable<ExamView>> GetTeacherExams(long TeacherId,long? CourseId,bool? Publish, int Page)
        {
            var Exams = await _examUnit.ExamRepository.GetTeacherExams(TeacherId,CourseId, Publish, Page);
            return Exams;
        }

        public async Task<IEnumerable<StudentExamsToCorrect>> GetStudentExamsToCorrect(long ExamId,int Page,long? solveStatusId)
        {
            var Exams = await _examUnit.ExamRepository.GetStudentExamsToCorrect(ExamId, Page, solveStatusId);
            return Exams;
        }

        public async Task<StudentExam> GetStuedntExamForTeacher(long examId, long studentId)
        {
            var studentExam = await _examUnit.StudentExamRepository.GetWhere(" where ExamId=" + examId + " and StudentId=" + studentId + "");
            return studentExam;
        }

        public async Task<IEnumerable<StudentAnswer>> GetStudentExamAnswers(long StudentExamId)
        {
            var StudentAnswers = await _examUnit.StudentAnswerRepository.GetAll(" where StudentExamId=" + StudentExamId + "");
            foreach (var answer in StudentAnswers)
            {
                var Question = await _examUnit.ExamQuestionRepository.Get(answer.ExamQuestionId);
                answer.ExamQuestion = Question;
                if (answer.ExamAnswerId!=null)
                {
                    var ans = await _examUnit.ExamAnswerRepository.Get(answer.ExamAnswerId);
                    answer.ExamAnswer = ans;
                    answer.ExamQuestion.ExamAnswers = (await _examUnit.ExamAnswerRepository.GetAll(" where ExamQuestionId=" + answer.ExamQuestionId + "")).ToList();

                }
            }
            return StudentAnswers;
        }

        public async Task<IEnumerable<ExaminationSummary>> GetExaminationSummary(long StudentExamId, long examId)
        {
            var ExaminationSummaryData = await _examUnit.ExamRepository.GetExaminationSummary(StudentExamId, examId);
            return ExaminationSummaryData;
        }



        public async Task<IEnumerable<StudentExamsToCorrect>> GetStuedntExam(long StudentId,long? CourseId,long? SolveStatusId, int Page)
        {
            var studentExam = await _examUnit.StudentExamRepository.GetStuedntExams(StudentId, CourseId, SolveStatusId, Page);
            return studentExam;
        }
        public async Task<ExamView> GetExamDeatailsForStudent(long StudentId,long ExamId)
        {
            var ExamData = await _examUnit.ExamRepository.GetExamDeatailsForStudent(StudentId,ExamId);
            return ExamData;

        }

        public async Task<IEnumerable<StudentAnswer>> GetStudentExamAnswersForStudent(long StudentId,long ExamId)
        {
            var StudentAnswers = await _examUnit.StudentAnswerRepository.GetStudentAnswers(ExamId,StudentId);
            foreach (var answer in StudentAnswers)
            {
                var Question = await _examUnit.ExamQuestionRepository.Get(answer.ExamQuestionId);
                answer.ExamQuestion = Question;
                if (answer.ExamAnswerId != null)
                {
                    var ans = await _examUnit.ExamAnswerRepository.Get(answer.ExamAnswerId);
                    answer.ExamAnswer = ans;
                    answer.ExamQuestion.ExamAnswers = (await _examUnit.ExamAnswerRepository.GetAll(" where ExamQuestionId=" + answer.ExamQuestionId + "")).ToList();

                }
            }
            return StudentAnswers;
        }

        public async Task<StudentAnswer> GetStudentAnswer(long StudentAnswerId)
        {
            var studentAnswer = await _examUnit.StudentAnswerRepository.Get(StudentAnswerId);
            return studentAnswer;
        }

        public async Task<StudentAnswer> CorrectEssayQuestion(StudentAnswer studentAnswerData)
        {
            var correct = await _examUnit.StudentAnswerRepository.Update(studentAnswerData);
            if (correct)
            {
                return studentAnswerData;
            }
            return null;
        }

        public async Task<StudentAnswer> AddStudentPdfAnswer(StudentAnswer studentAnswer)
        {
            try
            {
                var DeleteOldAnswer = await _examUnit.StudentAnswerRepository.DeleteOldAnswer(studentAnswer.ExamQuestionId);
            }
            catch{        }
            studentAnswer.Corrected = false;
            var id = await _examUnit.StudentAnswerRepository.Add(studentAnswer);
            if (id>0)
            {
                studentAnswer.Id = id;
                //var ExamData = await _examUnit.ExamRepository.getExamByStudentExamId(studentAnswer.StudentExamId.Value);
                //long SolveStatusId = Late;
                //if (ExamData.DeadlineDate > DateTime.UtcNow)
                //{
                //    SolveStatusId = On_Time;
                //}
                //var UpdateTotalDegreeAndStatusOfSolveing = await _examUnit.StudentExamRepository.UpdateTotalDegreeAndStatusOfSolveing(studentAnswer.StudentExamId.Value, SolveStatusId);
                //if (!UpdateTotalDegreeAndStatusOfSolveing)
                //{
                //    return null;
                //}
                return studentAnswer;
            }
            return null;
        }

        public async Task<StudentExam> StartExam(StudentExam studentExam)
        {
            var id = await _examUnit.StudentExamRepository.Add(studentExam);
            if (id > 0)
            {
                studentExam.Id = id;
                return studentExam;
            }
            return null;
        }

        public bool DeleteExam(Exam examData)
        {
            var  deleteAllQuestions = _examUnit.ExamQuestionRepository.DeleteWhere(" where ExamId=" + examData.Id + " ");
            var delete =  _examUnit.ExamRepository.Delete(examData);
            return delete;
        }

        public async Task<StudentExam> GetStudentExam(long StudentId, long ExamId)
        {
            var studentExam = (await _examUnit.StudentExamRepository.GetAll(" where StudentId=" + StudentId + " and ExamId=" + ExamId + "")).FirstOrDefault() ;
            return studentExam;
        }

        public async Task<StudentExam> GetStudentExamById(long studentExamId)
        {
            var studentExam = (await _examUnit.StudentExamRepository.GetAll(" where  Id=" + studentExamId + "")).FirstOrDefault();
            return studentExam;
        }

        public async Task<IEnumerable<StudentAnswer>> SaveStudentAnswers(List<StudentAnswer> studentAnswers,long StudentExamId)
        {
            List<StudentAnswer> studentAnswersToSave = new List<StudentAnswer>();
            foreach (var itemAnswer  in studentAnswers)
            {
                var Question = await _examUnit.ExamQuestionRepository.Get(itemAnswer.ExamQuestionId);
                ExamAnswer  examAnswer=null;
                if (itemAnswer.ExamAnswerId>0)
                {
                    examAnswer = await _examUnit.ExamAnswerRepository.Get(itemAnswer.ExamAnswerId);

                }
                if (Question.ExamQuestionTypeId!=3&& Question.ExamQuestionTypeId != 4)
                {
                    if (examAnswer!=null)
                    {
                        if (examAnswer.IsTrue == true)
                        {
                            itemAnswer.Degree = Question.Degree;
                            itemAnswer.IsTrue = true;
                            itemAnswer.Corrected = true;
                        }
                        else
                        {
                            itemAnswer.Degree = 0;
                            itemAnswer.IsTrue = false;
                            itemAnswer.Corrected = true;

                        }

                    }

                }
                else
                {
                    itemAnswer.Corrected = false;
                    itemAnswer.Degree = 0;
                }
                itemAnswer.CreationDate = DateTime.UtcNow;
                var isAdded = studentAnswersToSave.Where(i => i.StudentExamId == itemAnswer.StudentExamId && i.ExamQuestionId == itemAnswer.ExamQuestionId && i.ExamAnswerId == itemAnswer.ExamAnswerId).FirstOrDefault();
                if (isAdded!=null)
                {
                    continue;
                }
                studentAnswersToSave.Add(itemAnswer);
            }
            await _examUnit.StudentAnswerRepository.AddBulk(studentAnswersToSave);
            var StudentAnswers = await GetStudentExamAnswers(StudentExamId);
            if (StudentAnswers.Count()==0)
            {
                return null;
            }
            var ExamData = await _examUnit.ExamRepository.getExamByStudentExamId(StudentExamId);
            long SolveStatusId = Late;
            if (ExamData.DeadlineDate>DateTime.UtcNow)
            {
                SolveStatusId = On_Time;
            }
           var UpdateTotalDegreeAndStatusOfSolveing = await _examUnit.StudentExamRepository.UpdateTotalDegreeAndStatusOfSolveing(StudentExamId,SolveStatusId);
            if (!UpdateTotalDegreeAndStatusOfSolveing)
            {
                return null;
            }
            return StudentAnswers;
        }

        public async Task UpdateStudentExam(StudentExam updateStudentExam)
        {
            var d = await _examUnit.StudentExamRepository.Update(updateStudentExam);
        }

        public async Task<IEnumerable<StudentExam>> CheckIfThisExamSolvedByStudents(long ExamId)
        {
            var studentExams = await _examUnit.StudentExamRepository.GetAll(" where ExamId=" + ExamId + "");
            return studentExams;
        }
    }
}
