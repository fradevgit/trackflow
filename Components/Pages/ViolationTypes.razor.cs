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
    public partial class ViolationTypes
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

        protected IEnumerable<TrackFlow.Models.dbforall.ViolationType> violationTypes;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.ViolationType> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            violationTypes = await dbforallService.GetViolationTypes();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddViolationType>("Add ViolationType", null);
            await grid0.Reload();
        }

        protected async Task EditRow(TrackFlow.Models.dbforall.ViolationType args)
        {
            await DialogService.OpenAsync<EditViolationType>("Edit ViolationType", new Dictionary<string, object> { {"ViolationID", args.ViolationID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.ViolationType violationType)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteViolationType(violationType.ViolationID);

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
                    Detail = $"Unable to delete ViolationType"
                });
            }
        }
    }
}