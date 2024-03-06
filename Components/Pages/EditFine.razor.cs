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
    public partial class EditFine
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
        public long FineID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            fine = await dbforallService.GetFineByFineId(FineID);

            violationTypesForViolationType = await dbforallService.GetViolationTypes();

            aspNetUsersForUserID = await dbforallService.GetAspNetUsers();
        }
        protected bool errorVisible;
        protected TrackFlow.Models.dbforall.Fine fine;

        protected IEnumerable<TrackFlow.Models.dbforall.ViolationType> violationTypesForViolationType;

        protected IEnumerable<TrackFlow.Models.dbforall.AspNetUser> aspNetUsersForUserID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await dbforallService.UpdateFine(FineID, fine);
                DialogService.Close(fine);
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