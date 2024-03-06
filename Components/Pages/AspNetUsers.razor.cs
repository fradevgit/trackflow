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
    public partial class AspNetUsers
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

        protected IEnumerable<TrackFlow.Models.dbforall.AspNetUser> aspNetUsers;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.AspNetUser> grid0;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            aspNetUsers = await dbforallService.GetAspNetUsers(new Query { Expand = "Team" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAspNetUser>("Add AspNetUser", null);
            await grid0.Reload();
        }

        protected async Task EditRow(TrackFlow.Models.dbforall.AspNetUser args)
        {
            await DialogService.OpenAsync<EditAspNetUser>("Edit AspNetUser", new Dictionary<string, object> { { "Id", args.Id } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.AspNetUser aspNetUser)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteAspNetUser(aspNetUser.Id);

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
                    Detail = $"Unable to delete AspNetUser"
                });
            }
        }

        // Method to handle changes in the UserName field and update the normalized field
        protected void OnUserNameChanged(string newValue)
        {
            foreach (var user in aspNetUsers)
            {
                if (user.UserName == newValue)
                {
                    user.NormalizedUserName = newValue.ToUpper();
                }
            }
        }

        // Method to handle changes in the Email field and update the normalized field
        protected void OnEmailChanged(string newValue)
        {
            foreach (var user in aspNetUsers)
            {
                if (user.Email == newValue)
                {
                    user.NormalizedEmail = newValue.ToUpper();
                }
            }
        }
    }
}
