using Dapper;
using Dapper.Contrib.Extensions;
using DataAccess.Entities;
using DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
  public  class StudentTransactionRepository:GenericRepository<StudentTransaction>
    {
        public async Task<StudentTransaction> AddTransaction(StudentTransaction transaction)
        {
            StudentTransaction NewTransaction = null;
            try
            {
                transaction.CreationDate = DateTime.UtcNow;
                var id = await _connectionFactory.GetConnection.InsertAsync(transaction);
                if (id > 0)
                {
                    NewTransaction = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<StudentTransaction>("select * from [StudentTransaction] where Id=" + id + "");
                }
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

        public async Task<StudentTransactionVM> GetTransactions(long StudentId, int Page)
        {
            try
            {
                var conn = _connectionFactory.GetConnection;
                Page = Page * 30;
                StudentTransactionVM studentTransactionVM  = new  StudentTransactionVM();
                var TotalAmount = await conn.QueryFirstAsync<float>("Select ISNULL(SUM(Amount) ,0) from [StudentTransaction] where StudentId=" + StudentId + " ");
                studentTransactionVM.TotalBalance = TotalAmount;
                var listTransaction = await GetAll(" where StudentId=" + StudentId + " ORDER BY Id DESC OFFSET " + Page + " Rows FETCH Next 30 Rows ONLY");
                studentTransactionVM.studentTransactions = listTransaction;
                return studentTransactionVM;

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

        public async Task<decimal> GetTotal(long StudentId)
        {
            var TotalAmount = await _connectionFactory.GetConnection.QueryFirstAsync<decimal>("Select ISNULL(SUM(Amount),0) from [StudentTransaction] where StudentId=" + StudentId + " ");

            return TotalAmount;
        }


        public async Task<StudentTransaction> CheckIsThisOperationFound(string PaymentId)
        {
            StudentTransaction NewTransaction = null;
            try
            {
                NewTransaction= await GetOneByQuery("select * from StudentTransaction where PaymentId='" + PaymentId + "'");
                return NewTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


 
        public async Task<bool> AddTransactionRecordsBulk(StudentTransaction studentTransactionToBuyCourse,
        StudentCourse studentCourse, StudentPromoCode studentPromoCode, PromoCode UpdatePromo,
        StudentNotification studentNotification, TeacherTransaction teacherTransaction, 
        SystemTransaction systemTransaction, TeacherNotification teacherNotification)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DTAppCon"].ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        await connection.InsertAsync(studentTransactionToBuyCourse, transaction: transaction);
                        await connection.InsertAsync(studentCourse, transaction: transaction);
                        if (studentPromoCode!=null)
                        {
                            await connection.InsertAsync(studentPromoCode, transaction: transaction);

                        }
                        if (UpdatePromo!=null)
                        {
                            await connection.UpdateAsync(UpdatePromo, transaction: transaction);
                        }
                        await connection.InsertAsync(studentNotification, transaction: transaction);
                        await connection.InsertAsync(teacherTransaction, transaction: transaction);
                        await connection.InsertAsync(systemTransaction, transaction: transaction);
                        await connection.InsertAsync(teacherNotification, transaction: transaction);

                        transaction.Commit();
                    }
                    connection.Close();
                    return true;
                }

            }
            catch (Exception e)
            {

                return false;
            }
        }

        public async Task<bool> AddTransactionRecordsBulk2(StudentTransaction studentTransactionToBuyCourse, StudentTransaction settlePaymentToBuyCourse,
      StudentCourse studentCourse, StudentPromoCode studentPromoCode, PromoCode UpdatePromo,
      StudentNotification studentNotification, TeacherTransaction teacherTransaction,
      SystemTransaction systemTransaction, TeacherNotification teacherNotification)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DTAppCon"].ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        if (studentTransactionToBuyCourse != null)
                        {
                            await connection.InsertAsync(studentTransactionToBuyCourse, transaction: transaction);

                        }

                        if (settlePaymentToBuyCourse != null)
                        {
                            await connection.InsertAsync(settlePaymentToBuyCourse, transaction: transaction);

                        }
                        await connection.InsertAsync(studentCourse, transaction: transaction);
                        if (studentPromoCode != null)
                        {
                            await connection.InsertAsync(studentPromoCode, transaction: transaction);

                        }
                        if (UpdatePromo != null)
                        {
                            await connection.UpdateAsync(UpdatePromo, transaction: transaction);
                        }
                        await connection.InsertAsync(studentNotification, transaction: transaction);
                        await connection.InsertAsync(teacherTransaction, transaction: transaction);
                        await connection.InsertAsync(systemTransaction, transaction: transaction);
                        await connection.InsertAsync(teacherNotification, transaction: transaction);

                        transaction.Commit();
                    }
                    connection.Close();
                    return true;
                }

            }
            catch (Exception e)
            {

                return false;
            }
        }
        public async Task<bool> AddTransactionRecordsForLiveBulk(StudentTransaction studentTransactionToBuyCourse,
        StudentLive studentLive, StudentPromoCode studentPromoCode, PromoCode UpdatePromo,
        StudentNotification studentNotification, TeacherTransaction teacherTransaction,
        SystemTransaction systemTransaction, TeacherNotification teacherNotification)
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DTAppCon"].ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        //await connection.InsertAsync(studentTransactionToBuyCourse, transaction: transaction);
                        await connection.InsertAsync(studentLive, transaction: transaction);
                        if (studentPromoCode != null)
                        {
                            await connection.InsertAsync(studentPromoCode, transaction: transaction);

                        }
                        if (UpdatePromo != null)
                        {
                            await connection.UpdateAsync(UpdatePromo, transaction: transaction);
                        }
                        await connection.InsertAsync(studentNotification, transaction: transaction);
                        await connection.InsertAsync(teacherTransaction, transaction: transaction);
                        await connection.InsertAsync(systemTransaction, transaction: transaction);
                        await connection.InsertAsync(teacherNotification, transaction: transaction);

                        transaction.Commit();
                    }
                    connection.Close();
                    return true;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<StudentTransaction> CheckIsThisTransactionFound(string cowpay_reference_id)
        {
            StudentTransaction NewTransaction = null;
            try
            {
                //NewTransaction = await GetOneByQuery("select  studentTransaction.* from [CowPayLog]  left join  StudentTransaction on [CowPayLog].cowpay_reference_id=  StudentTransaction.ReferenceNumber where order_status='PAID' and payment_gateway_reference_id ='" + cowpay_reference_id + "'");

                NewTransaction = await GetOneByQuery("select * from StudentTransaction where ReferenceNumber='" + cowpay_reference_id + "'");
                return NewTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal async Task<List<string>> CheckPaymentIds()
        {
            var sql = @"select  PaymentId from StudentTransaction where StudentId Is Null and CreationDate >=DATEADD(HOUR,-24, (select GETUTCDATE()))";
            var Ids = await _connectionFactory.GetConnection.QueryAsync<string>(sql);
            return Ids.ToList();
        }
    }
}
