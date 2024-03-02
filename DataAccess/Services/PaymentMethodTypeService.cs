using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class PaymentMethodTypeService
    {
        PaymentMethodTypeRepository _paymentMethodTypeRepository;

        private GenericRepository<Setting> _settingRepository;


        public PaymentMethodTypeService()
        {
            _paymentMethodTypeRepository = new PaymentMethodTypeRepository();
            _settingRepository = new GenericRepository<Setting>();
        }

        public async Task<IEnumerable<PaymentMethodType>> GetPaymentMethodTypeAsync(string BuildNumber, string CountryCode)
        {
            var BuildNumberFromDB = (await _settingRepository.GetAll(" where SettingKey='InReviewBuild'")).FirstOrDefault().SettingValue;
            if (BuildNumber != BuildNumberFromDB)
            {
                var Data1 = await _paymentMethodTypeRepository.GetAllByQuery(@"select PaymentMethodType.* from PaymentMethodType join Country on PaymentMethodType.CountryId = Country.Id
                                        where CountryCode ='" + CountryCode + "' and PaymentMethodType.InAppPurchase=0");
                if (Data1.Count() == 0)
                {
                    Data1 = await _paymentMethodTypeRepository.GetAllByQuery(@"select PaymentMethodType.* from PaymentMethodType join Country on PaymentMethodType.CountryId = Country.Id
                                        where PaymentMethodType.InAppPurchase=1");
                }

                return Data1;
            }

            var Data = await _paymentMethodTypeRepository.GetAllByQuery(@"select PaymentMethodType.* from PaymentMethodType join Country on PaymentMethodType.CountryId = Country.Id
                                        where PaymentMethodType.InAppPurchase=1");
            return Data;

        }
    }
}
