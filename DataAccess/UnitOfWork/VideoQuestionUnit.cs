using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
   public class VideoQuestionUnit:IDisposable
    {

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

        private VideoQuestionRepository _videoQuestionRepository;
        public VideoQuestionRepository VideoQuestionRepository
        {
            get
            {
                if (_videoQuestionRepository==null)
                {
                    _videoQuestionRepository = new VideoQuestionRepository();
                }
                return _videoQuestionRepository;
            }
            set
            {
                 _videoQuestionRepository = value;
            }
        }

        private ReplyRepository _replyRepository;
        public ReplyRepository ReplyRepository
        {
            get
            {
                if (_replyRepository==null)
                {
                    _replyRepository = new ReplyRepository();
                }
                return _replyRepository;
            }
            set
            {
                _replyRepository = value;
            }
        }

        private StudentNotificationRepository _studentNotificationRepository;
        public StudentNotificationRepository StudentNotificationRepository
        {
            get
            {
                if (_studentNotificationRepository==null)
                {
                    _studentNotificationRepository = new StudentNotificationRepository();
                }
                return _studentNotificationRepository;
            }
            set
            {
                _studentNotificationRepository = value;
            }
        }
        private TeacherNotificationRepository _teacherNotificationRepository;
        public TeacherNotificationRepository TeacherNotificationRepository
        {
            get
            {
                if (_teacherNotificationRepository == null)
                {
                    _teacherNotificationRepository = new TeacherNotificationRepository();
                }
                return _teacherNotificationRepository;
            }
            set
            {
                _teacherNotificationRepository = value;
            }
        }

        private GroupRepository _groupRepository;
        public GroupRepository GroupRepository
        {
            get
            {
                if (_groupRepository == null)
                {
                    _groupRepository = new GroupRepository();
                }
                return _groupRepository;
            }
            set
            {
                _groupRepository = value;
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

        private CourseRepository _courseRepository ;
        public CourseRepository CourseRepository
        {
            get
            {
                if (_courseRepository == null)
                {
                    _courseRepository = new  CourseRepository();
                }
                return _courseRepository;
            }
            set
            {
                _courseRepository = value;
            }
        }

        private ContentRepository _contentRepository;
        public ContentRepository ContentRepository
        {
            get
            {
                if (_contentRepository == null)
                {
                    _contentRepository = new ContentRepository();

                }
                return _contentRepository;
            }
            set
            {

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
