using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public class TransactionsUnit : IDisposable
    {

        public PaymentMethodTypeRepository PaymentMethodTypeRepository { get; set; } = new PaymentMethodTypeRepository();

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

        private TrackRepository _trackRepository;
        public TrackRepository TrackRepository
        {
            get
            {
                if (_trackRepository == null)
                {
                    _trackRepository = new  TrackRepository();
                }
                return _trackRepository;
            }
            set
            {
                _trackRepository = value;
            }

        }

        private TrackSubscriptionRepository _trackSubscriptionRepository;
        public TrackSubscriptionRepository TrackSubscriptionRepository
        {
            get
            {
                if (_trackSubscriptionRepository==null)
                {
                    _trackSubscriptionRepository = new TrackSubscriptionRepository();
                }
              return  _trackSubscriptionRepository;
            }
            set
            {
                _trackSubscriptionRepository = value;
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

        private StudentLiveRepository _studentLiveRepository;
        public StudentLiveRepository StudentLiveRepository
        {
            get
            {
                if (_studentLiveRepository == null)
                {
                    _studentLiveRepository = new StudentLiveRepository();
                }
                return _studentLiveRepository;
            }
            set
            {
                _studentLiveRepository = value;
            }
        }

        private StudentTransactionRepository _studentTransactionRepository;
        public StudentTransactionRepository StudentTransactionRepository
        {
            get
            {
                if (_studentTransactionRepository==null)
                {
                    _studentTransactionRepository = new StudentTransactionRepository();
                }
                return _studentTransactionRepository;
            }
            set
            {
                _studentTransactionRepository = value;
            }
        }


        private PromoCodeRepository _promoCodeRepository;
        public PromoCodeRepository PromoCodeRepository
        {
            get
            {
                if (_promoCodeRepository==null)
                {

                    _promoCodeRepository = new PromoCodeRepository();
                }
                return _promoCodeRepository;
            }
            set
            {
                _promoCodeRepository = value;
            }
        }


        private StudentPromoCodeRepository _studentPromoCodeRepository;
        public StudentPromoCodeRepository StudentPromoCodeRepository
        {
            get
            {
                if (_studentPromoCodeRepository==null)
                {
                    _studentPromoCodeRepository = new StudentPromoCodeRepository();

                }
                return _studentPromoCodeRepository;
            }
            set
            {
                _studentPromoCodeRepository = value;
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

        private SystemTransactionRepository _systemTransactionRepository;
        public SystemTransactionRepository SystemTransactionRepository
        {
            get
            {
                if (_systemTransactionRepository==null)
                {
                    _systemTransactionRepository = new SystemTransactionRepository();
                }
                return _systemTransactionRepository;
            }
            set
            {
                _systemTransactionRepository = value;
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


        private StudentNotificationRepository _studentNotificationRepository;
        public StudentNotificationRepository StudentNotificationRepository
        {
            get
            {
                if (_studentNotificationRepository == null)
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
                    _teacherNotificationRepository = new  TeacherNotificationRepository();
                }
                return _teacherNotificationRepository;
            }
            set
            {
                _teacherNotificationRepository = value;
            }
        }


        private InvoiceTransactionRepository _invoiceTransactionRepository;
        public InvoiceTransactionRepository InvoiceTransactionRepository
        {
            get
            {
                if (_invoiceTransactionRepository == null)
                {
                    _invoiceTransactionRepository = new  InvoiceTransactionRepository();
                }
                return _invoiceTransactionRepository;
            }
            set
            {
                _invoiceTransactionRepository = value;
            }

        }

        private DataRepository _dataRepository;
        public DataRepository DataRepository
        {
            get
            {
                if (_dataRepository == null)
                {
                    _dataRepository = new DataRepository();
                }
                return _dataRepository;
            }
            set
            {
                _dataRepository = value;
            }

        }

        private BankResponseRepository _bankResponseRepository;
        public BankResponseRepository BankResponseRepository
        {
            get
            {
                if (_bankResponseRepository == null)
                {
                    _bankResponseRepository = new  BankResponseRepository();
                }
                return _bankResponseRepository;
            }
            set
            {
                _bankResponseRepository = value;
            }

        }


        private CowPayLogRepository _cowPayLogRepository;
        public CowPayLogRepository CowPayLogRepository
        {
            get
            {
                if (_cowPayLogRepository==null)
                {
                    _cowPayLogRepository = new CowPayLogRepository();
                }
                return _cowPayLogRepository;
            }
            set
            {
                _cowPayLogRepository = value;
            }
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
