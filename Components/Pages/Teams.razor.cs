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
    public partial class Teams
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

        protected IEnumerable<TrackFlow.Models.dbforall.Team> teams;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.Team> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            teams = await dbforallService.GetTeams(new Query { Expand = "Shift" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddTeam>("Add Team", null);
            await grid0.Reload();
        }

        protected async Task EditRow(TrackFlow.Models.dbforall.Team args)
        {
            await DialogService.OpenAsync<EditTeam>("Edit Team", new Dictionary<string, object> { {"TeamID", args.TeamID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.Team team)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteTeam(team.TeamID);

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
                    Detail = $"Unable to delete Team"
                });
            }
        }
    }
}