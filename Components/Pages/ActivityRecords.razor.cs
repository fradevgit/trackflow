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
    public partial class ActivityRecords
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

        protected IEnumerable<TrackFlow.Models.dbforall.ActivityRecord> activityRecords;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.ActivityRecord> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            activityRecords = await dbforallService.GetActivityRecords(new Query { Expand = "AspNetUser" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddActivityRecord>("Add ActivityRecord", null);
            await grid0.Reload();
        }

        

        protected async Task EditRow(TrackFlow.Models.dbforall.ActivityRecord args)
        {
            await DialogService.OpenAsync<EditActivityRecord>("Edit ActivityRecord", new Dictionary<string, object> { {"ActivityID", args.ActivityID} });
        }


        protected async Task DeleteAllActivityRecords()
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete all records?") == true)
                {
                    await dbforallService.DeleteAllActivityRecords();
                    await grid0.Reload();
                }                

                // Optionally, reload the grid or perform any other necessary actions after deletion
                await grid0.Reload();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the deletion process
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete all Activity Records: {ex.Message}"
                });
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.ActivityRecord activityRecord)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteActivityRecord(activityRecord.ActivityID);

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
                    Detail = $"Unable to delete ActivityRecord"
                });
            }
        }
    }
}