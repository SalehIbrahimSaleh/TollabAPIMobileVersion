using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TollabAPI.Utils
{
    public class AppConstants
    {
        public const string Result = "result";
        public const string Failed = "failed";
        public const string Message = "message";
        public const string Success = "success";
        public const string Error = "errors";
        public const string Metas = "metas";
        public const string OTP = "OTP";
        public const string Model = "model";
        public const string Accept_Language = "Accept-Language";
        public const string Language = "language";
        public const string UserName = "userName";
        public const string Password = "password";
        public const string Doctor = "Doctor";
        public const string TimeSlot = "TimeSlot";
        public const string VideoCall= "VideoCall";
        public const string TimeTable="TimeTable";
        public const string Transaction="Transaction";
        public const string Reservation = "Reservation";
        public const string PharmacyLaboratoryService = "PharmacyLaboratoryService";
        public const string Notifcation="Notifcation";
        public const string DoctorSubService="DoctorSubService";
        public const string DoctorInssurance="DoctorInssurance";
        public const string ContactUs="ContactUs";
        public const string ComplaintsAndSuggestion="ComplaintsAndSuggestion";
        public const string Teacher = "Teacher";
        public const string Favorite= "Favorite";
        public const string Count= "Count";
        public const string  Student= "Student";
        public const string Token= "Token";
        public const string Order= "Order";
        public const string User = "User";
        public const string CustomerAddress= "CustomerAddress";
        public const string Technician= "Technician";
        public const string OrderPhoto= "OrderPhoto";
        public const string Rate = "Rate";
        public const string Complaint = "Complaint";
        public const string Code="Code";
        public const string Exam="Exam";

        ///////////////////////////////////////////////

        public const int This_Phone_Registerd_Before                     =1 ;
        public const int Result_No_Data_Found                            =2 ;
        public const int Result_error_try_later                          =3 ;
        public const int Result_invalid_password                         =4 ;
        public const int Result_Invalid_Email                            =5 ;
        public const int Result_Invalide_code                            =6 ;
        public const int Result_invalid_user_name_or_password            =7 ;
        public const int Result_invalid_access_token_relogin             =8 ;
        public const int Plese_complete_registeration                    =9 ;
        public const int Invalide_PhoneNumber                            =10;
        public const int Returned_Successfully                           =11;
        public const int Added_Successfully                              =12;
        public const int MOBILE_MAX_LENGTH                               =13;
        public const int MOBILE_MIN_LENGTH                               =14;
        public const int Result_No_Data_returned                         =15;
        public const int Result_Employee_not_found                       =16;
        public const int Result_Invalid_Parametrs                        =17;
        public const int This_Employee_Authorize_To_Update_On_This_Order =18;
        public const int Result_Student_not_Register_Yet                 =19;
        public const int Invalide_PhoneNumber_Or_Email                   =20;
        public const int This_Employee_Authorize_To_Assign_This_Order    =21;
        public const int Employee_Is_Disabled_From_Admin                 =22;
        public const int Student_Not_Found                               =23;
        public const int Operation_Not_Completed                         =24;
        public const int User_Not_Found                                  =25;
        public const int Result_Invalid_Course_Id                        =26;
        public const int Result_Enrolled_Before                          =27;
        public const int Result_This_PromoCode_InValid                   =28;
        public const int Result_You_Are_Used_This_PromoCode_Before       =29;
        public const int Result_You_Need_Charge_Your_Wallet              =30;
        public const int Result_Teacher_not_Register_Yet                 =31;
        public const int Result_Teacher_not_found                        =32;
        public const int Incorrect_Password                              =33;
        public const int Invalide_Password                               =34;
        public const int Teacher_Not_Found                               =35;
        public const int Maximum_Letters_Are_150                         =36;
        public const int Try_with_Other_Name                             =37;                         
        public const int This_Course_By_Track_Subscription               =38;
        public const int This_Track_By_Course_Subscription               =39;
        public const int Not_Valid_Price_Or_Duration                     =40;
        public const int Not_Valid_Price                                 =41;
        public const int Your_Account_Is_Disabled                        =42;
        public const int Result_Invalid_Exam_Id                          =43;
        public const int Invalide_Parameter                              =44;
        public const int Deadline_Date_Less_Than_Start_Date              =45;
        public const int This_Exam_Published_and_can_not_Update_It       =46;
        public const int Invalide_Exam_Type                              =47;
        public const int No_Files                                        =48;
        public const int Please_Add_Answers                              =49;
        public const int This_Exam_Not_Published_yet                     =50;
        public const int Not_Solved_Yet                                  =51;
        public const int Result_Invalid_Answer_Id                        =52;
        public const int Invalide_ExamQuestion_Type                      =53;
        public const int You_started_this_Exam                           =54;
        public const int You_did_not_start_this_Exam                     =55;
        public const int Invalid_SolveStatus                             =56;
        public const int This_Exam_Closed                                =57;
        public const int No_Questions_Added                              =58;
        public const int You_Can_Not_Add_Question_To_This_Exam_Because_Students_Solved_It = 59;
        public const int Invalide_ExamQuestion_Id = 60;
        public const int You_Can_Not_Update_Question_To_This_Exam_Because_Students_Solved_It = 61;
        public const int Result_Teacher_Assistant_not_found                                  = 62;
        public const int You_Can_Not_Delete_This_Exam_Because_Students_Solved_It             = 63;
        public const int You_Can_Not_Update_This_Exam_Because_Students_Solved_It             = 64;
        public const int You_Can_Not_change_Exam_type_Because_Question_added_It              = 65;
        public const int INVALID_CountrY_CODE = 66;
        public const int Unauthorized = 401;


        /////////////////////////////////////////////////
        public const int ExamTypeInteractive = 1;
        public const int ExamTypeFile = 2;

        public const int ExamQuestionType_True_Or_False = 1;
        public const int ExamQuestionType_MultipleChoice = 2;
        public const int ExamQuestionType_Article = 3;
        public const int ExamQuestionType_Pdf = 4;




        public const int SolveStatus_NotComplete = 1;
        public const int Late = 2;
        public const int On_Time = 3;

 
        public static List<string> getCountryCodes()
        {
            List<string> codes = new List<string>();
            codes.Add("+966");
            codes.Add("+2");
            return codes;

        }
    }
}