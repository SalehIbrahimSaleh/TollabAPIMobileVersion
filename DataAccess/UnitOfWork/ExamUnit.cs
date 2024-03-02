using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
   public class ExamUnit : IDisposable
    {
        private StudentAnswerRepository _studentAnswerRepository;
        public StudentAnswerRepository StudentAnswerRepository
        {
            get
            {
                if (_studentAnswerRepository==null)
                {
                    _studentAnswerRepository = new StudentAnswerRepository();
                }
                return _studentAnswerRepository;
            }
            set
            {
                _studentAnswerRepository = value;
            }

        }
        private ExamRepository _examRepository;
        public ExamRepository ExamRepository
        {
            get
            {
                if (_examRepository==null)
                {
                    _examRepository = new ExamRepository();
                }
                return _examRepository;
            }
            set
            {
                _examRepository = value;
            }
        }

        private StudentExamRepository _studentExamRepository;
        public StudentExamRepository StudentExamRepository
        {
            get
            {
                if (_studentExamRepository == null)
                {
                    _studentExamRepository = new StudentExamRepository();
                }
                return _studentExamRepository;
            }
            set
            {
                _studentExamRepository = value;
            }
        }
        private ExamQuestionRepository _examQuestionRepository;
        public ExamQuestionRepository ExamQuestionRepository
        {
            get
            {
                if (_examQuestionRepository==null)
                {
                    _examQuestionRepository = new ExamQuestionRepository();
                }
                return _examQuestionRepository;
            }
            set
            {
                _examQuestionRepository = value;
            }
        }
        private TeacherRepository _teacherRepository;
        public TeacherRepository TeacherRepository
        {
            get
            {
                if (_teacherRepository == null)
                {
                    _teacherRepository = new TeacherRepository();
                }
                return _teacherRepository;
            }
            set
            {
                _teacherRepository = value;
            }
        }


        private ExamAnswerRepository _examAnswerRepository;
        public ExamAnswerRepository ExamAnswerRepository
        {
            get
            {
                if (_examAnswerRepository==null)
                {
                    _examAnswerRepository = new ExamAnswerRepository();
                }
                return _examAnswerRepository;
            }
        }

        private TrackRepository _trackRepository;
        public TrackRepository TrackRepository
        {
            get
            {
                if (_trackRepository == null)
                {
                    _trackRepository = new TrackRepository();
                }
                return _trackRepository;
            }
            set
            {
                _trackRepository = value;
            }
        }

         private CourseRepository _courseRepository;
        public CourseRepository CourseRepository
        {
            get
            {
                if (_courseRepository == null)
                {
                    _courseRepository = new CourseRepository();
                }
                return _courseRepository;
            }
            set
            {
                _courseRepository = value;
            }
        }

        private StudentRepository _studentRepository;
        public StudentRepository StudentRepository
        {
            get
            {
                if (_studentRepository == null)
                {
                    _studentRepository = new StudentRepository();
                }
                return _studentRepository;
            }
            set
            {
                _studentRepository = value;
            }
        }

        private StudentCourseRepository _studentCourseRepository;
        public StudentCourseRepository StudentCourseRepository
        {
            get
            {
                if (_studentCourseRepository == null)
                {
                    _studentCourseRepository = new StudentCourseRepository();
                }
                return _studentCourseRepository;
            }
            set
            {
                _studentCourseRepository = value;
            }
        }

        void Complete()
        {
            throw new NotImplementedException();
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MetaDataUnit() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
