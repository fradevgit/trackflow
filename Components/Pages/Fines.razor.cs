using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using OfficeOpenXml;

namespace TrackFlow.Components.Pages
{
    public partial class Fines
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

        protected IEnumerable<TrackFlow.Models.dbforall.Fine> fines;

        protected RadzenDataGrid<TrackFlow.Models.dbforall.Fine> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            fines = await dbforallService.GetFines(new Query { Expand = "ViolationType1,AspNetUser" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddFine>("Add Fine", null);
            await grid0.Reload();
        }

        protected async Task DeleteAllFines(MouseEventArgs args)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete all fines?") == true)
                {
                    await dbforallService.DeleteAllFines();
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete all fines: {ex.Message}"
                });
            }
        }

        protected async Task EditRow(TrackFlow.Models.dbforall.Fine args)
        {
            await DialogService.OpenAsync<EditFine>("Edit Fine", new Dictionary<string, object> { {"FineID", args.FineID} });
        }

        private void ExportFinesToExcel(IEnumerable<TrackFlow.Models.dbforall.Fine> fines)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Fines");

            // Add headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Reference ID";
            worksheet.Cells[1, 3].Value = "Violation";
            worksheet.Cells[1, 4].Value = "User Name";
            worksheet.Cells[1, 5].Value = "Description";

            // Center align headers
            for (int i = 1; i <= 5; i++)
            {
                worksheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Add data
            int row = 2;
            double totalAmount = 0;
            int totalViolations = 0;
            foreach (var fine in fines)
            {
                worksheet.Cells[row, 1].Value = fine.FineID;
                worksheet.Cells[row, 2].Value = fine.RecordID;
                worksheet.Cells[row, 3].Value = fine.ViolationType1.ViolationName;
                worksheet.Cells[row, 4].Value = fine.AspNetUser.UserName;
                worksheet.Cells[row, 5].Value = fine.Description;

                // Center align data
                for (int i = 1; i <= 5; i++)
                {
                    worksheet.Cells[row, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Extract amount from description and add to total amount
                if (!string.IsNullOrEmpty(fine.Description))
                {
                    var descriptionParts = fine.Description.Split('-');
                    if (descriptionParts.Length > 0)
                    {
                        var amountPart = descriptionParts[0].Trim().TrimStart('$');
                        if (double.TryParse(amountPart, out double amount))
                        {
                            totalAmount += amount;
                            totalViolations++;
                        }
                    }
                }

                row++;
            }

            // Add total row
            worksheet.Cells[row, 3].Value = $"Total Fines: ";
            worksheet.Cells[row, 5].Value = $"${totalAmount}";
            worksheet.Cells[row, 4].Value = totalViolations; // Add total number of violations

            // Center align total row
            for (int i = 3; i <= 5; i++)
            {
                worksheet.Cells[row, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Auto fit columns
            worksheet.Cells.AutoFitColumns(0);

            // Save the Excel file to a stream
            var stream = new MemoryStream();
            package.SaveAs(stream);

            // Download the Excel file
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var excelFile = new MemoryStream(stream.ToArray());
            var dataUri = $"data:{contentType};base64,{Convert.ToBase64String(excelFile.ToArray())}";

            // Move navigation outside of the using block
            NavigationManager.NavigateTo(dataUri, true);
        }




        protected async Task GridDeleteButtonClick(MouseEventArgs args, TrackFlow.Models.dbforall.Fine fine)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await dbforallService.DeleteFine(fine.FineID);

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
                    Detail = $"Unable to delete Fine"
                });
            }

            

            
        }
    }
}