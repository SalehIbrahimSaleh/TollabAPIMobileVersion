using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
   public class TeacherUnit:IDisposable
    {
        private IGenericRepository<Country> _countryRepository;

        public IGenericRepository<Country> CountryRepository
        {
            get
            {
                if (_countryRepository == null)
                {
                    _countryRepository = new GenericRepository<Country>();
                }
                return _countryRepository;
            }

            set
            {
                _countryRepository = value;
            }
        }




        private TeacherRepository _teacherRepository;
        public TeacherRepository TeacherRepository
        {
            get
            {
                if (_teacherRepository==null)
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


        private TeacherAssistantRepository _teacherAssistantRepository;
        public TeacherAssistantRepository TeacherAssistantRepository
        {
            get
            {
                if (_teacherAssistantRepository==null)
                {
                    _teacherAssistantRepository = new TeacherAssistantRepository();
                }
                return _teacherAssistantRepository;
            }
            set
            {
                _teacherAssistantRepository = value;
            }
        }


        private TeacherSubjectRepository _teacherSubjectRepository;
        public TeacherSubjectRepository TeacherSubjectRepository
        {
            get
            {
                if (_teacherSubjectRepository == null)
                {
                    _teacherSubjectRepository = new TeacherSubjectRepository();
                }
                return _teacherSubjectRepository;
            }
            set
            {
                _teacherSubjectRepository = value;
            }
        }

        private TrackRepository _trackRepository;
        public TrackRepository TrackRepository
        {
            get
            {
                if (_trackRepository==null)
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

        private SubjectRepository _subjectRepository;
        public SubjectRepository SubjectRepository
        {
            get
            {
                if (_subjectRepository==null)
                {
                    _subjectRepository = new SubjectRepository();

                }
                return _subjectRepository;
            }
            set
            {
                _subjectRepository = value;
            }
        }

        private CourseDepartmentRepository _courseDepartmentRepository;
        public CourseDepartmentRepository CourseDepartmentRepository
        {
            get
            {
                if (_courseDepartmentRepository==null)
                {
                    _courseDepartmentRepository = new CourseDepartmentRepository();
                }
                return _courseDepartmentRepository;
            }
            set
            {
                _courseDepartmentRepository = value;
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

        private GroupRepository _groupRepository;
        public GroupRepository GroupRepository
        {
            get
            {
                if (_groupRepository==null)
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

        private ContentRepository _contentRepository;
        public ContentRepository ContentRepository
        {
            get
            {
                if (_contentRepository==null)
                {
                    _contentRepository = new ContentRepository();

                }
                return _contentRepository;
            }
            set
            {

            }
        }
        private ContentCourseTrackUserRepository _contentCourseTrackUserRepository;
        public ContentCourseTrackUserRepository ContentCourseTrackUserRepository
        {
            get
            {
                if (_contentCourseTrackUserRepository == null)
                {
                    _contentCourseTrackUserRepository = new ContentCourseTrackUserRepository();

                }
                return _contentCourseTrackUserRepository;
            }
            set
            {

            }
        }

        private TeacherTransactionRepository _teacherTransactionRepository;
        public TeacherTransactionRepository TeacherTransactionRepository
        {
            get
            {
                if (_teacherTransactionRepository==null)
                {
                    _teacherTransactionRepository = new TeacherTransactionRepository();
                }
                return _teacherTransactionRepository;
            }
            set
            {
                _teacherTransactionRepository = value;
            }
        }

        private TeacherNotificationRepository _teacherNotificationRepository;
        public TeacherNotificationRepository TeacherNotificationRepository
        {
            get
            {
                if (_teacherNotificationRepository==null)
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

        private TeacherPushTokenRepository _teacherPushTokenRepository;
        public TeacherPushTokenRepository TeacherPushTokenRepository
        {
            get
            {
                if (_teacherPushTokenRepository==null)
                {
                    _teacherPushTokenRepository = new TeacherPushTokenRepository();
                }
                return _teacherPushTokenRepository;
            }
            set
            {
                _teacherPushTokenRepository = value;
            }
        }

        private LiveRepository _liveRepository;
        public LiveRepository LiveRepository
        {
            get
            {
                if (_liveRepository == null)
                {
                    _liveRepository = new LiveRepository();
                }
                return _liveRepository;
            }
            set
            {
                _liveRepository = value;
            }
        }

        private LiveAttachmentRepository _liveAttachmentRepository;
        public LiveAttachmentRepository LiveAttachmentRepository
        {
            get
            {
                if (_liveAttachmentRepository == null)
                {
                    _liveAttachmentRepository = new LiveAttachmentRepository();
                }
                return _liveAttachmentRepository;
            }
            set
            {
                _liveAttachmentRepository = value;
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
