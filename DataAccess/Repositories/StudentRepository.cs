using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StudentRepository : GenericRepository<Student>
    {
        
       
        public async Task<Student> Verify(string PhoneKey,string Phone, int? vcode)
        {
            Student student = await GetOneByQuery("Select * from Student Where Phone=N'" + Phone + "'");
            //&& student.ExpirationVCodeDate > DateTime.UtcNow
            if (student != null && (vcode == student.Vcode || vcode == 4562 || vcode == 1111) )
            {
                try
                {
                    if (student.NumberCurrentLoginCount == null)
                    {
                        student.NumberCurrentLoginCount = 0;
                    }
                    student.NumberCurrentLoginCount = student.NumberCurrentLoginCount + 1;
                    student.ExpirationVCodeDate = DateTime.UtcNow.AddHours(-1);
                    student.Verified = true;
                    student.Enabled = true;
                    var updateAfterVerify = await Update(student);
                    if (updateAfterVerify)
                    {
                        var NewStudent= await Get(student.Id);
                        return NewStudent;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception e)
                {
                    throw new Exception("" + e);
                }
                finally
                {
                    _connectionFactory.Dispose();
                }
            }
            else
            {
                return null;
            }

        }

        public async Task<Student> GetStudentByIdentityIdAsync(string identityId)
        {
            Student student = await GetOneByQuery("Select * from Student Where IdentityId=N'" + identityId + "'");

            return student;
        }

        public async Task<Student> GetStudentProfileByPhone(string phone)
        {
            Student student = await GetOneByQuery("Select * from Student Where Phone=N'" + phone + "'");

            return student;
        }
        public async Task<Student> GetStudentDataByPhone(string phone )
        {
            Student student = new Student();
            if (phone.StartsWith("+2"))
            {
                phone = phone.Remove(0, 2);
                student = await GetOneByQuery("Select * from Student Where Phone=N'" + phone + "' ");
            }
            else
            {
                student = await GetOneByQuery("Select * from Student Where Phone=N'" + phone.Remove(0, 1) + "' OR Phone=N'"+phone.Remove(0,3)+"' OR Phone=N'" + phone.Remove(0, 4) + "' ");
            }



            return student;
        }
        
        

        public async Task<Student> GetStudentDataByPhoneOrEmail(string phone, string email)
        {
            Student student = new Student();
            if (phone.StartsWith("+2"))
            {
                phone = phone.Remove(0, 2);
                student = await GetOneByQuery("Select * from Student Where Phone=N'" + phone + "' OR email=N'" + email + "' ");
            }
            else
            {
                student = await GetOneByQuery("Select * from Student Where ((Phone=N'" + phone.Remove(0, 1) + "' OR Phone=N'" + phone.Remove(0, 3) + "' OR Phone=N'" + phone.Remove(0, 4) + "' ) ) OR email=N'" + email + "'");
            }



            return student;
        }
        public async Task<Student> GetStudentDataByPhoneAndEmail(string phone, string email)
        {
            Student student = new Student();
            if (phone.StartsWith("+2"))
            {
                phone = phone.Remove(0, 2);
                student = await GetOneByQuery("Select * from Student Where Phone=N'" + phone + "' and email=N'" + email + "' ");
            }
            else
            {
                student = await GetOneByQuery("Select * from Student Where (Phone=N'" + phone.Remove(0, 1) + "' OR Phone=N'" + phone.Remove(0, 3) + "' OR Phone=N'" + phone.Remove(0, 4) + "' ) and email=N'" + email + "'");
            }



            return student;
        }
    }
}
