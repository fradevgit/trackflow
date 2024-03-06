using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using TrackFlow.Models.dbforall; // Assuming this namespace contains Shift model

namespace TrackFlow.Components.Pages
{
    [Authorize(Roles = "admin,superadmin")]
    public partial class AddShift : ComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected dbforallService dbforallService { get; set; }

        protected Shift shift = new Shift();
        protected bool errorVisible;
        protected string errorMessage;

        protected async Task FormSubmit()
        {
            try
            {
                // Check if the shift name is provided
                if (string.IsNullOrWhiteSpace(shift.ShiftName))
                {
                    SetErrorMessage("Shift name is required.");
                    return; // Don't save if the shift name is missing
                }

                // Check if the time format is valid for both start and end time
                if (!IsValidTimeFormat(shift.StartTime) || !IsValidTimeFormat(shift.EndTime))
                {
                    SetErrorMessage("Invalid time format. Please use hh:mm:ss format.");
                    return; // Don't save if the format is invalid
                }

                await dbforallService.CreateShift(shift);
                DialogService.Close(shift);
            }
            catch (Exception ex)
            {
                SetErrorMessage("An error occurred while saving the shift.");
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        protected bool IsValidTimeFormat(string time)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(time, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$");
        }

        protected void SetErrorMessage(string message)
        {
            errorVisible = true;
            errorMessage = message;
        }
    }
}
