using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace TrackFlow.Components.Pages
{
    public partial class EditShift
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        
        [Inject]
        public dbforallService dbforallService { get; set; }

        [Parameter]
        public long ShiftID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            shift = await dbforallService.GetShiftByShiftId(ShiftID);
        }

        protected string errorMessage; // Updated to string type
        private bool errorVisible = false;
        protected TrackFlow.Models.dbforall.Shift shift;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                // Check if the shift name is provided
                if (string.IsNullOrWhiteSpace(shift.ShiftName))
                {
                    errorMessage = "Please enter a Shift Name";
                    errorVisible = true;
                    return; // Don't save if the shift name is missing
                }

                // Check if the time format is valid for both start and end time
                if (!IsValidTimeFormat(shift.StartTime))
                {
                    errorMessage = "Please enter a valid format for Start Time (hh:mm:ss)";
                    errorVisible = true;
                    return; // Don't save if the format is invalid
                }

                if (!IsValidTimeFormat(shift.EndTime))
                {
                    errorMessage = "Please enter a valid format for End Time (hh:mm:ss)";
                    errorVisible = true;
                    return; // Don't save if the format is invalid
                }

                await dbforallService.UpdateShift(ShiftID, shift);
                DialogService.Close(shift);
            }
            catch (Exception ex)
            {
                errorMessage = "Cannot save Shift";
                errorVisible = true;
            }
        }


        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        // Validate time format as hh:mm:ss
        protected bool IsValidTimeFormat(string time)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(time, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$");
        }
    }
}