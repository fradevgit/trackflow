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
    public partial class AddTeam
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

        protected override async Task OnInitializedAsync()
        {
            team = new TrackFlow.Models.dbforall.Team();

            shiftsForShiftID = await dbforallService.GetShifts();
        }
        protected bool errorVisible;
        protected TrackFlow.Models.dbforall.Team team;

        protected IEnumerable<TrackFlow.Models.dbforall.Shift> shiftsForShiftID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await dbforallService.CreateTeam(team);
                DialogService.Close(team);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}