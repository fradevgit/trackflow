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
    public partial class EditApplicationUser
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

        protected IEnumerable<TrackFlow.Models.ApplicationRole> roles;
        protected TrackFlow.Models.ApplicationUser user;
        protected IEnumerable<string> userRoles;
        protected string error;
        protected bool errorVisible;

        [Inject]
        protected TeamService TeamService { get; set; }
        protected IEnumerable<TrackFlow.Models.Team> teams; // Store the list of teams
        protected TrackFlow.Models.Team selectedTeam; // Store the selected team


        [Parameter]
        public string Id { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await Security.GetUserById($"{Id}");

            userRoles = user.Roles.Select(role => role.Id);
            
            teams = await TeamService.GetTeamsAsync(); // Fetch teams from the service

            roles = await Security.GetRoles();
        }

        protected async Task FormSubmit(TrackFlow.Models.ApplicationUser user)
        {
            try
            {
                user.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
                

                await Security.UpdateUser($"{Id}", user);
                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}