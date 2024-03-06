using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System.Net.Http;
using TrackFlow.Services;
using Radzen;
using Radzen.Blazor;

namespace TrackFlow.Components.Layout
{
    public partial class MainLayout
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

        private bool sidebarExpanded = true;

        [Inject]
        protected SecurityService Security { get; set; }


        private string btcUsdRate;

        private string ltcUsdRate;

        private string ethUsdRate;

        private bool IsManager = false;

        private bool IsAdmin = false;



        [Inject]
        private ICryptoService cryptoService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            btcUsdRate = await cryptoService.GetBTCUSD();

            ltcUsdRate = await cryptoService.GetLTCUSD();

            ethUsdRate = await cryptoService.GetETHUSD();

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Check if the user is in the "Administrator" role            
            IsManager = user.IsInRole("manager");

            IsAdmin = user.IsInRole("admin");



        }

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
