using Dapper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class StudentDepartmentRepository:GenericRepository<StudentDepartment>
    {
        public async Task<bool> AddDepartmentToStudent(List<long> DepartmentIds,long StudentId)
        {
            try
            {
                var members = new List<StudentDepartment>();
                var DeleteOld = await _connectionFactory.GetConnection.ExecuteAsync(" Delete from StudentDepartment where StudentId=" + StudentId + " ");

                foreach (var departmentId in DepartmentIds)
                {
                        members.Add(
                                new StudentDepartment()
                                {
                                    DepartmentId = departmentId,
                                    StudentId = StudentId
                                });
                }
                if (members.Count()>0)
                {
                    string query = "INSERT INTO [dbo].[StudentDepartment]([DepartmentId],[StudentId]) VALUES ";
                    foreach (var item in members)
                    {
                        string Values = "(" + item.DepartmentId + "," + item.StudentId + "),";
                        query += Values;
                    }
                    query = query.Remove(query.Length - 1, 1);
                    var result = await _connectionFactory.GetConnection.ExecuteAsync(query);
                    if (result > 0)
                    {
                        return true;
                    }
                }
              
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
           

        }


    }
}
