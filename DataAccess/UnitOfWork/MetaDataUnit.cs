using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public class MetaDataUnit : IDisposable
    {
        private IGenericRepository<Country> _countryRepository;
        private IGenericRepository<PaymentMethod> _paymentMethodRepository;
        private IGenericRepository<Section> _sectionRepository;
        private IGenericRepository<TermAndCondition> _termAndConditionRepository;
        private IGenericRepository<Reference> _referenceRepository;
        private IGenericRepository<CourseStatus> _courseStatusRepository;
       private IGenericRepository<AboutUs> _aboutUsRepository;
       private IGenericRepository<SystemSetting> _systemSettingRepository;
       private IGenericRepository<Setting> _settingRepository;
       private IGenericRepository<ExamQuestionType> _examQuestionTypeRepository;
       private IGenericRepository<ExamType> _examTypeRepository;
       private IGenericRepository<SolveStatus> _solveStatusRepository;



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

        public IGenericRepository<PaymentMethod> PaymentMethodRepository
        {
            get
            {
                if (_paymentMethodRepository == null)
                {

                    _paymentMethodRepository = new GenericRepository<PaymentMethod>();
                }
                return _paymentMethodRepository;
            }

            set
            {
                _paymentMethodRepository = value;
            }
        }

        public IGenericRepository<Section> SectionRepository
        {
            get
            {
                if (_sectionRepository == null)
                {

                    _sectionRepository = new GenericRepository<Section>();
                }
                return _sectionRepository;
            }

            set
            {
                _sectionRepository = value;
            }
        }

        public IGenericRepository<AboutUs> AboutUsRepository
        {
            get
            {
                if (_aboutUsRepository == null)
                {

                    _aboutUsRepository = new GenericRepository<AboutUs>();
                }
                return _aboutUsRepository;
            }

            set
            {
                _aboutUsRepository = value;
            }
        }
        public IGenericRepository<TermAndCondition> TermAndConditionRepository
        {
            get
            {
                if (_termAndConditionRepository == null)
                {

                    _termAndConditionRepository = new GenericRepository<TermAndCondition>();
                }
                return _termAndConditionRepository;
            }

            set
            {
                _termAndConditionRepository = value;
            }
        }

        public IGenericRepository<Reference> ReferenceRepository
        {
            get
            {
                if (_referenceRepository == null)
                {

                    _referenceRepository = new GenericRepository<Reference>();
                }
                return _referenceRepository;
            }

            set
            {
                _referenceRepository = value;
            }
        }

        public IGenericRepository<CourseStatus> CourseStatusRepository
        {
            get
            {
                if (_courseStatusRepository == null)
                {

                    _courseStatusRepository = new GenericRepository<CourseStatus>();
                }
                return _courseStatusRepository;
            }

            set
            {
                _courseStatusRepository = value;
            }
        }

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

        public IGenericRepository<SystemSetting> SystemSettingRepository
        {
            get
            {
                if (_systemSettingRepository == null)
                {
                    _systemSettingRepository = new GenericRepository<SystemSetting>();
                }
                return _systemSettingRepository;
            }

            set
            {
                _systemSettingRepository = value;
            }
        }

        public IGenericRepository<ExamQuestionType> ExamQuestionTypeRepository
        {
            get
            {
                if (_examQuestionTypeRepository == null)
                {

                    _examQuestionTypeRepository = new GenericRepository<ExamQuestionType>();
                }
                return _examQuestionTypeRepository;
            }

            set
            {
                _examQuestionTypeRepository = value;
            }
        }

        public IGenericRepository<ExamType> ExamTypeRepository
        {
            get
            {
                if (_examTypeRepository == null)
                {
                    _examTypeRepository = new GenericRepository<ExamType>();
                }
                return _examTypeRepository;
            }
            set
            {
                _examTypeRepository = value;
            }
        }

        public IGenericRepository<SolveStatus> SolveStatusRepository
        {
            get
            {
                if (_solveStatusRepository == null)
                {
                    _solveStatusRepository = new GenericRepository<SolveStatus>();
                }
                return _solveStatusRepository;
            }
            set
            {
                _solveStatusRepository = value;
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
