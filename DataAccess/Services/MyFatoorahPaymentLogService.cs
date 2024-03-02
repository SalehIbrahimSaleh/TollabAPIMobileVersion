using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class MyFatoorahPaymentLogService
    {
        GenericRepository<MyFatoorahPaymentLog> _repository;
        public MyFatoorahPaymentLogService()
        {
            _repository = new GenericRepository<MyFatoorahPaymentLog>();
        }

        public async Task<long> InsertInvoice(long invoiceId)
        {
            var result = await _repository.Add(new MyFatoorahPaymentLog
            {

                InvoiceId = invoiceId,
                IsChecked = false,
                IsResponded = false
            });
            return result;
        }

        public async Task<bool> UpdateInvoice(long invoiceId, bool isChecked, bool isResponded, string status)
        {
            var entity = await _repository.GetOneByQuery($" Select * from MyFatoorahPaymentLog Where InvoiceId={invoiceId}");
            if (entity != null)
            {
                entity.IsChecked = isChecked;
                entity.IsResponded = isResponded;
                entity.LastStatus = status;
                entity.LastChecked = DateTime.Now;
                return await _repository.Update(entity);
            }
            return false;
        }

        public async Task<IEnumerable<MyFatoorahPaymentLog>> GetAllUnresponded()
        {
            return await _repository.GetAllByQuery(" Select * from MyFatoorahPaymentLog Where IsResponded=0 OR IsResponded is Null");

        }
    }
}
