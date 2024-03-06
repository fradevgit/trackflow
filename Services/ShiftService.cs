using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;


using Radzen;
using System.Reflection;

namespace TrackFlow.Services
{
    public class ShiftService
    {
        private readonly dbforallService _dbforallService;
        private readonly object _lock = new(); // Lock object for synchronization


        public ShiftService(dbforallService dbforallService)
        {
            _dbforallService = dbforallService;
        }

        public bool IsShiftStarted { get; private set; }
        public bool IsOnBreak { get; private set; }
        public bool IsOnLunch { get; private set; }
        public DateTime ShiftStartTime { get; private set; }
        public List<ShiftActivity> ShiftActivities { get; private set; } = new List<ShiftActivity>();
        public int BreakCount { get; private set; } // New property to track break count
        public int LunchCount { get; private set; } // New property to track lunch count
        private int activityNumber; // Start numbering from 0


        public async Task LoadUserActivities(string userId)
        {
            try
            {
                // Synchronize access to ShiftActivities list
                lock (ShiftActivities)
                {
                    // Clear the history when starting a new shift
                    ShiftActivities.Clear();

                    // Initialize ShiftActivities if not already initialized
                    ShiftActivities ??= new List<ShiftActivity>();
                }

                var mostRecentRecord = await _dbforallService.GetMostRecentUserRecord(userId);

                IsShiftStarted = DateTime.TryParseExact(mostRecentRecord?.ShiftStartTime, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime shiftStartTime) 
                                && shiftStartTime.Date == DateTime.Today 
                                && shiftStartTime != DateTime.MinValue 
                                && string.IsNullOrEmpty(mostRecentRecord.ShiftEndTime);

                // Initialize ShiftActivities if not already initialized
                if (IsShiftStarted)
                {
                    activityNumber = 0; // Start numbering from 1 for this shift

                    // Start of shift
                    ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.StartShift, Time = shiftStartTime });

                    // Count breaks from recent record
                    BreakCount = -1;
                    if (!string.IsNullOrEmpty(mostRecentRecord.Break1StartTime)) BreakCount++;
                    if (!string.IsNullOrEmpty(mostRecentRecord.Break2StartTime)) BreakCount++;
                    if (!string.IsNullOrEmpty(mostRecentRecord.Break3StartTime)) BreakCount++;
                    if (!string.IsNullOrEmpty(mostRecentRecord.Break4StartTime)) BreakCount++;
                    

                    // Start and end times for breaks
                    AddBreakActivityIfExist(mostRecentRecord?.Break1StartTime, ShiftActivityType.StartBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break1EndTime, ShiftActivityType.EndBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break2StartTime, ShiftActivityType.StartBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break2EndTime, ShiftActivityType.EndBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break3StartTime, ShiftActivityType.StartBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break3EndTime, ShiftActivityType.EndBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break4StartTime, ShiftActivityType.StartBreak);
                    AddBreakActivityIfExist(mostRecentRecord?.Break4EndTime, ShiftActivityType.EndBreak);

                    // Check for ongoing lunch
                    if (!string.IsNullOrEmpty(mostRecentRecord?.LunchStartTime))
                    {
                        if (string.IsNullOrEmpty(mostRecentRecord?.LunchEndTime))
                        {
                            // If LunchEndTime is null or empty, lunch is ongoing
                            IsOnLunch = true;
                            LunchCount = 1; // Increment lunch count for ongoing lunch
                            AddBreakActivityIfExist(mostRecentRecord?.LunchStartTime, ShiftActivityType.StartLunch);
                        }
                        else
                        {
                            // If both LunchStartTime and LunchEndTime are present, lunch is completed
                            LunchCount = 1;
                            AddBreakActivityIfExist(mostRecentRecord?.LunchStartTime, ShiftActivityType.StartLunch);
                            AddBreakActivityIfExist(mostRecentRecord?.LunchEndTime, ShiftActivityType.EndLunch);
                        }
                    }

                    // Check for ongoing break
                    if (!string.IsNullOrEmpty(mostRecentRecord?.Break1StartTime) && string.IsNullOrEmpty(mostRecentRecord?.Break1EndTime) ||
                        !string.IsNullOrEmpty(mostRecentRecord?.Break2StartTime) && string.IsNullOrEmpty(mostRecentRecord?.Break2EndTime) ||
                        !string.IsNullOrEmpty(mostRecentRecord?.Break3StartTime) && string.IsNullOrEmpty(mostRecentRecord?.Break3EndTime) ||
                        !string.IsNullOrEmpty(mostRecentRecord?.Break4StartTime) && string.IsNullOrEmpty(mostRecentRecord?.Break4EndTime))
                    {
                        IsOnBreak = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Logging any exceptions that occur
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }





        private void AddBreakActivityIfExist(string timeString, ShiftActivityType type)
        {
            if (!string.IsNullOrEmpty(timeString))
            {
                if (DateTime.TryParseExact(timeString, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime time))
                {
                    // Insert the activity at index 0 to ensure it's added at the bottom (newest)
                    ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = type, Time = time });
                }
            }
        }





        

        public void StartShift()
        {
        
            IsShiftStarted = true;
            ShiftStartTime = DateTime.Now;
            ShiftActivities.Clear(); // Clear the history when starting a new shift
            activityNumber = 0; // Reset activityNumber to 0 at the beginning of a shift
            ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.StartShift, Time = DateTime.Now });
            ResetCounts(); // Reset break and lunch counts
        }

        public void EndShift()
        {
            IsShiftStarted = false;
            IsOnBreak = false;
            IsOnLunch = false;
            ShiftStartTime = DateTime.MinValue;
            ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.EndShift, Time = DateTime.Now });
        }




        public bool TakeBreak()
        {
            if (!IsOnBreak && BreakCount < 3)
            {
                IsOnBreak = true;
                ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.StartBreak, Time = DateTime.Now });
                // Increment BreakCount when a break is started
                BreakCount++;
                return true;
            }
            else if (IsOnBreak)
            {
                IsOnBreak = false;
                ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.EndBreak, Time = DateTime.Now });
            }
            return false;
        }

        

        public bool TakeLunch()
        {
            if (!IsOnLunch && LunchCount < 1)
            {
                IsOnLunch = true;
                ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.StartLunch, Time = DateTime.Now });
                return true;
            }
            else if (IsOnLunch)
            {
                IsOnLunch = false;
                LunchCount++;
                ShiftActivities.Insert(0, new ShiftActivity { Number = activityNumber++, Type = ShiftActivityType.EndLunch, Time = DateTime.Now });
            }
            return false;

        }

        private void ResetCounts()
        {
            BreakCount = -1;
            LunchCount = 0;
        }

        

        
    }

    public enum ShiftActivityType
    {
    [StringValue("Start Shift")]
    StartShift,
    [StringValue("End Shift")]
    EndShift,
    [StringValue("Start Break")]
    StartBreak,
    [StringValue("End Break")]
    EndBreak,
    [StringValue("Start Lunch")]
    StartLunch,
    [StringValue("End Lunch")]
    EndLunch
    }

    public class StringValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].Value : null;

            
        }
    }

    public class ShiftActivity
    {
        public int Number { get; set; }
        public ShiftActivityType Type { get; set; }
        public DateTime Time { get; set; }
    }


}
