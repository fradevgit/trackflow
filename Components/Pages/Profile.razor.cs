using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using TrackFlow.Models.dbforall;

namespace TrackFlow.Components.Pages
{
    public partial class Profile
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected dbforallService dbService { get; set; }
    

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        List<Fine> fines = []; // Initialize the list


        protected string oldPassword = "";
        protected string newPassword = "";
        protected string confirmPassword = "";
        protected TrackFlow.Models.ApplicationUser user;
        protected string error;
        protected bool errorVisible;
        protected bool successVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()

        {

            await base.OnInitializedAsync();
            var userId = await dbService.GetAspNetUserIdByUsername(Security.User.UserName);
            fines = (await dbService.GetUserFines(userId)).ToList();
            user = await Security.GetUserById($"{Security.User.Id}");
        }

        private string GetFineAmount(string description)
        {
            var parts = description.Split(new[] { " - " }, StringSplitOptions.None);
            if (parts.Length > 0)
            {
                return parts[0];
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetFineDate(string description)
        {
            var parts = description.Split(new[] { " - " }, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                var dateString = parts.LastOrDefault(); // Get the last part which should be the date
                // Parse the date and format it as dd.MM.yyyy
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    return $"{date:dd.MM.yyyy}";
                }
            }
            return string.Empty;
        }

        private string GetFineDescription(string description)
        {
            var parts = description.Split(new[] { " - " }, StringSplitOptions.None);
            return parts.Length > 1 ? parts[1] : string.Empty;
        }

        protected async Task FormSubmit()
        {
            try
            {
                await Security.ChangePassword(oldPassword, newPassword);
                successVisible = true;
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }

            
        }
    }
}