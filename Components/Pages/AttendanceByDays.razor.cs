using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OfficeOpenXml;
using TrackFlow.Models.dbforall;

namespace TrackFlow.Components.Pages
{
    public partial class AttendanceByDays : ComponentBase
    {
        [Inject]
        public dbforallService dbforallService { get; set; }
        
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private List<UserAttendance> userAttendance;
        private List<DateTime> daysInMonth;
        private int currentMonth;
        private int currentYear;
        private DateTime selectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);


        protected async Task OnCurrentDateChanged(DateTime args)
        {
            selectedDate = new DateTime(args.Year, args.Month, 1);
            await LoadAttendanceData(args.Year, args.Month);
        }


        protected override async Task OnInitializedAsync()
        {
            
            // Initialize current month and year
            currentMonth = DateTime.Now.Month;
            currentYear = DateTime.Now.Year;

            // Load attendance data for the current month
            await LoadAttendanceData(currentYear, currentMonth);
        }

        private async Task LoadAttendanceData(int year, int month)
        {
            try
            {

                if (dbforallService == null)
                {
                    // Log or handle the case where _dbforallService is null
                    Console.WriteLine("_dbforallService is null");
                    return;
                }

                // Get the list of users
                var users = await dbforallService.GetAspNetUsers();

                // Get the list of days in the specified month
                daysInMonth = GetDaysInMonth(year, month);

                // Initialize userAttendance list
                userAttendance = new List<UserAttendance>();

                // Retrieve activity records for each user and determine attendance for each day
                foreach (var user in users)
                {
                    var userAttendanceData = new UserAttendance
                    {
                        UserName = user.UserName,
                        AttendanceStatus = new Dictionary<DateTime, bool>()
                    };

                    // Retrieve activity records for the user within the specified month
                    var activityRecords = await dbforallService.GetActivityRecordsForUserInMonth(user.Id, year, month);

                    foreach (var day in daysInMonth)
                    {
                        var parsedDate = day.Date;
                        var attendanceStatus = activityRecords.Any(record => record.ShiftStartTime != null && DateTime.Parse(record.ShiftStartTime).Date == parsedDate);
                        userAttendanceData.AttendanceStatus.Add(day, attendanceStatus);
                    }

                    userAttendance.Add(userAttendanceData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception to maintain the normal error handling flow
            }
        }

        private void ExportToExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add($"{DateTimeFormatInfo.CurrentInfo.GetMonthName(currentMonth)}_{currentYear}");

            // Add headers
            worksheet.Cells[1, 1].Value = "User";
            for (int i = 0; i < daysInMonth.Count; i++)
            {
                worksheet.Cells[1, i + 2].Value = daysInMonth[i].ToString("dd");
            }

            // Add data
            for (int i = 0; i < userAttendance.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = userAttendance[i].UserName;
                for (int j = 0; j < daysInMonth.Count; j++)
                {
                    var attendanceStatus = userAttendance[i].AttendanceStatus[daysInMonth[j]];
                    worksheet.Cells[i + 2, j + 2].Value = attendanceStatus ? "Present" : "Absent";
                }
            }

            // Auto fit columns
            worksheet.Cells.AutoFitColumns(0);

            // Save the Excel file
            var stream = new MemoryStream();
            package.SaveAs(stream);

            // Download the Excel file
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var excelFile = new MemoryStream(stream.ToArray());
            var dataUri = $"data:{contentType};base64,{Convert.ToBase64String(excelFile.ToArray())}";
            NavigationManager.NavigateTo(dataUri, true);
        }

        private List<DateTime> GetDaysInMonth(int year, int month)
        {
            var daysInMonth = new List<DateTime>();
            var daysInMonthCount = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= daysInMonthCount; i++)
            {
                daysInMonth.Add(new DateTime(year, month, i));
            }

            return daysInMonth;
        }



        public class UserAttendance
        {
            public string UserName { get; set; }
            public Dictionary<DateTime, bool> AttendanceStatus { get; set; }
        }
    }
}
