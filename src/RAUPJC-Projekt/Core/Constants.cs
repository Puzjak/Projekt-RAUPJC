using System;

namespace RAUPJC_Projekt.Core
{
    public class Constants
    {
        public const string CustomerRoleName = "Customer";
        public const string EmployeeRoleName = "Employee";
        public const string AdministratorRoleName = "Administrator";

        public const string SessionKeyDate = "Date";
        public const string SessionKeyService = "Service";
        public const string SessionKeyUserId = "UserId";
        public const string SessionKeyUser = "User";
        public const string SessionKeyFreeTerms = "FreeTerms";
        public const string SessionKeyRoles = "Roles";
        public const string SessionKeyTermIds = "TermIds";


        public const int StartOfWorkingHoursHour = 9;
        public const int StartOfWorkingHoursMinute = 0;
        public const int EndOfWorkingHoursHour = 20;
        public const int EndOfWorkingHoursMinute = 0;
        public const int WorkingMinutes =
            (EndOfWorkingHoursHour - StartOfWorkingHoursHour)*60 + StartOfWorkingHoursMinute + EndOfWorkingHoursMinute;
        public const int MinuteStep = 30;
        public const int NumberOfWorkers = 2;

        public static readonly DateTime StartOfWorkingHours = new DateTime()
            .AddHours(StartOfWorkingHoursHour).AddMinutes(StartOfWorkingHoursMinute);
        public static readonly DateTime EndOfWorkingHours = new DateTime()
            .AddHours(EndOfWorkingHoursHour).AddMinutes(EndOfWorkingHoursMinute);

    }
}
