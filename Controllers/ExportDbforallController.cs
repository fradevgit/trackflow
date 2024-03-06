using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using TrackFlow.Data;

namespace TrackFlow.Controllers
{
    public partial class ExportdbforallController : ExportController
    {
        private readonly dbforallContext context;
        private readonly dbforallService service;

        public ExportdbforallController(dbforallContext context, dbforallService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/dbforall/activityrecords/csv")]
        [HttpGet("/export/dbforall/activityrecords/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActivityRecordsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetActivityRecords(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/activityrecords/excel")]
        [HttpGet("/export/dbforall/activityrecords/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActivityRecordsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetActivityRecords(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/fines/csv")]
        [HttpGet("/export/dbforall/fines/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFinesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFines(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/fines/excel")]
        [HttpGet("/export/dbforall/fines/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFinesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFines(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/shifts/csv")]
        [HttpGet("/export/dbforall/shifts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShiftsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetShifts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/shifts/excel")]
        [HttpGet("/export/dbforall/shifts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportShiftsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetShifts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/teams/csv")]
        [HttpGet("/export/dbforall/teams/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTeamsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTeams(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/teams/excel")]
        [HttpGet("/export/dbforall/teams/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTeamsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTeams(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/violationtypes/csv")]
        [HttpGet("/export/dbforall/violationtypes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportViolationTypesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetViolationTypes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/violationtypes/excel")]
        [HttpGet("/export/dbforall/violationtypes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportViolationTypesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetViolationTypes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/aspnetusers/csv")]
        [HttpGet("/export/dbforall/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/dbforall/aspnetusers/excel")]
        [HttpGet("/export/dbforall/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }
    }
}
