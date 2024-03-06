using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using TrackFlow.Data;

namespace TrackFlow.Components.Pages
{
    public partial class EditAspNetUser : ComponentBase
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
        protected IEnumerable<TrackFlow.Models.ApplicationUser> users;
        protected IEnumerable<TrackFlow.Models.ApplicationRole> roles;
        protected TrackFlow.Models.ApplicationUser user;
        protected IEnumerable<string> userRoles;
        [Inject]
        public dbforallService dbforallService { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await Security.GetUserById($"{Id}");
            roles = await Security.GetRoles();
            userRoles = user.Roles.Select(role => role.Id);
            aspNetUser = await dbforallService.GetAspNetUserById(Id);
            teamsForTeamID = await dbforallService.GetTeams();
        }

        protected bool errorVisible;
        protected TrackFlow.Models.dbforall.AspNetUser aspNetUser;
        protected IEnumerable<TrackFlow.Models.dbforall.Team> teamsForTeamID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await dbforallService.UpdateAspNetUser(Id, aspNetUser);

                // Update normalized fields
                await dbforallService.UpdateNormalizedUserName(Id, aspNetUser.UserName.ToUpper());
                await dbforallService.UpdateNormalizedEmail(Id, aspNetUser.Email.ToUpper());

                // Update user roles
                user.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
                await Security.UpdateUser($"{Id}", user);

                DialogService.Close(aspNetUser);
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
