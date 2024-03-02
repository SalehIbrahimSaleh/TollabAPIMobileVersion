using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
   public class StudentUnit:IDisposable
    {

        public DisableReasonRepository DisableReasonRepository { get; set; } = new DisableReasonRepository();
        public SecurityLogRepository SecurityLogRepository { get; set; } = new SecurityLogRepository();

        private TokenStoreRepository _tokenStoreRepository;
        public TokenStoreRepository TokenStoreRepository
        {
            get
            {
                if (_tokenStoreRepository==null)
                {
                    _tokenStoreRepository = new TokenStoreRepository();
                }
                return _tokenStoreRepository;
            }
            set
            {
                _tokenStoreRepository = value;
            }
        }
        private IGenericRepository<Setting> _settingRepository;
        public IGenericRepository<Setting> SettingRepository
        {
            get
            {
                if (_settingRepository == null)
                {

                    _settingRepository = new GenericRepository<Setting>();
                }
                return _settingRepository;
            }

            set
            {
                _settingRepository = value;
            }
        }
        private StudentCourseDownloadedRepository _studentCourseDownloadedRepository;
        public StudentCourseDownloadedRepository StudentCourseDownloadedRepository
        {
            get
            {
                if (_studentCourseDownloadedRepository == null)
                {
                    _studentCourseDownloadedRepository = new StudentCourseDownloadedRepository();

                }
                return _studentCourseDownloadedRepository;
            }
            set
            {
                _studentCourseDownloadedRepository = value;
            }
        }
        private StudentCourseContentRepository _studentCourseContentRepository;
        public StudentCourseContentRepository StudentCourseContentRepository
        {
            get
            {
                if (_studentCourseContentRepository == null)
                {
                    _studentCourseContentRepository = new StudentCourseContentRepository();

                }
                return _studentCourseContentRepository;
            }
            set
            {
                _studentCourseContentRepository = value;
            }
        }
        private StudentPackageRepository _studentPackageRepository;
        public StudentPackageRepository StudentPackageRepository
        {
            get
            {
                if (_studentPackageRepository == null)
                {
                    _studentPackageRepository = new StudentPackageRepository();

                }
                return _studentPackageRepository;
            }
            set
            {
                _studentPackageRepository = value;
            }
        }
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
        private TrackSubscriptionRepository _trackSubscriptionRepository;

        public TrackSubscriptionRepository TrackSubscriptionRepository
        {
            get
            {
                if (_trackSubscriptionRepository == null)
                {
                    _trackSubscriptionRepository = new TrackSubscriptionRepository();
                }
                return _trackSubscriptionRepository;
            }
            set
            {
                _trackSubscriptionRepository = value;
            }
        }

        private LogErrorRepository _logErrorRepository;
        public LogErrorRepository LogErrorRepository
        {
            get
            {
                if (_logErrorRepository == null)
                {
                    _logErrorRepository = new LogErrorRepository();
                }
                return _logErrorRepository;
            }
            set
            {
                _logErrorRepository = value;
            }

        }

        private StudentRepository _studentRepository;
        public StudentRepository StudentRepository
        {
            get
            {
                if (_studentRepository==null)
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

         
        private OfflinePackageRepository _offlinePackageRepository;
        public OfflinePackageRepository OfflinePackageRepository
        {
            get
            {
                if (_offlinePackageRepository == null)
                {
                    _offlinePackageRepository = new OfflinePackageRepository();
                }
                return _offlinePackageRepository;
            }
            set
            {
                _offlinePackageRepository = value;
            }
        }
        

        private DownloadObjRepository _downloadObjRepository;
        public DownloadObjRepository DownloadObjRepository
        {
            get
            {
                if (_downloadObjRepository == null)
                {
                    _downloadObjRepository = new DownloadObjRepository();
                }
                return _downloadObjRepository;
            }
            set
            {
                _downloadObjRepository = value;
            }
        }

        private SectionRepository _sectionRepository;
        public SectionRepository SectionRepository
        {
            get
            {
                if (_sectionRepository == null)
                {
                    _sectionRepository = new SectionRepository();
                }
                return _sectionRepository;
            }
            set
            {
                _sectionRepository = value;
            }

        }


        private CategoryRepository _categoryRepository;
        public CategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository==null)
                {
                    _categoryRepository = new CategoryRepository();
                }
                return _categoryRepository;
            }
            set
            {
                _categoryRepository = value;
            }
        }


        private SubCategoryRepository _subCategoryRepository;
        public SubCategoryRepository SubCategoryRepository
        {
            get
            {
                if (_subCategoryRepository == null)
                {
                    _subCategoryRepository = new SubCategoryRepository();
                }
                return _subCategoryRepository;
            }
            set
            {
                _subCategoryRepository = value;
            }
        }

        private DepartmentRepository _departmentRepository;
        public DepartmentRepository DepartmentRepository
        {
            get
            {
                if (_departmentRepository == null)
                {
                    _departmentRepository = new DepartmentRepository();
                }
                return _departmentRepository;
            }
            set
            {
                _departmentRepository = value;
            }
        }

        private StudentDepartmentRepository _studentDepartmentRepository;
        public StudentDepartmentRepository StudentDepartmentRepository
        {
            get
            {
                if (_studentDepartmentRepository == null)
                {
                    _studentDepartmentRepository = new  StudentDepartmentRepository();
                }
                return _studentDepartmentRepository;
            }
            set
            {
                _studentDepartmentRepository = value;
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
        private TrackPromotionRepository _trackPromotionRepository;
        public TrackPromotionRepository TrackPromotionRepository
        {
            get
            {
                if (_trackPromotionRepository == null)
                {
                    _trackPromotionRepository = new TrackPromotionRepository();
                }
                return _trackPromotionRepository;
            }
            set
            {
                _trackPromotionRepository = value;
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
                _contentRepository = value;
            }
        }
        private TrackPromotionCourseRepository _trackPromotionCourseRepository;
        public TrackPromotionCourseRepository TrackPromotionCourseRepository
        {
            get
            {
                if (_trackPromotionCourseRepository == null)
                {
                    _trackPromotionCourseRepository = new TrackPromotionCourseRepository();
                }
                return _trackPromotionCourseRepository;
            }
            set
            {
                _trackPromotionCourseRepository = value;
            }
        }
        private OfferRepository _offerRepository;
        public OfferRepository OfferRepository
        {
            get
            {
                if (_offerRepository == null)
                {
                    _offerRepository = new OfferRepository();
                }
                return _offerRepository;
            }
            set
            {
              _offerRepository = value;
            }
        }

        private StudentCourseRepository _studentCourseRepository;
        public StudentCourseRepository StudentCourseRepository
        {
            get
            {
                if (_studentCourseRepository==null)
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

        private CourseRepository _courseRepository;
        public CourseRepository CourseRepository
        {
            get
            {
                if (_courseRepository==null)
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

        private SearchWordRepository _searchWordRepository;
        public SearchWordRepository SearchWordRepository
        {
            get
            {
                if (_searchWordRepository==null)
                {
                    _searchWordRepository = new SearchWordRepository();
                }
                return _searchWordRepository;
            }
            set
            {
                _searchWordRepository = value;
            }
        }

        private StudentContentRepository _studentContentRepository;
        public StudentContentRepository StudentContentRepository
        {
            get
            {
                if (_studentContentRepository==null)
                {
                    _studentContentRepository = new StudentContentRepository();
                }
                return _studentContentRepository;
            }

            set
            {
                _studentContentRepository = value;
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

        private FavouriteRepository _favouriteRepository;
        public FavouriteRepository FavouriteRepository
        {
            get
            {
                if (_favouriteRepository==null)
                {
                    _favouriteRepository = new FavouriteRepository();
                }
                return _favouriteRepository;
            }
            set
            {
                _favouriteRepository = value;
            }
        }


        private ContactUsRepository _contactUsRepository;
        public ContactUsRepository ContactUsRepository
        {
            get
            {
                if (_contactUsRepository==null)
                {
                    _contactUsRepository = new ContactUsRepository();
                }
                return _contactUsRepository;
            }
            set
            {
                _contactUsRepository = value;
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

        private StudentPushTokenRepository _studentPushTokenRepository;
        public StudentPushTokenRepository StudentPushTokenRepository
        {
            get
            {
                if (_studentPushTokenRepository==null)
                {
                    _studentPushTokenRepository = new StudentPushTokenRepository();
                }
                return _studentPushTokenRepository;

            }
            set
            {
                _studentPushTokenRepository = value;
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


        private TeacherTransactionRepository _teacherTransactionRepository;
        public TeacherTransactionRepository TeacherTransactionRepository
        {
            get
            {
                if (_teacherTransactionRepository == null)
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

        private UserDeviceLogRepository _userDeviceLogRepository;
        public UserDeviceLogRepository UserDeviceLogRepository
        {
            get
            {
                if (_userDeviceLogRepository==null)
                {
                    _userDeviceLogRepository = new UserDeviceLogRepository();
                }
                return _userDeviceLogRepository;
            }
            set
            {
                _userDeviceLogRepository = value;
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
