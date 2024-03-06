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
    public partial class Shifts
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

        protected IEnumerable<TrackFlow.Models.dbforall.Shift> shifts;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.Shift> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            shifts = await dbforallService.GetShifts();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddShift>("Add Shift", null);
            await grid0.Reload();
        }

        protected async Task EditRow(TrackFlow.Models.dbforall.Shift args)
        {
            await DialogService.OpenAsync<EditShift>("Edit Shift", new Dictionary<string, object> { {"ShiftID", args.ShiftID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.Shift shift)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteShift(shift.ShiftID);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Shift"
                });
            }
        }
    }
}