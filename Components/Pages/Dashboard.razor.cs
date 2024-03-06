using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using TrackFlow.Services;
using TrackFlow.Models.dbforall;

namespace TrackFlow.Components.Pages
{
    public partial class Dashboard
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
        protected ShiftService Shift { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        public dbforallService dbforallService { get; set; }

        
        private string currentUserId;

        protected override async Task OnInitializedAsync()
        {
            activityRecord = new TrackFlow.Models.dbforall.ActivityRecord();

            previousRecord = new TrackFlow.Models.dbforall.ActivityRecord();
            
            
            

            aspNetUsersForUserID = await dbforallService.GetAspNetUsers();

            currentUserId = await dbforallService.GetAspNetUserIdByUsername(Security.User.UserName);

            await Shift.LoadUserActivities(currentUserId);

            if (Shift.IsShiftStarted) 
            {
                // Retrieve the most recent user record
                activityRecord = await dbforallService.GetMostRecentUserRecord(currentUserId);
            }

            if (!Shift.IsShiftStarted)
            {
                previousRecord = await dbforallService.GetMostRecentUserRecord(currentUserId);
            }
        }

        protected bool errorVisible;
        protected TrackFlow.Models.dbforall.ActivityRecord activityRecord;
        private ActivityRecord previousRecord;
        protected IEnumerable<TrackFlow.Models.dbforall.AspNetUser> aspNetUsersForUserID;

    }
}