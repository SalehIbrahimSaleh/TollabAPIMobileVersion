﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
   public class StudentTransactionVM
    {
        public float? TotalBalance { get; set; }
        public IEnumerable<StudentTransaction> studentTransactions  { get; set; }

    }
}
