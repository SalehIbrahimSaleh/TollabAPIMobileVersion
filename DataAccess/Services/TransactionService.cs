using DataAccess.Entities;
using DataAccess.PaymentModels;
using DataAccess.UnitOfWork;
using DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class TransactionService
    {
        TransactionsUnit _transactionUnit;
        StudentService _studentService;
        StudentUnit  _studentUnit;
        public TransactionService()
        {
            _transactionUnit = new TransactionsUnit();
            _studentService = new StudentService();
            _studentUnit = new StudentUnit();
        }

        public async Task<PromoCode> CheckPromoCode(long StudentId, string PromocodeText)
        {
            var GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
            if (GetPromo == null)
            {
                return null;
            }
            if (GetPromo != null)
            {
                var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(StudentId, GetPromo.Id);
                if (checkIfStudentUsedThisPromoCodeBefore)
                {
                    return null;
                }
                var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(StudentId, GetPromo);
                if (checkIfStudentValidToThisPromocode == false)
                {
                    return null;
                }
                if (checkIfStudentValidToThisPromocode)
                {
                    return GetPromo;
                }
            }

            return null;
        }
        public async Task<int> BuyNow(long CourseId, Student student, string PromocodeText, long CountryId)
        {
            try
            {


                string promoNumber = null;
                PromoCode GetPromo = null;
                var course = await _transactionUnit.CourseRepository.Get(CourseId);
                var track = await _transactionUnit.TrackRepository.GetTrackByCourseId(CourseId);
                if (course == null)
                {
                    return 0;
                }
                if (track.BySubscription == true)
                {
                    return 5;
                }
                if (course.CurrentCost == null)
                {
                    return 6;
                }

                var teacher = await _transactionUnit.TeacherRepository.GetTeacherByCourseId(CourseId);

                var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisCourseBefore(CourseId, student.Id);
                if (IsEnrolledBefore)
                {
                    return 1;
                }

                //promocode
                if (!string.IsNullOrEmpty(PromocodeText))
                {
                    GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                    if (GetPromo == null)
                    {
                        return 2;
                    }
                    if (GetPromo != null)
                    {
                        var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                        if (checkIfStudentUsedThisPromoCodeBefore)
                        {
                            return 3;
                        }
                        var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                        if (checkIfStudentValidToThisPromocode == false)
                        {
                            return 8;
                        }
                        if (checkIfStudentValidToThisPromocode)
                        {
                           var dicountAmount = (course.CurrentCost * GetPromo.PromoCodeValue) / 100;
                           // var dicountAmount = GetPromo.PromoCodeValue;

                            course.CurrentCost = course.CurrentCost - dicountAmount;
                            promoNumber = GetPromo.PromoCodeText;
                        }
                        //StudentTransaction studentTransaction = new StudentTransaction { Amount = GetPromo.PromoCodeValue, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 2, Reason = "PromoCode Charging", ReferenceNumber = GetPromo.PromoCodeText};
                        //var AddTransactionWithPromocode = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransaction);

                    }
                }

                //check wallet
                var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(student.Id);
                if (GetTotalWallet < course.CurrentCost)
                {
                    //if (student.PaymentLink==null)
                    //{
                    //    var link = await _studentService.SendPaymentLink(student);

                    //}

                    //var days = DateTime.UtcNow.Subtract(student.LastSendDate.Value);
                    //if (days.Days>7)
                    //{
                    //    var link = await _studentService.SendPaymentLinkAgain(student);

                    //}

                    return 4;
                }

                //deduced from Student Wallet
                StudentTransaction studentTransactionToBuyCourse = new StudentTransaction
                { Amount = -course.CurrentCost, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Enrolling In Course (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = PaymentKeyGenerator.getNewKey(), PromocodeNumber = promoNumber };
                var AddTransactionWithCourseCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyCourse);

                if (AddTransactionWithCourseCost.Id > 0)
                {
                    StudentCourse studentCourse = new StudentCourse { StudentId = student.Id, CourseId = course.Id, EnrollmentDate = DateTime.UtcNow };
                    var addCourseToStudent = await _transactionUnit.StudentCourseRepository.Add(studentCourse);

                    //add promo to student
                    if (GetPromo != null)
                    {
                        StudentPromoCode studentPromoCode = new StudentPromoCode
                        {
                            PromoCodeId = GetPromo.Id,
                            StudentId = student.Id
                        };
                        await _transactionUnit.StudentPromoCodeRepository.Add(studentPromoCode);
                        GetPromo.UsedCount = GetPromo.UsedCount + 1;
                        await _transactionUnit.PromoCodeRepository.Update(GetPromo);
                    }
                    //add notification
                    StudentNotification studentNotification = new StudentNotification
                    {
                        CreationDate = DateTime.UtcNow,
                        CourseId = CourseId,
                        NotificationToId = CourseId,
                        ReferenceId = 1,
                        StudentId = student.Id,
                        Title = studentTransactionToBuyCourse.Reason,
                        TitleLT = studentTransactionToBuyCourse.ReasonLT
                    };
                    await _transactionUnit.StudentNotificationRepository.AddNotificationWithoutPush(studentNotification);
                    //
                    //Add to Teacher wallet
                    if (addCourseToStudent > 0)
                    {
                        var teacherAmount = (course.CurrentCost * teacher.TakenPercentage) / 100;
                        var systemAmount = course.CurrentCost - teacherAmount;
                        TeacherTransaction teacherTransaction = new TeacherTransaction
                        {
                            Amount = teacherAmount,
                            CreationDate = DateTime.UtcNow,
                            PaymentMethodId = 1,
                            Reason = "New subscription in course (" + course.Name + ") for student:- "+ student.Name + "",
                            ReasonLT = " تم اضافة اشتراك جديد في دورة(" + course.NameLT + ")" +"للطالب "+student.Name+"",
                            ReferenceNumber = PaymentKeyGenerator.getNewKey(),
                            TeacherId = teacher.Id,
                            CourseId = CourseId,
                            PromocodeNumber = promoNumber,
                            CountryId = CountryId
                        };
                        var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                        SystemTransaction systemTransaction = new SystemTransaction
                        {
                            Amount = systemAmount,
                            CreationDate = DateTime.UtcNow,
                            Reason = "New subscription in course number #" + CourseId + ""
                        };
                        await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                        if (addTransactionToTeacher.Id > 0)
                        {
                            //add notification
                            TeacherNotification teacherNotification = new TeacherNotification
                            {
                                CreationDate = DateTime.UtcNow,
                                NotificationToId = CourseId,
                                ReferenceId = 1,
                                TeacherId = teacher.Id,
                                Title = teacherTransaction.Reason,
                                TitleLT = teacherTransaction.ReasonLT
                            };
                            await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                        }
                    }
                }

                return 7;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> BuyNowV2(long CourseId, Student student, string PromocodeText, long CountryId)
        {
            try
            {

                var ReferenceNumber = PaymentKeyGenerator.getNewKey();
                string promoNumber = null;
                PromoCode GetPromo = null;
                var course = await _transactionUnit.CourseRepository.Get(CourseId);
                var track = await _transactionUnit.TrackRepository.GetTrackByCourseId(CourseId);
                if (course == null)
                {
                    return 0;
                }
                if (track.BySubscription == true)
                {
                    return 5;
                }
                if (course.CurrentCost == null)
                {
                    return 6;
                }

                var teacher = await _transactionUnit.TeacherRepository.GetTeacherByCourseId(CourseId);

                var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisCourseBefore(CourseId, student.Id);
                if (IsEnrolledBefore)
                {
                    return 1;
                }

                //promocode
                if (!string.IsNullOrEmpty(PromocodeText))
                {
                    GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                    if (GetPromo == null)
                    {
                        return 2;
                    }
                    if (GetPromo != null)
                    {
                        var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                        if (checkIfStudentUsedThisPromoCodeBefore)
                        {
                            return 3;
                        }
                        var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                        if (checkIfStudentValidToThisPromocode == false)
                        {
                            return 8;
                        }
                        if (checkIfStudentValidToThisPromocode)
                        {
                            var dicountAmount = (course.CurrentCost * GetPromo.PromoCodeValue) / 100;
                            //var dicountAmount = GetPromo.PromoCodeValue;
                            course.CurrentCost = course.CurrentCost - dicountAmount;
                            promoNumber = GetPromo.PromoCodeText;
                        }
                    }
                }

                //check wallet
                var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(student.Id);
                if (GetTotalWallet < course.CurrentCost)
                {
                    return 4;
                }
                StudentPromoCode studentPromoCode = null;
                //deduced from Student Wallet
                //01
                StudentTransaction studentTransactionToBuyCourse = new StudentTransaction { Amount = -course.CurrentCost, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Enrolling In Course (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };
                //02
                StudentCourse studentCourse = new StudentCourse { StudentId = student.Id, ReferenceNumber= ReferenceNumber, CourseId = course.Id, EnrollmentDate = DateTime.UtcNow };
                //add promo to student
                if (GetPromo != null)
                {
                    //03
                    studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = student.Id
                    };

                    //04 update
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                }
                //05 add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    CourseId = CourseId,
                    NotificationToId = CourseId,
                    ReferenceId = 1,
                    StudentId = student.Id,
                    Title = studentTransactionToBuyCourse.Reason,
                    TitleLT = studentTransactionToBuyCourse.ReasonLT
                };
                //06 Add to Teacher wallet
                var teacherAmount = (course.CurrentCost * teacher.TakenPercentage) / 100;
                var systemAmount = course.CurrentCost - teacherAmount;
                TeacherTransaction teacherTransaction = new TeacherTransaction
                {
                    Amount = teacherAmount,
                    CreationDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    Reason = "New subscription in course (" + course.Name + ") for student:- " + student.Name + "",
                    ReasonLT = " تم اضافة اشتراك جديد في دورة(" + course.NameLT + ")" + "للطالب " + student.Name + "",
                    ReferenceNumber = ReferenceNumber,
                    TeacherId = teacher.Id,
                    CourseId = CourseId,
                    PromocodeNumber = promoNumber,
                    CountryId = CountryId
                };
                //07 add to system
                SystemTransaction systemTransaction = new SystemTransaction
                {
                    ReferenceNumber= ReferenceNumber,
                    Amount = systemAmount,
                    CreationDate = DateTime.UtcNow,
                    Reason = "New subscription in course number #" + CourseId + ""
                };
                //08 add notification
                TeacherNotification teacherNotification = new TeacherNotification
                {
                    CreationDate = DateTime.UtcNow,
                    NotificationToId = CourseId,
                    ReferenceId = 1,
                    TeacherId = teacher.Id,
                    Title = teacherTransaction.Reason,
                    TitleLT = teacherTransaction.ReasonLT
                };
                var insertall=  await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsBulk(studentTransactionToBuyCourse,
                    studentCourse, studentPromoCode, GetPromo/*update*/, studentNotification, teacherTransaction,
                    systemTransaction, teacherNotification);
                if (insertall)
                {
                    return 7;
                }

                return 10;
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<int> BuyLive(long liveId, Student student, string PromocodeText, long CountryId)
        {
            try
            {

                var ReferenceNumber = PaymentKeyGenerator.getNewKey();
                string promoNumber = null;
                PromoCode GetPromo = null;
                var live = await _studentUnit.LiveRepository.Get(liveId);
                if (live == null)
                {
                    return 0;
                }

                var teacher = await _transactionUnit.TeacherRepository.GetTeacherByLiveId(liveId);

                var IsEnrolledBefore = await _transactionUnit.StudentLiveRepository.CheckIfStudentEnrolledInThisLiveBefore(liveId, student.Id);
                if (IsEnrolledBefore)
                {
                    return 1;
                }

                //promocode
                if (!string.IsNullOrEmpty(PromocodeText))
                {
                    GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                    if (GetPromo == null)
                    {
                        return 2;
                    }
                    if (GetPromo != null)
                    {
                        var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                        if (checkIfStudentUsedThisPromoCodeBefore)
                        {
                            return 3;
                        }
                        var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                        if (checkIfStudentValidToThisPromocode == false)
                        {
                            return 8;
                        }
                        if (checkIfStudentValidToThisPromocode)
                        {
                            var dicountAmount = (live.CurrentPrice * GetPromo.PromoCodeValue) / 100;
                            //var dicountAmount = GetPromo.PromoCodeValue;
                            live.CurrentPrice = live.CurrentPrice - dicountAmount.Value;
                            promoNumber = GetPromo.PromoCodeText;
                        }
                    }
                }

                //check wallet
                var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(student.Id);
                if (GetTotalWallet < live.CurrentPrice)
                {
                    return 4;
                }
                StudentPromoCode studentPromoCode = null;
                //deduced from Student Wallet
                //01
                StudentTransaction studentTransactionToBuyCourse = new StudentTransaction { Amount = -live.CurrentPrice, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Enrolling In Live (" + live.LiveName + ")", ReasonLT = "الإشتراك في بث مباشر (" + live.LiveName + ")", LiveId = liveId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };
                //02
                StudentLive studentLive = new StudentLive { StudentId = student.Id, ReferenceNumber = ReferenceNumber, LiveId = live.Id, EnrollmentDate = DateTime.UtcNow };
                //add promo to student
                if (GetPromo != null)
                {
                    //03
                    studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = student.Id
                    };

                    //04 update
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                }
                //05 add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    LiveId = liveId,
                    NotificationToId = liveId,
                    ReferenceId = 6,
                    StudentId = student.Id,
                    Title = studentTransactionToBuyCourse.Reason,
                    TitleLT = studentTransactionToBuyCourse.ReasonLT
                };
                //06 Add to Teacher wallet
                var teacherAmount = (live.CurrentPrice * teacher.LiveTakenPercentage) / 100;
                var systemAmount = live.CurrentPrice - teacherAmount;
                TeacherTransaction teacherTransaction = new TeacherTransaction
                {
                    Amount = teacherAmount,
                    CreationDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    Reason = "New subscription in live (" + live.LiveName + ") for student:- " + student.Name + "",
                    ReasonLT = " تم اضافة اشتراك جديد في بث مباشر(" + live.LiveName + ")" + "للطالب " + student.Name + "",
                    ReferenceNumber = ReferenceNumber,
                    TeacherId = teacher.Id,
                    LiveId = liveId,
                    PromocodeNumber = promoNumber,
                    CountryId = CountryId
                };
                //07 add to system
                SystemTransaction systemTransaction = new SystemTransaction
                {
                    ReferenceNumber = ReferenceNumber,
                    Amount = systemAmount,
                    CreationDate = DateTime.UtcNow,
                    Reason = "New subscription in live number #" + liveId + ""
                };
                //08 add notification
                TeacherNotification teacherNotification = new TeacherNotification
                {
                    CreationDate = DateTime.UtcNow,
                    NotificationToId = liveId,
                    ReferenceId = 6,
                    TeacherId = teacher.Id,
                    Title = teacherTransaction.Reason,
                    TitleLT = teacherTransaction.ReasonLT
                };
                var insertall = await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsForLiveBulk(studentTransactionToBuyCourse,
                    studentLive, studentPromoCode, GetPromo/*update*/, studentNotification, teacherTransaction,
                    systemTransaction, teacherNotification);
                if (insertall)
                {
                    return 7;
                }

                return 10;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> BuyNowByCourseCode(string CourseCode, Student student, string PromocodeText)
        {
            string promoNumber = null;
            PromoCode GetPromo = null;
            var course = await _transactionUnit.CourseRepository.GetWhere(" Where CourseCode='"+ CourseCode + "' ");
            var track = await _transactionUnit.TrackRepository.GetTrackByCourseId(course.Id);
            if (course == null)
            {
                return 0;
            }
            if (track.BySubscription == true)
            {
                return 5;
            }
            if (course.CurrentCost == null)
            {
                return 6;
            }

            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByCourseId(course.Id);

            var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisCourseBefore(course.Id, student.Id);
            if (IsEnrolledBefore)
            {
                return 1;
            }

            //promocode
            if (!string.IsNullOrEmpty(PromocodeText))
            {
                GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                if (GetPromo == null)
                {
                    return 2;
                }
                if (GetPromo != null)
                {
                    var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                    if (checkIfStudentUsedThisPromoCodeBefore)
                    {
                        return 3;
                    }
                    var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                    if (checkIfStudentValidToThisPromocode == false)
                    {
                        return 8;
                    }
                    if (checkIfStudentValidToThisPromocode)
                    {
                       var dicountAmount = (course.CurrentCost * GetPromo.PromoCodeValue) / 100;
                        //var dicountAmount = GetPromo.PromoCodeValue;

                        course.CurrentCost = course.CurrentCost - dicountAmount;
                        promoNumber = GetPromo.PromoCodeText;
                    }
                }
            }

            //check wallet
            var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(student.Id);
            if (GetTotalWallet < course.CurrentCost)
            {
                //if (student.PaymentLink == null)
                //{
                //    var link = await _studentService.SendPaymentLink(student);

                //}
                //var days = DateTime.UtcNow.Subtract(student.LastSendDate.Value);
                //if (days.Days > 7)
                //{
                //    var link = await _studentService.SendPaymentLinkAgain(student);

                //}

                return 4;
            }

            //deduced from Student Wallet
            StudentTransaction studentTransactionToBuyCourse = new StudentTransaction
            { Amount = -course.CurrentCost, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Enrolling In Course (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = course.Id, ReferenceNumber = PaymentKeyGenerator.getNewKey(), PromocodeNumber = promoNumber };
            var AddTransactionWithCourseCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyCourse);

            if (AddTransactionWithCourseCost.Id > 0)
            {
                StudentCourse studentCourse = new StudentCourse { StudentId = student.Id, CourseId = course.Id, EnrollmentDate = DateTime.UtcNow };
                var addCourseToStudent = await _transactionUnit.StudentCourseRepository.Add(studentCourse);

                //add promo to student
                if (GetPromo != null)
                {
                    StudentPromoCode studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = student.Id
                    };
                    await _transactionUnit.StudentPromoCodeRepository.Add(studentPromoCode);
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                    await _transactionUnit.PromoCodeRepository.Update(GetPromo);
                }
                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    CourseId = course.Id,
                    NotificationToId = course.Id,
                    ReferenceId = 1,
                    StudentId = student.Id,
                    Title = studentTransactionToBuyCourse.Reason,
                    TitleLT = studentTransactionToBuyCourse.ReasonLT
                };
                await _transactionUnit.StudentNotificationRepository.AddNotificationWithoutPush(studentNotification);
                //
                //Add to Teacher wallet
                if (addCourseToStudent > 0)
                {
                    var teacherAmount = (course.CurrentCost * teacher.TakenPercentage) / 100;
                    var systemAmount = course.CurrentCost - teacherAmount;
                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 1,
                        Reason = "New subscription in course (" + course.Name + ") for student:- " + student.Name + "",
                        ReasonLT = " تم اضافة اشتراك جديد في دورة(" + course.NameLT + ")" + "للطالب " + student.Name + "",
                        ReferenceNumber = PaymentKeyGenerator.getNewKey(),
                        TeacherId = teacher.Id,
                        CourseId = course.Id,
                        PromocodeNumber = promoNumber,
                        CountryId=student.CountryId
                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription in course number #" + course.Id + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {
                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            NotificationToId = course.Id,
                            ReferenceId = 1,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                }
            }

            return 7;
        }

        public async Task<int> TrackSubscription(long TrackId, long StudentId, string PromocodeText, long CountryId)
        {
            var StudentData =await _studentUnit.StudentRepository.Get(StudentId);
            var ReferenceNumber = PaymentKeyGenerator.getNewKey();
            string promoNumber = null;
            PromoCode GetPromo = null;
            var track = await _transactionUnit.TrackRepository.Get(TrackId);
           if (track == null)
            {
                return 0;
            }
            if (track.BySubscription == false)
            {
                return 5;
            }
            if (track.SubscriptionDuration==null||track.SubscriptionCurrentPrice==null ||track.SubscriptionDuration==null)
            {
                return 6;
            }
            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByTrackId(TrackId);

            var IsEnrolledBefore = await _transactionUnit.TrackSubscriptionRepository.CheckIfStudentSubscripeThistrackBefore(track.Id, StudentId);
            if (IsEnrolledBefore != null && IsEnrolledBefore.DurationExpiration.Value.Date > DateTime.UtcNow.Date)
            {
                return 1;
            }


            if (!string.IsNullOrEmpty(PromocodeText))
            {
                 GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                if (GetPromo == null)
                {
                    return 2;
                }
                if (GetPromo != null)
                {
                    var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(StudentId, GetPromo.Id);
                    if (checkIfStudentUsedThisPromoCodeBefore)
                    {
                        return 3;
                    }
                    var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(StudentId, GetPromo);
                    if (checkIfStudentValidToThisPromocode==false)
                    {
                        return 8;
                    }
                    if (checkIfStudentValidToThisPromocode)
                    {
                        var dicountAmount = (track.SubscriptionCurrentPrice * GetPromo.PromoCodeValue) / 100;
                        //var dicountAmount = GetPromo.PromoCodeValue;

                        track.SubscriptionCurrentPrice = track.SubscriptionCurrentPrice - dicountAmount;
                        promoNumber = GetPromo.PromoCodeText;
                    }

                }


            }

            var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(StudentId);
            if (GetTotalWallet < track.SubscriptionCurrentPrice)
            {
                return 4;
            }
            //deduced from Student Wallet
            StudentTransaction studentTransactionToBuyTrack = new StudentTransaction
            { Amount = -track.SubscriptionCurrentPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Subscription In Track ("+track.Name+")",ReasonLT= "الاشتراك في مادة (" + track.NameLT + ")", TrackId = TrackId, ReferenceNumber = ReferenceNumber, PromocodeNumber=promoNumber };
            var AddTransactionWithTrackCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyTrack);
            if (AddTransactionWithTrackCost.Id > 0)
            {
                long addOrUpdateCourseToStudent = 0;
                if (IsEnrolledBefore == null)
                {
                    TrackSubscription trackSubscription = new TrackSubscription
                    {
                        StudentId = StudentId,
                        TrackId = track.Id,
                        CreationDate = DateTime.UtcNow,
                        DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value)
                    };
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.AddTrackToStudent(trackSubscription);
                }
                else
                {
                    IsEnrolledBefore.TrackId = track.Id;
                    IsEnrolledBefore.StudentId = StudentId;
                    IsEnrolledBefore.CreationDate = DateTime.UtcNow;
                    IsEnrolledBefore.DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value);
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.UpdateTrackToStudent(IsEnrolledBefore);
                }

                //add promo to student
                if (GetPromo != null)
                {
                    StudentPromoCode studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = StudentId
                    };
                    await _transactionUnit.StudentPromoCodeRepository.Add(studentPromoCode);
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                    await _transactionUnit.PromoCodeRepository.Update(GetPromo);

                }

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 5,
                    StudentId = StudentId,
                    Title = studentTransactionToBuyTrack.Reason,
                    TitleLT= studentTransactionToBuyTrack.ReasonLT
                };
                await _transactionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                //
                //Add to Teacher wallet
                if (addOrUpdateCourseToStudent > 0)
                {
                    var teacherAmount = (track.SubscriptionCurrentPrice * teacher.TakenPercentage) / 100;
                    var systemAmount = track.SubscriptionCurrentPrice - teacherAmount;

                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 1,
                        Reason = "New subscription in track (" + track.Name + ")" +" for student: "+ StudentData.Name+ "",
                        ReasonLT = "تم اضافة اشتراك جديد في مادة (" + track.NameLT + ")" +"للطالب : "+StudentData.Name+"",
                        ReferenceNumber = ReferenceNumber,
                        TeacherId = teacher.Id,
                        TrackId = track.Id,
                        PromocodeNumber=promoNumber,
                        CountryId=CountryId
                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        ReferenceNumber= ReferenceNumber,
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription in Track number #" + track.Id + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {
                    

                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            ReferenceId = 5,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                }
            }

            return 7;
        }

        public async Task<int> TrackSubscriptionByTrackCode(string TrackCode, long StudentId, string PromocodeText, long CountryId)
        {
            var StudentData = await _studentUnit.StudentRepository.Get(StudentId);
            string promoNumber = null;
            PromoCode GetPromo = null;
            var track = await _transactionUnit.TrackRepository.GetWhere(" where TrackCode='"+ TrackCode + "' ");
            if (track == null)
            {
                return 0;
            }
            if (track.BySubscription == false)
            {
                return 5;
            }
            if (track.SubscriptionDuration == null || track.SubscriptionCurrentPrice == null || track.SubscriptionDuration == null)
            {
                return 6;
            }
            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByTrackId(track.Id);

            var IsEnrolledBefore = await _transactionUnit.TrackSubscriptionRepository.CheckIfStudentSubscripeThistrackBefore(track.Id, StudentId);
            if (IsEnrolledBefore!=null&& IsEnrolledBefore.DurationExpiration.Value.Date>DateTime.UtcNow.Date)
            {
                return 1;
            }

            if (!string.IsNullOrEmpty(PromocodeText))
            {
                GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                if (GetPromo == null)
                {
                    return 2;
                }
                if (GetPromo != null)
                {
                    var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(StudentId, GetPromo.Id);
                    if (checkIfStudentUsedThisPromoCodeBefore)
                    {
                        return 3;
                    }
                    var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(StudentId, GetPromo);
                    if (checkIfStudentValidToThisPromocode == false)
                    {
                        return 8;
                    }
                    if (checkIfStudentValidToThisPromocode)
                    {
                         var dicountAmount = (track.SubscriptionCurrentPrice * GetPromo.PromoCodeValue) / 100;
                        //var dicountAmount = GetPromo.PromoCodeValue;
                        track.SubscriptionCurrentPrice = track.SubscriptionCurrentPrice - dicountAmount;
                        promoNumber = GetPromo.PromoCodeText;
                    }

                }


            }

            var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(StudentId);
            if (GetTotalWallet < track.SubscriptionCurrentPrice)
            {
                return 4;
            }
            //deduced from Student Wallet
            StudentTransaction studentTransactionToBuyTrack = new StudentTransaction
            { Amount = -track.SubscriptionCurrentPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Subscription In Track (" + track.Name + ")", ReasonLT = "الاشتراك في مادة (" + track.NameLT + ")", TrackId = track.Id, ReferenceNumber = PaymentKeyGenerator.getNewKey(), PromocodeNumber = promoNumber };
            var AddTransactionWithTrackCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyTrack);
            if (AddTransactionWithTrackCost.Id > 0)
            {
                long addOrUpdateCourseToStudent = 0;
                if (IsEnrolledBefore==null)
                {
                    TrackSubscription trackSubscription = new TrackSubscription
                    { StudentId = StudentId,
                      TrackId = track.Id,
                      CreationDate = DateTime.UtcNow,
                      DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value) };
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.AddTrackToStudent(trackSubscription);
                }
                else
                {
                    IsEnrolledBefore.TrackId = track.Id;
                    IsEnrolledBefore.StudentId = StudentId;
                    IsEnrolledBefore.CreationDate = DateTime.UtcNow;
                    IsEnrolledBefore.DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value);
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.UpdateTrackToStudent(IsEnrolledBefore);
                }
                //add promo to student
                if (GetPromo != null)
                {
                    StudentPromoCode studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = StudentId
                    };
                    await _transactionUnit.StudentPromoCodeRepository.Add(studentPromoCode);
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                    await _transactionUnit.PromoCodeRepository.Update(GetPromo);

                }

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 5,
                    StudentId = StudentId,
                    Title = studentTransactionToBuyTrack.Reason,
                    TitleLT = studentTransactionToBuyTrack.ReasonLT
                };
                await _transactionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                //
                //Add to Teacher wallet
                if (addOrUpdateCourseToStudent > 0)
                {
                    var teacherAmount = (track.SubscriptionCurrentPrice * teacher.TakenPercentage) / 100;
                    var systemAmount = track.SubscriptionCurrentPrice - teacherAmount;

                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 1,
                        Reason = "New subscription in track (" + track.Name + ")" + " for student: " + StudentData.Name + "",
                        ReasonLT = "تم اضافة اشتراك جديد في مادة (" + track.NameLT + ")" + "للطالب : " + StudentData.Name + "",
                        ReferenceNumber = PaymentKeyGenerator.getNewKey(),
                        TeacherId = teacher.Id,
                        TrackId = track.Id,
                        PromocodeNumber = promoNumber,
                        CountryId = CountryId

                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription in Track number #" + track.Id + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {


                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            ReferenceId = 5,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                }
            }

            return 7;
        }

        public async Task<StudentTransactionVM> GetTransactions(long StudentId, int Page)
        {
            var result = await _transactionUnit.StudentTransactionRepository.GetTransactions(StudentId, Page);
            return result;
        }

        public async Task<StudentTransaction> PaymentSuccess(string paymentId)
        {
            StudentTransaction studentTransaction = new StudentTransaction()
            {
                CreationDate = DateTime.UtcNow,
                PaymentId = paymentId,
                Reason = "Charging by KNET",
                ReasonLT = " شحن عن طريق بطاقة كي نت",


            };
           var InsertedTransaction = await _transactionUnit.StudentTransactionRepository.CheckIsThisOperationFound(paymentId);
           if (InsertedTransaction == null)
            {
                var id = await _transactionUnit.StudentTransactionRepository.Add(studentTransaction);
                if (id > 0)
                {
                    return studentTransaction;
                }
                else
                    return null;

            }
           return InsertedTransaction;
        }

        public async Task<IEnumerable< StudentTransaction>> GetALLPendingOperations()
        {
            var result = await _transactionUnit.StudentTransactionRepository.GetAll(" where StudentId IS  NULL And  PaymentId Is Not NULL");
            return result;
        }

        public async Task<StudentTransaction> GetTransactionyPaymentId(string PaymentId)
        {
            var result = await _transactionUnit.StudentTransactionRepository.GetAll(" where  PaymentId ='"+ PaymentId +"'");
            if (result.Count()>0)
            {
                return result.FirstOrDefault();
            }
            return null;
        }


        public async Task<BankResponse> LogResponseToDataBase(BankResponse bankResponse)
        {
            BankResponse bankResponseHeader = new BankResponse()
            {
                IsSuccess = bankResponse.IsSuccess,
                Message = bankResponse.Message,
                ValidationErrors = bankResponse.ValidationErrors
            };
            var bankResponseHeaderId = await _transactionUnit.BankResponseRepository.Add(bankResponseHeader);
            if (bankResponseHeaderId>0 &&bankResponse.Data!=null)
            {
                var data = bankResponse.Data;
                data.BankResponseId = bankResponseHeaderId;
                var dataId = await _transactionUnit.DataRepository.Add(data);
                if (dataId>0 && bankResponse.Data.InvoiceTransactions.Count()>0)
                {

                    foreach (var item in bankResponse.Data.InvoiceTransactions)
                    {
                        item.DataId = dataId;
                        await _transactionUnit.InvoiceTransactionRepository.Add(item);
                    }

                    var InvoiceTransaction = bankResponse.Data.InvoiceTransactions.Where(i => i.TransactionStatus == "Succss").FirstOrDefault();

                    if (InvoiceTransaction!=null)
                    {
                        var result = await _transactionUnit.StudentTransactionRepository.GetAll(" where PaymentId ='"+ InvoiceTransaction.PaymentId + "' or PaymentId ='" + bankResponse.Data.InvoiceId + "'");
                        var transaction = result.FirstOrDefault();
                        if (transaction!=null)
                        {
                            decimal TheFactor = 1;
                            var studentData = await _studentUnit.StudentRepository.Get(long.Parse(bankResponse.Data.CustomerName));
                            if (studentData!=null)
                            {
                                var paymentMethodType = (await _transactionUnit.PaymentMethodTypeRepository.GetAll(" where CountryId=" + studentData.CountryId + " and Id!=2 ")).FirstOrDefault();
                                if (paymentMethodType!=null)
                                {
                                    TheFactor = (decimal)paymentMethodType.TheFactor.Value;
                                }
                            }
                            transaction.Amount = decimal.Parse(InvoiceTransaction.TransationValue)/TheFactor;
                            transaction.StudentId =long.Parse( bankResponse.Data.CustomerName);
                            transaction.ReferenceNumber = InvoiceTransaction.ReferenceId;
                            transaction.PaymentMethodId = 1;
                            await _transactionUnit.StudentTransactionRepository.Update(transaction);
                            StudentNotification studentNotification = new StudentNotification()
                            {
                                CreationDate = DateTime.UtcNow,
                                NotificationToId = transaction.StudentId,
                                NotificationTypeId = 2,
                                ReferenceId = 5,
                                StudentId = transaction.StudentId,
                                Title = transaction.Reason,
                                TitleLT = transaction.ReasonLT
                            };
                           await _studentUnit.StudentNotificationRepository.AddNotification(studentNotification);
                        }

                    }
                }
            }
            return bankResponse;
        }
        //public async Task<TeacherTransaction> paymentError(string paymentId, long? customerId)
        //{


        //}


        public async Task<int> BuyCourseByApplePurchasing(long CourseId, Student student)
        {
            var paymentKey = PaymentKeyGenerator.getNewKey();
            var course = await _transactionUnit.CourseRepository.Get(CourseId);
            var track = await _transactionUnit.TrackRepository.GetTrackByCourseId(CourseId);
            if (course == null)
            {
                return 0;
            }
            if (track.BySubscription == true)
            {
                return 5;
            }
            if (course.CurrentCost == null)
            {
                return 6;
            }

            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByCourseId(CourseId);

            var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisCourseBefore(CourseId, student.Id);
            if (IsEnrolledBefore)
            {
                return 1;
            }

            StudentTransaction studentTransactionToBuyCourse = new StudentTransaction
            { Amount = course.CurrentCost < 0 ? course.CurrentCost : course.CurrentCost, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Enrolling In Course by apple (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = paymentKey };

            var settlePaymentToBuyCourse = new StudentTransaction
            { Amount = course.CurrentCost < 0 ? course.CurrentCost : course.CurrentCost * -1, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Settle paymentTo Buy Course by apple (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = paymentKey };


            //deduced from Student Wallet because it deduct from app purchse   so we put it 0 from wallet
            //StudentTransaction studentTransactionToBuyCourse = new StudentTransaction
            //{ Amount = 0, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Enrolling In Course by apple (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = paymentKey };
            // var AddTransactionWithCourseCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyCourse);

            //if (AddTransactionWithCourseCost.Id > 0)
            // {
            StudentCourse studentCourse = new StudentCourse { StudentId = student.Id, ReferenceNumber = paymentKey, CourseId = course.Id, EnrollmentDate = DateTime.UtcNow };
            // var addCourseToStudent = await _transactionUnit.StudentCourseRepository.Add(studentCourse);

            //add notification
            StudentNotification studentNotification = new StudentNotification
            {
                CreationDate = DateTime.UtcNow,
                CourseId = CourseId,
                NotificationToId = CourseId,
                ReferenceId = 1,
                StudentId = student.Id,
                Title = studentTransactionToBuyCourse.Reason,
                TitleLT = studentTransactionToBuyCourse.ReasonLT
            };
            //await _transactionUnit.StudentNotificationRepository.AddNotificationWithoutPush(studentNotification);
            //
            //Add to Teacher wallet
            // if (addCourseToStudent > 0)
            //{
            var CourseCostAfterRemove30OverCostOfApple = course.SKUPrice * 70 / 100;
            var teacherAmount = (CourseCostAfterRemove30OverCostOfApple * teacher.TakenPercentage) / 100;
            var systemAmount = CourseCostAfterRemove30OverCostOfApple - teacherAmount;
            TeacherTransaction teacherTransaction = new TeacherTransaction
            {
                Amount = teacherAmount,
                CreationDate = DateTime.UtcNow,
                PaymentMethodId = 3,//INP
                Reason = "New subscription by apple in course (" + course.Name + ") for student:- " + student.Name + "",
                ReasonLT = " تم اضافة اشتراك جديد في دورة(" + course.NameLT + ")" + "للطالب " + student.Name + "",
                ReferenceNumber = paymentKey,
                TeacherId = teacher.Id,
                CourseId = CourseId,
                CountryId = student.CountryId
            };
            //   var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
            SystemTransaction systemTransaction = new SystemTransaction
            {
                Amount = systemAmount,
                CreationDate = DateTime.UtcNow,
                Reason = "New subscription by apple in course number #" + CourseId + ""
            };
            //  await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
            // if (addTransactionToTeacher.Id > 0)
            // {
            //add notification
            TeacherNotification teacherNotification = new TeacherNotification
            {
                CreationDate = DateTime.UtcNow,
                NotificationToId = CourseId,
                ReferenceId = 1,
                TeacherId = teacher.Id,
                Title = teacherTransaction.Reason,
                TitleLT = teacherTransaction.ReasonLT
            };
            // await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

            //  }
            //}
            //  }
            StudentPromoCode studentPromoCode = null;
            PromoCode GetPromo = null;
            var insertall = await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsBulk2(studentTransactionToBuyCourse, settlePaymentToBuyCourse,
                     studentCourse, studentPromoCode, GetPromo/*update*/, studentNotification, teacherTransaction,
                     systemTransaction, teacherNotification);
            if (insertall)
            {
                return 7;
            }

            return 10;
        }

        public async Task<int> PackageSubscription(long PackageId, long StudentId, string PromocodeText, long CountryId)
        {
            var paymentKey = PaymentKeyGenerator.getNewKey();
        
            var package = await _studentUnit.OfflinePackageRepository.Get(PackageId);

            if (package == null)
            {
                return 0;
            }

            if (package.NewPrice == null)
            {
                return 6;
            }


            var IsEnrolledBefore = await _transactionUnit.StudentPackageRepository.CheckIfStudentEnrolledInThisPackageBefore(PackageId, StudentId);
            if (IsEnrolledBefore)
            {
                return 1;
            }

            StudentTransaction studentTransactionToBuyPackage = new StudentTransaction
            { Amount = package.NewPrice < 0 ? package.NewPrice : package.NewPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Enrolling In Package by apple (" + package.Name + ")", ReasonLT = "الإشتراك في دورة (" + package.Name + ")", OfflinePackageId = PackageId, ReferenceNumber = paymentKey };

            await _transactionUnit.StudentTransactionRepository.Add(studentTransactionToBuyPackage);


            StudentPackage studentPackage = new StudentPackage
            {
                StudentId = StudentId,
                PackageId = PackageId,
                EnrollmentDate = DateTime.UtcNow
            };
            await _transactionUnit.StudentPackageRepository.Add(studentPackage);

           
            SystemTransaction systemTransaction = new SystemTransaction
            {
                Amount = package.NewPrice,
                CreationDate = DateTime.UtcNow,
                Reason = "New subscription by apple in Package number #" + PackageId + ""
            };
            await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);

            return 7;
            

        }

        public async Task<int> PackageSubscriptionByApplePurchasing(long PackageId, long StudentId,  long CountryId)
        {
            var paymentKey = PaymentKeyGenerator.getNewKey();
            var package = await _studentUnit.OfflinePackageRepository.Get(PackageId);
             if (package == null)
            {
                return 0;
            }
           
            if (package.NewPrice == null)
            {
                return 6;
            }

 
            var IsEnrolledBefore = await _transactionUnit.StudentPackageRepository.CheckIfStudentEnrolledInThisPackageBefore(PackageId, StudentId);
            if (IsEnrolledBefore)
            {
                return 1;
            }

            StudentTransaction studentTransactionToBuyPackage = new StudentTransaction
            { Amount = package.NewPrice < 0 ? package.NewPrice : package.NewPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Enrolling In Package by apple (" + package.Name + ")", ReasonLT = "الإشتراك في دورة (" + package.Name + ")", OfflinePackageId = PackageId, ReferenceNumber = paymentKey };

            await _transactionUnit.StudentTransactionRepository.Add(studentTransactionToBuyPackage);


            StudentPackage studentPackage = new StudentPackage
            {
                StudentId = StudentId,
                PackageId = PackageId,
                EnrollmentDate = DateTime.UtcNow
            };
            await _transactionUnit.StudentPackageRepository.Add(studentPackage);


            SystemTransaction systemTransaction = new SystemTransaction
            {
                Amount = package.NewPrice,
                CreationDate = DateTime.UtcNow,
                Reason = "New subscription by apple in Package number #" + PackageId + ""
            };
            await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);

            return 7;
        }

        public async Task<int> BuyLiveByApplePurchasing(long LiveId, Student student)
        {
            var paymentKey = PaymentKeyGenerator.getNewKey();
            var live = await _studentUnit.LiveRepository.Get(LiveId);
            if (live == null)
            {
                return 0;
            }
           

            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByLiveId(LiveId);

            var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisLiveBefore(LiveId, student.Id);
            if (IsEnrolledBefore)
            {
                return 1;
            }
            //deduced from Student Wallet because it deduct from app purchse   so we put it 0 from wallet
            
            StudentTransaction studentTransactionToBuyLive = new StudentTransaction { Amount = live.CurrentPrice < 0 ? live.CurrentPrice : live.CurrentPrice, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Direct paymentTo Buy Live by apple (" + live.LiveName + ")", ReasonLT = "الإشتراك في دورة (" + live.LiveName + ")", LiveId = LiveId, ReferenceNumber = paymentKey};

            var settlePaymentToBuyCourse = new StudentTransaction { Amount = live.CurrentPrice < 0 ? live.CurrentPrice : live.CurrentPrice * -1, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Settle paymentTo Buy Live by apple (" + live.LiveName + ")", ReasonLT = "الإشتراك في دورة (" + live.LiveName + ")", LiveId = LiveId, ReferenceNumber = paymentKey};

            //var AddTransactionWithCourseCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyCourse);

            //if (AddTransactionWithCourseCost.Id > 0)
            //{
                StudentLive studentLive = new StudentLive { StudentId = student.Id, ReferenceNumber = paymentKey, LiveId = live.Id, EnrollmentDate = DateTime.UtcNow };
              //  var addLiveToStudent = await _transactionUnit.StudentLiveRepository.Add(studentLive);

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    LiveId = LiveId,
                    NotificationToId = LiveId,
                    ReferenceId = 6,
                    StudentId = student.Id,
                    Title = "Enrolling  From apple In Live  (" + live.LiveName + ")",
                    TitleLT = "الإشتراك في بث مباشر (" + live.LiveName + ")"
                };
                //await _transactionUnit.StudentNotificationRepository.AddNotificationWithoutPush(studentNotification);
                //
                //Add to Teacher wallet
               // if (addLiveToStudent > 0)
               // {
                    var LiveCostAfterRemove30OverCostOfApple = live.CurrentSKUPrice * 70 / 100;
                    var teacherAmount = (LiveCostAfterRemove30OverCostOfApple * teacher.LiveTakenPercentage) / 100;
                    var systemAmount = LiveCostAfterRemove30OverCostOfApple - teacherAmount;
                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 3,//INP
                        Reason = "New subscription by apple in course (" + live.LiveName + ") for student:- " + student.Name + "",
                        ReasonLT = " تم اضافة اشتراك جديد في دورة(" + live.LiveName + ")" + "للطالب " + student.Name + "",
                        ReferenceNumber = paymentKey,
                        TeacherId = teacher.Id,
                        LiveId = LiveId,
                        CountryId = student.CountryId
                    };
               //     var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription by apple in live number #" + LiveId + ""
                    };
                 //   await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                 //   if (addTransactionToTeacher.Id > 0)
                 //   {
                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            NotificationToId = LiveId,
                            ReferenceId = 6,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
            //    await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

            //   }
            // }
            // }

            var insertall = await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsForLiveBulk(null,
                   studentLive, null, null/*update*/, studentNotification, teacherTransaction,
                   systemTransaction, teacherNotification);
            if (insertall)
            {
                return 7;
            }

            return 10;
        }

        public async Task<List<String>> CheckPaymentIds()
        {
            var Ids = await _transactionUnit.StudentTransactionRepository.CheckPaymentIds();
            return Ids;
        }

        public async Task<int> TrackSubscriptionByApplePurchasing(long TrackId, long StudentId,long COuntryId)
        {
            var StudentData = await _studentUnit.StudentRepository.Get(StudentId);
            var PaymentKey = PaymentKeyGenerator.getNewKey();
            var track = await _transactionUnit.TrackRepository.Get(TrackId);
            if (track == null)
            {
                return 0;
            }
            if (track.BySubscription == false)
            {
                return 5;
            }
            if (track.SubscriptionDuration == null || track.SubscriptionCurrentPrice == null || track.SubscriptionDuration == null)
            {
                return 6;
            }
            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByTrackId(TrackId);

            var IsEnrolledBefore = await _transactionUnit.TrackSubscriptionRepository.CheckIfStudentSubscripeThistrackBefore(track.Id, StudentId);
            if (IsEnrolledBefore != null && IsEnrolledBefore.DurationExpiration.Value.Date > DateTime.UtcNow.Date)
            {
                return 1;
            }
            track.SubscriptionCurrentPrice = track.SubscriptionCurrentPrice;

            //deduced from Student Wallet set amount by 0 because apple take the cost from student using apple pay and give system amount after take the presentage
            StudentTransaction studentTransactionToBuyTrack = new StudentTransaction
            { Amount = -track.SubscriptionCurrentPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Subscription By Apple In Track (" + track.Name + ")", ReasonLT = "الاشتراك في مادة (" + track.NameLT + ")", TrackId = TrackId, ReferenceNumber = PaymentKey };
            var AddTransactionWithTrackCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyTrack);
            if (AddTransactionWithTrackCost.Id > 0)
            {
                long addOrUpdateCourseToStudent = 0;
                if (IsEnrolledBefore == null)
                {
                    TrackSubscription trackSubscription = new TrackSubscription
                    {
                        StudentId = StudentId,
                        TrackId = track.Id,
                        CreationDate = DateTime.UtcNow,
                        DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value),
                        ReferenceNumber = PaymentKey
                    };
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.AddTrackToStudent(trackSubscription);
                }
                else
                {
                    IsEnrolledBefore.TrackId = track.Id;
                    IsEnrolledBefore.StudentId = StudentId;
                    IsEnrolledBefore.CreationDate = DateTime.UtcNow;
                    IsEnrolledBefore.DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value);
                    IsEnrolledBefore.ReferenceNumber = PaymentKey;
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.UpdateTrackToStudent(IsEnrolledBefore);
                }

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 5,
                    StudentId = StudentId,
                    Title = studentTransactionToBuyTrack.Reason,
                    TitleLT = studentTransactionToBuyTrack.ReasonLT
                };
                //await _transactionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                //
                //Add to Teacher wallet
                if (addOrUpdateCourseToStudent > 0)
                {
                    var TrackCostAfterRemove30OverCostOfApple = track.SKUPrice * 70 / 100;

                    var teacherAmount = (TrackCostAfterRemove30OverCostOfApple * teacher.TakenPercentage) / 100;
                    var systemAmount =   TrackCostAfterRemove30OverCostOfApple - teacherAmount;

                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 3,
                        Reason = "New subscription by apple in track (" + track.Name + ")" + " for student: " + StudentData.Name + "",
                        ReasonLT = "تم اضافة اشتراك جديد في مادة (" + track.NameLT + ")" + "للطالب : " + StudentData.Name + "",
                        ReferenceNumber = PaymentKey,
                        TeacherId = teacher.Id,
                        TrackId = track.Id,
                        CountryId=COuntryId
                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription by apple in Track number #" + track.Id + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {
                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            ReferenceId = 5,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                }
            }

            return 7;
        }
        public async Task<int> PromotionSubscription(long PromotionId, long StudentId, string PromocodeText, long CountryId)
        {
            var StudentData = await _studentUnit.StudentRepository.Get(StudentId);
            var ReferenceNumber = PaymentKeyGenerator.getNewKey();
            string promoNumber = null;
            PromoCode GetPromo = null;
            var track = await _transactionUnit.TrackPromotionRepository.Get(PromotionId);
            if (track == null)
            {
                return 0;
            }
            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByTrackId(track.TrackId.Value);

            

            if (!string.IsNullOrEmpty(PromocodeText))
            {
                GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                if (GetPromo == null)
                {
                    return 2;
                }
                if (GetPromo != null)
                {
                    var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(StudentId, GetPromo.Id);
                    if (checkIfStudentUsedThisPromoCodeBefore)
                    {
                        return 3;
                    }
                    var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(StudentId, GetPromo);
                    if (checkIfStudentValidToThisPromocode == false)
                    {
                        return 8;
                    }
                    if (checkIfStudentValidToThisPromocode)
                    {
                        var dicountAmount = (track.NewPrice * GetPromo.PromoCodeValue) / 100;
                        //var dicountAmount = GetPromo.PromoCodeValue;

                        track.NewPrice = track.NewPrice - dicountAmount;
                        promoNumber = GetPromo.PromoCodeText;
                    }

                }


            }

            var GetTotalWallet = await _transactionUnit.StudentTransactionRepository.GetTotal(StudentId);
            if (GetTotalWallet < track.NewPrice)
            {
                return 4;
            }
            //deduced from Student Wallet
            StudentTransaction studentTransactionToBuyTrack = new StudentTransaction
            { Amount = -track.NewPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 1, Reason = "Subscription In Promotion (" + track.Name + ")", ReasonLT = "الاشتراك في مادة (" + track.Name + ")", PromotionId = PromotionId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };
            var AddTransactionWithTrackCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyTrack);
            if (AddTransactionWithTrackCost.Id > 0)
            {
                TrackSubscription trackSubscription = new TrackSubscription
                {
                    StudentId = StudentId,
                    TrackId = track.TrackId.Value,
                    CreationDate = DateTime.UtcNow
                };
                await _transactionUnit.TrackSubscriptionRepository.AddTrackToStudent(trackSubscription);

                //add promo to student
                if (GetPromo != null)
                {
                    StudentPromoCode studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = StudentId
                    };
                    await _transactionUnit.StudentPromoCodeRepository.Add(studentPromoCode);
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                    await _transactionUnit.PromoCodeRepository.Update(GetPromo);

                }

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 5,
                    StudentId = StudentId,
                    Title = studentTransactionToBuyTrack.Reason,
                    TitleLT = studentTransactionToBuyTrack.ReasonLT
                };
                await _transactionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                //
                //Add to Teacher wallet
                 
                    var teacherAmount = (track.NewPrice * teacher.TakenPercentage) / 100;
                    var systemAmount = track.NewPrice - teacherAmount;

                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 1,
                        Reason = "New subscription in promotion (" + track.Name + ")" + " for student: " + StudentData.Name + "",
                        ReasonLT = "تم اضافة اشتراك جديد في مادة (" + track.Name + ")" + "للطالب : " + StudentData.Name + "",
                        ReferenceNumber = ReferenceNumber,
                        TeacherId = teacher.Id,
                        TrackId = track.TrackId,
                        PromocodeNumber = promoNumber,
                        CountryId = CountryId
                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        ReferenceNumber = ReferenceNumber,
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription in promotion number #" + track.TrackId + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {


                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            ReferenceId = 5,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                
            }

            return 7;
        }

        public async Task<int> PromotionSubscriptionByApplePurchasing(long PromotionId, long StudentId, long COuntryId)
        {
            var StudentData = await _studentUnit.StudentRepository.Get(StudentId);
            var PaymentKey = PaymentKeyGenerator.getNewKey();
            var track = await _transactionUnit.TrackRepository.Get(PromotionId);
            if (track == null)
            {
                return 0;
            }
            if (track.BySubscription == false)
            {
                return 5;
            }
            if (track.SubscriptionDuration == null || track.SubscriptionCurrentPrice == null || track.SubscriptionDuration == null)
            {
                return 6;
            }
            var teacher = await _transactionUnit.TeacherRepository.GetTeacherByTrackId(PromotionId);

            var IsEnrolledBefore = await _transactionUnit.TrackSubscriptionRepository.CheckIfStudentSubscripeThistrackBefore(track.Id, StudentId);
            if (IsEnrolledBefore != null && IsEnrolledBefore.DurationExpiration.Value.Date > DateTime.UtcNow.Date)
            {
                return 1;
            }
            track.SubscriptionCurrentPrice = track.SubscriptionCurrentPrice;

            //deduced from Student Wallet set amount by 0 because apple take the cost from student using apple pay and give system amount after take the presentage
            StudentTransaction studentTransactionToBuyTrack = new StudentTransaction
            { Amount = -track.SubscriptionCurrentPrice, StudentId = StudentId, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Subscription By Apple In Promotion (" + track.Name + ")", ReasonLT = "الاشتراك في مادة (" + track.NameLT + ")", PromotionId = PromotionId, ReferenceNumber = PaymentKey };
            var AddTransactionWithTrackCost = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransactionToBuyTrack);
            if (AddTransactionWithTrackCost.Id > 0)
            {
                long addOrUpdateCourseToStudent = 0;
                if (IsEnrolledBefore == null)
                {
                    TrackSubscription trackSubscription = new TrackSubscription
                    {
                        StudentId = StudentId,
                        TrackId = track.Id,
                        CreationDate = DateTime.UtcNow,
                        DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value),
                        ReferenceNumber = PaymentKey
                    };
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.AddTrackToStudent(trackSubscription);
                }
                else
                {
                    IsEnrolledBefore.TrackId = track.Id;
                    IsEnrolledBefore.StudentId = StudentId;
                    IsEnrolledBefore.CreationDate = DateTime.UtcNow;
                    IsEnrolledBefore.DurationExpiration = DateTime.UtcNow.AddDays(track.SubscriptionDuration.Value);
                    IsEnrolledBefore.ReferenceNumber = PaymentKey;
                    addOrUpdateCourseToStudent = await _transactionUnit.TrackSubscriptionRepository.UpdateTrackToStudent(IsEnrolledBefore);
                }

                //add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    ReferenceId = 5,
                    StudentId = StudentId,
                    Title = studentTransactionToBuyTrack.Reason,
                    TitleLT = studentTransactionToBuyTrack.ReasonLT
                };
                await _transactionUnit.StudentNotificationRepository.AddNotification(studentNotification);
                //
                //Add to Teacher wallet
                if (addOrUpdateCourseToStudent > 0)
                {
                    var TrackCostAfterRemove30OverCostOfApple = track.SKUPrice * 70 / 100;

                    var teacherAmount = (TrackCostAfterRemove30OverCostOfApple * teacher.TakenPercentage) / 100;
                    var systemAmount = TrackCostAfterRemove30OverCostOfApple - teacherAmount;

                    TeacherTransaction teacherTransaction = new TeacherTransaction
                    {
                        Amount = teacherAmount,
                        CreationDate = DateTime.UtcNow,
                        PaymentMethodId = 3,
                        Reason = "New subscription by apple in track (" + track.Name + ")" + " for student: " + StudentData.Name + "",
                        ReasonLT = "تم اضافة اشتراك جديد في مادة (" + track.NameLT + ")" + "للطالب : " + StudentData.Name + "",
                        ReferenceNumber = PaymentKey,
                        TeacherId = teacher.Id,
                        TrackId = track.Id,
                        CountryId = COuntryId
                    };
                    var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                    SystemTransaction systemTransaction = new SystemTransaction
                    {
                        Amount = systemAmount,
                        CreationDate = DateTime.UtcNow,
                        Reason = "New subscription by apple in Track number #" + track.Id + ""
                    };
                    await _transactionUnit.SystemTransactionRepository.Add(systemTransaction);
                    if (addTransactionToTeacher.Id > 0)
                    {
                        //add notification
                        TeacherNotification teacherNotification = new TeacherNotification
                        {
                            CreationDate = DateTime.UtcNow,
                            ReferenceId = 5,
                            TeacherId = teacher.Id,
                            Title = teacherTransaction.Reason,
                            TitleLT = teacherTransaction.ReasonLT
                        };
                        await _transactionUnit.TeacherNotificationRepository.AddNotification(teacherNotification);

                    }
                }
            }

            return 7;
        }
       
        public async Task<long> addCowPayLog(CowPayLog cowPayLog)
        {
            var StudentId = long.Parse(cowPayLog.customer_merchant_profile_id);
            var StudentData = await _studentService.GetStudentProfile(StudentId);
            var id = await _transactionUnit.CowPayLogRepository.Add(cowPayLog);

            // var IsInsertedBefore = await _transactionUnit.StudentTransactionRepository.CheckIsThisTransactionFound(cowPayLog.payment_gateway_reference_id);
            var IsInsertedBefore = await _transactionUnit.StudentTransactionRepository.CheckIsThisTransactionFound(cowPayLog.cowpay_reference_id);

            if (cowPayLog.order_status == "PAID" && IsInsertedBefore == null)
            {
                var Amount = decimal.Parse(cowPayLog.amount);
                if (StudentData.CountryId != 20011)
                {
                    Amount = Amount / 50;
                }
                StudentTransaction transaction = new StudentTransaction();
                transaction.Amount = Amount;
                transaction.StudentId = long.Parse(cowPayLog.customer_merchant_profile_id);
                transaction.ReferenceNumber = cowPayLog.cowpay_reference_id;
                transaction.PaymentMethodId = 1;
                transaction.CreationDate = DateTime.UtcNow;
                transaction.Reason = (cowPayLog.channel== "PAYATFAWRY" || cowPayLog.channel == "CARD") ? "Charging wallet by Fawry"  : "Charging wallet by CowPay_updated2";
                transaction.ReasonLT = (cowPayLog.channel == "PAYATFAWRY" || cowPayLog.channel == "CARD") ? "شحن عملية بفورى" : "شحن المحفظة عن طريق كاوباي";
                await _transactionUnit.StudentTransactionRepository.Add(transaction);

                var d = Task.Run(async () =>
                {
                    StudentNotification studentNotification = new StudentNotification()
                    {
                        CreationDate = DateTime.UtcNow,
                        NotificationToId = transaction.StudentId,
                        NotificationTypeId = 2,
                        ReferenceId = 5,
                        StudentId = transaction.StudentId,
                        Title = transaction.Reason,
                        TitleLT = transaction.ReasonLT
                    };
                    await _studentUnit.StudentNotificationRepository.AddNotification(studentNotification);

                });
            }
            return id;
        }



        public async Task<long> AddStudentTransaction(int type, long? id, BankResponse bank)
        {
            try
            {
                var InvoiceTransaction = bank.Data.InvoiceTransactions.FirstOrDefault();

                var studentTransaction = new StudentTransaction();
                studentTransaction.Amount = bank.Data.InvoiceValue;
                studentTransaction.StudentId = long.Parse(bank.Data.CustomerName);
                studentTransaction.CreationDate = DateTime.UtcNow;
                studentTransaction.PaymentId = InvoiceTransaction != null ? InvoiceTransaction.PaymentId : (bank.Data.InvoiceId != null) ? bank.Data.InvoiceId.ToString() : bank.Data.CustomerMobile;
                studentTransaction.PaymentMethodId = 3;
                studentTransaction.ReferenceNumber = PaymentKeyGenerator.getNewKey();

                // add course transaction
                if (type == 2)
                {
                    var course = await _transactionUnit.CourseRepository.Get(id);
                    studentTransaction.Reason = "Enrolling In Course From Web  (" + course.Name + ")";
                    studentTransaction.ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")";
                    studentTransaction.CourseId = course.Id;
                    var result = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransaction);
                    return result.Id;

                }
                // add live transaction
                else if (type == 3)
                {
                    var live = await _studentUnit.LiveRepository.Get(id);
                    studentTransaction.Reason = "Enrolling In Live From Web  (" + live.LiveName + ")";
                    studentTransaction.ReasonLT = "الإشتراك في بث مباشر  (" + live.LiveName + ")";
                    studentTransaction.LiveId = live.Id;
                    var result = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransaction);
                    return result.Id;
                }
                //Charge Wallet
                else if (type == 4)
                {
                    studentTransaction.Reason = "Charging Wallet by KNET";
                    studentTransaction.ReasonLT = "شحن المحفظة باستخدام KNET";
                    var result = await _transactionUnit.StudentTransactionRepository.AddTransaction(studentTransaction);
                    return result.Id;
                }



                return 0;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }


        public async Task<BankResponse> LogTransaction(int type, BankResponse bankResponse)
        {
            BankResponse bankResponseHeader = new BankResponse()
            {
                IsSuccess = bankResponse.IsSuccess,
                Message = bankResponse.Message,
                ValidationErrors = bankResponse.ValidationErrors
            };
            var bankResponseHeaderId = await _transactionUnit.BankResponseRepository.Add(bankResponseHeader);
            if (bankResponseHeaderId > 0 && bankResponse.Data != null)
            {
                var data = bankResponse.Data;
                data.BankResponseId = bankResponseHeaderId;
                var dataId = await _transactionUnit.DataRepository.Add(data);
                if (dataId > 0 && bankResponse.Data.InvoiceTransactions.Count() > 0)
                {

                    foreach (var item in bankResponse.Data.InvoiceTransactions)
                    {
                        item.DataId = dataId;
                        var insertionResult = await _transactionUnit.InvoiceTransactionRepository.Add(item);

                    }

                    var InvoiceTransaction = bankResponse.Data.InvoiceTransactions.Where(i => i.TransactionStatus == "Succss").FirstOrDefault();

                    if (InvoiceTransaction != null)
                    {



                        var result = await _transactionUnit.StudentTransactionRepository.GetAll(" where PaymentId ='" + InvoiceTransaction.PaymentId + "' or PaymentId ='" + bankResponse.Data.InvoiceId + "'");
                        var transaction = result.FirstOrDefault();
                        if (transaction != null)
                        {
                            decimal TheFactor = 1;
                            var studentData = await _studentUnit.StudentRepository.Get(long.Parse(bankResponse.Data.CustomerName));
                            if (studentData != null)
                            {
                                var paymentMethodType = (await _transactionUnit.PaymentMethodTypeRepository.GetAll(" where CountryId=" + studentData.CountryId + " and Id!=2 ")).FirstOrDefault();
                                if (paymentMethodType != null)
                                {
                                    TheFactor = (decimal)paymentMethodType.TheFactor.Value;
                                }
                            }
                            transaction.Amount = decimal.Parse(InvoiceTransaction.TransationValue) / TheFactor;
                            transaction.StudentId = long.Parse(bankResponse.Data.CustomerName);
                            transaction.ReferenceNumber = InvoiceTransaction.ReferenceId;
                            transaction.PaymentMethodId = 1;
                            await _transactionUnit.StudentTransactionRepository.Update(transaction);
                            StudentNotification studentNotification = new StudentNotification()
                            {
                                CreationDate = DateTime.UtcNow,
                                NotificationToId = transaction.StudentId,
                                NotificationTypeId = 2,
                                ReferenceId = 5,
                                StudentId = transaction.StudentId,
                                Title = transaction.Reason,
                                TitleLT = transaction.ReasonLT
                            };
                            await _studentUnit.StudentNotificationRepository.AddNotification(studentNotification);
                        }

                    }
                }
            }
            return bankResponse;
        }

        public async Task<int> BuyCourseFromWeb(long CourseId, Student student, string PromocodeText, long CountryId, bool isPayFromWallet = false)
        {
            try
            {

                var ReferenceNumber = PaymentKeyGenerator.getNewKey();
                string promoNumber = null;
                PromoCode GetPromo = null;
                var course = await _transactionUnit.CourseRepository.Get(CourseId);
                var track = await _transactionUnit.TrackRepository.GetTrackByCourseId(CourseId);
                if (course == null)
                {
                    return 0;
                }
                if (track.BySubscription == true)
                {
                    return 5;
                }
                if (course.CurrentCost == null)
                {
                    return 6;
                }

                var teacher = await _transactionUnit.TeacherRepository.GetTeacherByCourseId(CourseId);

                var IsEnrolledBefore = await _transactionUnit.StudentCourseRepository.CheckIfStudentEnrolledInThisCourseBefore(CourseId, student.Id);
                if (IsEnrolledBefore)
                {
                    return 1;
                }

                //promocode
                if (!string.IsNullOrEmpty(PromocodeText))
                {
                    GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                    if (GetPromo == null)
                    {
                        return 2;
                    }
                    if (GetPromo != null)
                    {
                        var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                        if (checkIfStudentUsedThisPromoCodeBefore)
                        {
                            return 3;
                        }
                        var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                        if (checkIfStudentValidToThisPromocode == false)
                        {
                            return 8;
                        }
                        if (checkIfStudentValidToThisPromocode)
                        {
                            var dicountAmount = (course.CurrentCost * GetPromo.PromoCodeValue) / 100;
                            //var dicountAmount = GetPromo.PromoCodeValue;
                            course.CurrentCost = course.CurrentCost - dicountAmount;
                            promoNumber = GetPromo.PromoCodeText;
                        }
                    }
                }


                StudentPromoCode studentPromoCode = null;
                ////deduced from Student Wallet
                ////01
                StudentTransaction studentTransactionToBuyCourse = new StudentTransaction();
                //if (isPayFromWallet == true)
                //{
                //   // studentTransactionToBuyCourse = new StudentTransaction { Amount = course.CurrentCost < 0 ? course.CurrentCost : course.CurrentCost * -1, CourseId = course.Id, PaymentMethodId = 1, CreationDate = DateTime.Now, StudentId = student.Id, Reason = "Enrolling In Course From Web_Wallet (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", ReferenceNumber = "PayFromWallet" };
                //}



                studentTransactionToBuyCourse = new StudentTransaction { Amount = course.CurrentCost < 0 ? course.CurrentCost : course.CurrentCost, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Direct paymentTo Buy Course From Web (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };

                var settlePaymentToBuyCourse = new StudentTransaction { Amount = course.CurrentCost < 0 ? course.CurrentCost : course.CurrentCost * -1, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Settle paymentTo Buy Course From Web (" + course.Name + ")", ReasonLT = "الإشتراك في دورة (" + course.NameLT + ")", CourseId = CourseId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };

                StudentCourse studentCourse = new StudentCourse { StudentId = student.Id, ReferenceNumber = ReferenceNumber, CourseId = course.Id, EnrollmentDate = DateTime.UtcNow };
                //add promo to student
                if (GetPromo != null)
                {
                    //03
                    studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = student.Id
                    };

                    //04 update
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                }
                //05 add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    CourseId = CourseId,
                    NotificationToId = CourseId,
                    ReferenceId = 1,
                    StudentId = student.Id,
                    Title = "Enrolling In Course From Web (" + course.Name + ")",
                    TitleLT = "الإشتراك في دورة (" + course.NameLT + ")"
                };
                //06 Add to Teacher wallet
                var teacherAmount = (course.CurrentCost * teacher.TakenPercentage) / 100;
                var systemAmount = course.CurrentCost - teacherAmount;
                TeacherTransaction teacherTransaction = new TeacherTransaction
                {
                    Amount = teacherAmount,
                    CreationDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    Reason = "New subscription From Web in course (" + course.Name + ") for student:- " + student.Name + "",
                    ReasonLT = " تم اضافة اشتراك جديد في دورة(" + course.NameLT + ")" + "للطالب " + student.Name + "",
                    ReferenceNumber = ReferenceNumber,
                    TeacherId = teacher.Id,
                    CourseId = CourseId,
                    PromocodeNumber = promoNumber,
                    CountryId = CountryId
                };
                //   var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                //07 add to system
                SystemTransaction systemTransaction = new SystemTransaction
                {
                    ReferenceNumber = ReferenceNumber,
                    Amount = systemAmount,
                    CreationDate = DateTime.UtcNow,
                    Reason = "New subscription From Web in course number #" + CourseId + ""
                };
                //08 add notification
                TeacherNotification teacherNotification = new TeacherNotification
                {
                    CreationDate = DateTime.UtcNow,
                    NotificationToId = CourseId,
                    ReferenceId = 1,
                    TeacherId = teacher.Id,
                    Title = teacherTransaction.Reason,
                    TitleLT = teacherTransaction.ReasonLT
                };



                var insertall = await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsBulk2(studentTransactionToBuyCourse, settlePaymentToBuyCourse,
                    studentCourse, studentPromoCode, GetPromo/*update*/, studentNotification, teacherTransaction,
                    systemTransaction, teacherNotification);
                if (insertall)
                {
                    return 7;
                }

                return 10;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> BuyLiveFromWeb(long liveId, Student student, string PromocodeText, long CountryId)
        {
            try
            {

                var ReferenceNumber = PaymentKeyGenerator.getNewKey();
                string promoNumber = null;
                PromoCode GetPromo = null;
                var live = await _studentUnit.LiveRepository.Get(liveId);
                if (live == null)
                {
                    return 0;
                }

                var teacher = await _transactionUnit.TeacherRepository.GetTeacherByLiveId(liveId);

                var IsEnrolledBefore = await _transactionUnit.StudentLiveRepository.CheckIfStudentEnrolledInThisLiveBefore(liveId, student.Id);
                if (IsEnrolledBefore)
                {
                    return 1;
                }

                //promocode
                if (!string.IsNullOrEmpty(PromocodeText))
                {
                    GetPromo = await _transactionUnit.PromoCodeRepository.IsThisPromoCodeValid(PromocodeText);
                    if (GetPromo == null)
                    {
                        return 2;
                    }
                    if (GetPromo != null)
                    {
                        var checkIfStudentUsedThisPromoCodeBefore = await _transactionUnit.StudentPromoCodeRepository.checkIfStudentUsedThisPromoCodeBefore(student.Id, GetPromo.Id);
                        if (checkIfStudentUsedThisPromoCodeBefore)
                        {
                            return 3;
                        }
                        var checkIfStudentValidToThisPromocode = await _transactionUnit.PromoCodeRepository.StudentValidToThisPromocode(student.Id, GetPromo);
                        if (checkIfStudentValidToThisPromocode == false)
                        {
                            return 8;
                        }
                        if (checkIfStudentValidToThisPromocode)
                        {
                            var dicountAmount = (live.CurrentPrice * GetPromo.PromoCodeValue) / 100;
                            //var dicountAmount = GetPromo.PromoCodeValue;
                            live.CurrentPrice = live.CurrentPrice - dicountAmount.Value;
                            promoNumber = GetPromo.PromoCodeText;
                        }
                    }
                }


                StudentPromoCode studentPromoCode = null;
                //deduced from Student Wallet
                //01
                //StudentTransaction studentTransactionToBuyCourse = new StudentTransaction { Amount = -live.CurrentPrice, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Enrolling In Live (" + live.LiveName + ")", ReasonLT = "الإشتراك في بث مباشر (" + live.LiveName + ")", LiveId = liveId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };

                StudentTransaction studentTransactionToBuyLive = new StudentTransaction { Amount = live.CurrentPrice < 0 ? live.CurrentPrice : live.CurrentPrice, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Direct paymentTo Buy Live by apple (" + live.LiveName + ")", ReasonLT = "الإشتراك في دورة (" + live.LiveName + ")", LiveId = liveId, ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };

                var settlePaymentToBuyCourse = new StudentTransaction { Amount = live.CurrentPrice < 0 ? live.CurrentPrice : live.CurrentPrice * -1, StudentId = student.Id, CreationDate = DateTime.UtcNow, PaymentMethodId = 3, Reason = "Settle paymentTo Buy Live by apple (" + live.LiveName + ")", ReasonLT = "الإشتراك في دورة (" + live.LiveName + ")", LiveId = liveId , ReferenceNumber = ReferenceNumber, PromocodeNumber = promoNumber };


                //02
                StudentLive studentLive = new StudentLive { StudentId = student.Id, ReferenceNumber = ReferenceNumber, LiveId = live.Id, EnrollmentDate = DateTime.UtcNow };
                //add promo to student
                if (GetPromo != null)
                {
                    //03
                    studentPromoCode = new StudentPromoCode
                    {
                        PromoCodeId = GetPromo.Id,
                        StudentId = student.Id
                    };

                    //04 update
                    GetPromo.UsedCount = GetPromo.UsedCount + 1;
                }
                //05 add notification
                StudentNotification studentNotification = new StudentNotification
                {
                    CreationDate = DateTime.UtcNow,
                    LiveId = liveId,
                    NotificationToId = liveId,
                    ReferenceId = 6,
                    StudentId = student.Id,
                    Title = "Enrolling  From Web In Live  (" + live.LiveName + ")",
                    TitleLT = "الإشتراك في بث مباشر (" + live.LiveName + ")"
                };
                //06 Add to Teacher wallet
                var teacherAmount = (live.CurrentPrice * teacher.LiveTakenPercentage) / 100;
                var systemAmount = live.CurrentPrice - teacherAmount;
                TeacherTransaction teacherTransaction = new TeacherTransaction
                {
                    Amount = teacherAmount,
                    CreationDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    Reason = "New subscription From Web in live (" + live.LiveName + ") for student:- " + student.Name + "",
                    ReasonLT = " تم اضافة اشتراك جديد في بث مباشر(" + live.LiveName + ")" + "للطالب " + student.Name + "",
                    ReferenceNumber = ReferenceNumber,
                    TeacherId = teacher.Id,
                    LiveId = liveId,
                    PromocodeNumber = promoNumber,
                    CountryId = CountryId
                };
                var addTransactionToTeacher = await _transactionUnit.TeacherTransactionRepository.AddTransaction(teacherTransaction);
                //07 add to system
                SystemTransaction systemTransaction = new SystemTransaction
                {
                    ReferenceNumber = ReferenceNumber,
                    Amount = systemAmount,
                    CreationDate = DateTime.UtcNow,
                    Reason = "New subscription From Web in live number #" + liveId + ""
                };
                //08 add notification
                TeacherNotification teacherNotification = new TeacherNotification
                {
                    CreationDate = DateTime.UtcNow,
                    NotificationToId = liveId,
                    ReferenceId = 6,
                    TeacherId = teacher.Id,
                    Title = teacherTransaction.Reason,
                    TitleLT = teacherTransaction.ReasonLT
                };
                var insertall = await _transactionUnit.StudentTransactionRepository.AddTransactionRecordsForLiveBulk(null,
                    studentLive, studentPromoCode, GetPromo/*update*/, studentNotification, teacherTransaction,
                    systemTransaction, teacherNotification);
                if (insertall)
                {
                    return 7;
                }

                return 10;
            }
            catch (Exception e)
            {

                throw;
            }
        }

    }
}


//if (dataId>0 && bankResponse.Data.InvoiceTransactions.FirstOrDefault()!=null)
//                {
                    

//                    var InvoiceTransaction = bankResponse.Data.InvoiceTransactions.FirstOrDefault();
//InvoiceTransaction.DataId = dataId;
//                    var InvoiceTransactionId = await _transactionUnit.InvoiceTransactionRepository.Add(InvoiceTransaction);
//                    if (InvoiceTransaction.TransactionStatus== "Succss")
//                    {
//                        var result = await _transactionUnit.StudentTransactionRepository.GetAll(" where PaymentId ='" + InvoiceTransaction.PaymentId + "'");
//var transaction = result.FirstOrDefault();
//                        if (transaction!=null)
//                        {
//                            transaction.Amount = decimal.Parse(InvoiceTransaction.TransationValue);
//transaction.StudentId =long.Parse(bankResponse.Data.CustomerName);
//transaction.ReferenceNumber = InvoiceTransaction.ReferenceId;
//                            transaction.PaymentMethodId = 1;
//                            await _transactionUnit.StudentTransactionRepository.Update(transaction);
//                        }

//                    }
//                }


