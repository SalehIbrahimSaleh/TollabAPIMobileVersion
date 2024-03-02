using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Services;
using DataAccess.UnitOfWork;

namespace DataAccess.Repositories.Tests
{
    [TestClass()]
    public class StudentTransactionRepositoryTests
    {
        TransactionsUnit _transactionUnit;
        StudentService _studentService;
        StudentUnit _studentUnit;
        public StudentTransactionRepositoryTests()
        {
            _transactionUnit = new TransactionsUnit();
            _studentService = new StudentService();
            _studentUnit = new StudentUnit();
        }

        [TestMethod()]
        public void CheckIsThisTransactionFoundTest()
        {
            double beginningBalance = 11.99;
            double debitAmount = -100.00;
            _transactionUnit.StudentTransactionRepository.CheckIsThisTransactionFound("1951629");
        }
    }
}