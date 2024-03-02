using Dapper;
using Dapper.Contrib.Extensions;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public  class TeacherTransactionRepository:GenericRepository<TeacherTransaction>
    {

        public async Task<TeacherTransaction> AddTransaction(TeacherTransaction transaction)
        {
            TeacherTransaction NewTransaction = null;
            try
            {
                transaction.CreationDate = DateTime.UtcNow;
                var id = await _connectionFactory.GetConnection.InsertAsync(transaction);
                if (id > 0)
                    NewTransaction = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<TeacherTransaction>("select * from [TeacherTransaction] where Id=" + id + "");
                return NewTransaction;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connectionFactory.Dispose();
            }

        }

        public async Task<TeacherTransactionVM> GetTransactions(long TeacherId, int Page,long CountryId)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                Page = Page * 30;
                TeacherTransactionVM teacherTransactionVM = new   TeacherTransactionVM();
                var TotalAmount = await conn.QueryFirstAsync<float>("Select ISNULL(SUM(Amount) ,0) from [TeacherTransaction] where TeacherId=" + TeacherId + " and CountryId=" + CountryId + " ");
                teacherTransactionVM.TotalBalance = TotalAmount;
                var listTransaction = await GetAll(" where TeacherId=" + TeacherId + " and CountryId="+ CountryId+ " ORDER BY Id DESC OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");
                teacherTransactionVM.TeacherTransactions = listTransaction;
                return teacherTransactionVM;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                _connectionFactory.Dispose();
            }


        }

    }
}
