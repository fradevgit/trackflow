using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Radzen;

using TrackFlow.Data;

namespace TrackFlow
{
    public partial class dbforallService
    {
        dbforallContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly dbforallContext context;
        private readonly NavigationManager navigationManager;

        public dbforallService(dbforallContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportTeamsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/teams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/teams/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTeamsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/teams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/teams/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTeamsRead(ref IQueryable<TrackFlow.Models.dbforall.Team> items);

        public async Task<IQueryable<TrackFlow.Models.dbforall.Team>> GetTeams(Query query = null)
        {
            var items = Context.Teams.AsQueryable();

            items = items.Include(i => i.Shift);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTeamsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTeamGet(TrackFlow.Models.dbforall.Team item);
        partial void OnGetTeamByTeamId(ref IQueryable<TrackFlow.Models.dbforall.Team> items);


        public async Task<TrackFlow.Models.dbforall.Team> GetTeamByTeamId(long teamid)
        {
            var items = Context.Teams
                              .AsNoTracking()
                              .Where(i => i.TeamID == teamid);

            items = items.Include(i => i.Shift);
 
            OnGetTeamByTeamId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTeamGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTeamCreated(TrackFlow.Models.dbforall.Team item);
        partial void OnAfterTeamCreated(TrackFlow.Models.dbforall.Team item);

        public async Task<TrackFlow.Models.dbforall.Team> CreateTeam(TrackFlow.Models.dbforall.Team team)
        {
            OnTeamCreated(team);

            var existingItem = Context.Teams
                              .Where(i => i.TeamID == team.TeamID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Teams.Add(team);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(team).State = EntityState.Detached;
                throw;
            }

            OnAfterTeamCreated(team);

            return team;
        }

        public async Task<TrackFlow.Models.dbforall.Team> CancelTeamChanges(TrackFlow.Models.dbforall.Team item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTeamUpdated(TrackFlow.Models.dbforall.Team item);
        partial void OnAfterTeamUpdated(TrackFlow.Models.dbforall.Team item);

        public async Task<TrackFlow.Models.dbforall.Team> UpdateTeam(long teamid, TrackFlow.Models.dbforall.Team team)
        {
            OnTeamUpdated(team);

            var itemToUpdate = Context.Teams
                              .Where(i => i.TeamID == team.TeamID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(team);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTeamUpdated(team);

            return team;
        }

        partial void OnTeamDeleted(TrackFlow.Models.dbforall.Team item);
        partial void OnAfterTeamDeleted(TrackFlow.Models.dbforall.Team item);

        public async Task<TrackFlow.Models.dbforall.Team> DeleteTeam(long teamid)
        {
            var itemToDelete = Context.Teams
                              .Where(i => i.TeamID == teamid)
                              .Include(i => i.AspNetUsers)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTeamDeleted(itemToDelete);


            Context.Teams.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTeamDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUsersRead(ref IQueryable<TrackFlow.Models.dbforall.AspNetUser> items);

        public async Task<IQueryable<TrackFlow.Models.dbforall.AspNetUser>> GetAspNetUsers(Query query = null)
        {
            var items = Context.AspNetUsers.AsQueryable();

            items = items.Include(i => i.Team);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserGet(TrackFlow.Models.dbforall.AspNetUser item);
        partial void OnGetAspNetUserById(ref IQueryable<TrackFlow.Models.dbforall.AspNetUser> items);

        public async Task<string> GetAspNetUserIdByUsername(string username)
        {
            var user = await Context.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == username);
            return user?.Id;
        }
        public async Task<TrackFlow.Models.dbforall.AspNetUser> GetAspNetUserById(string id)
        {
            var items = Context.AspNetUsers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Team);
 
            OnGetAspNetUserById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserCreated(TrackFlow.Models.dbforall.AspNetUser item);
        partial void OnAfterAspNetUserCreated(TrackFlow.Models.dbforall.AspNetUser item);

        public async Task<TrackFlow.Models.dbforall.AspNetUser> CreateAspNetUser(TrackFlow.Models.dbforall.AspNetUser aspnetuser)
        {
            OnAspNetUserCreated(aspnetuser);

            var existingItem = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUsers.Add(aspnetuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuser).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserCreated(aspnetuser);

            return aspnetuser;
        }

        public async Task<TrackFlow.Models.dbforall.AspNetUser> CancelAspNetUserChanges(TrackFlow.Models.dbforall.AspNetUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserUpdated(TrackFlow.Models.dbforall.AspNetUser item);
        partial void OnAfterAspNetUserUpdated(TrackFlow.Models.dbforall.AspNetUser item);

        public async Task<TrackFlow.Models.dbforall.AspNetUser> UpdateAspNetUser(string id, TrackFlow.Models.dbforall.AspNetUser aspnetuser)
        {
            OnAspNetUserUpdated(aspnetuser);

            var itemToUpdate = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserUpdated(aspnetuser);

            return aspnetuser;
        }

        public async Task UpdateNormalizedUserName(string id, string normalizedUserName)
        {
            var userToUpdate = await Context.AspNetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (userToUpdate != null)
            {
                userToUpdate.NormalizedUserName = normalizedUserName;
                await Context.SaveChangesAsync();
            }
        }

        public async Task UpdateNormalizedEmail(string id, string normalizedEmail)
        {
            var userToUpdate = await Context.AspNetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (userToUpdate != null)
            {
                userToUpdate.NormalizedEmail = normalizedEmail;
                await Context.SaveChangesAsync();
            }
        }


        partial void OnAspNetUserDeleted(TrackFlow.Models.dbforall.AspNetUser item);
        partial void OnAfterAspNetUserDeleted(TrackFlow.Models.dbforall.AspNetUser item);

        public async Task<TrackFlow.Models.dbforall.AspNetUser> DeleteAspNetUser(string id)
        {
            var itemToDelete = Context.AspNetUsers
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserDeleted(itemToDelete);


            Context.AspNetUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserDeleted(itemToDelete);

            return itemToDelete;
        }
         public async Task ExportShiftsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/shifts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/shifts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportShiftsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/shifts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/shifts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnShiftsRead(ref IQueryable<TrackFlow.Models.dbforall.Shift> items);

        public async Task<IQueryable<TrackFlow.Models.dbforall.Shift>> GetShifts(Query query = null)
        {
            var items = Context.Shifts.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnShiftsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnShiftGet(TrackFlow.Models.dbforall.Shift item);
        partial void OnGetShiftByShiftId(ref IQueryable<TrackFlow.Models.dbforall.Shift> items);


        public async Task<TrackFlow.Models.dbforall.Shift> GetShiftByShiftId(long shiftid)
        {
            var items = Context.Shifts
                              .AsNoTracking()
                              .Where(i => i.ShiftID == shiftid);

 
            OnGetShiftByShiftId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnShiftGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnShiftCreated(TrackFlow.Models.dbforall.Shift item);
        partial void OnAfterShiftCreated(TrackFlow.Models.dbforall.Shift item);

        public async Task<TrackFlow.Models.dbforall.Shift> CreateShift(TrackFlow.Models.dbforall.Shift shift)
        {
            OnShiftCreated(shift);

            var existingItem = Context.Shifts
                              .Where(i => i.ShiftID == shift.ShiftID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Shifts.Add(shift);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(shift).State = EntityState.Detached;
                throw;
            }

            OnAfterShiftCreated(shift);

            return shift;
        }

        public async Task<TrackFlow.Models.dbforall.Shift> CancelShiftChanges(TrackFlow.Models.dbforall.Shift item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnShiftUpdated(TrackFlow.Models.dbforall.Shift item);
        partial void OnAfterShiftUpdated(TrackFlow.Models.dbforall.Shift item);

        

        public async Task<TrackFlow.Models.dbforall.Shift> UpdateShift(long shiftid, TrackFlow.Models.dbforall.Shift shift)
        {
            OnShiftUpdated(shift);

            var itemToUpdate = Context.Shifts
                              .Where(i => i.ShiftID == shift.ShiftID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(shift);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterShiftUpdated(shift);

            return shift;
        }

        partial void OnShiftDeleted(TrackFlow.Models.dbforall.Shift item);
        partial void OnAfterShiftDeleted(TrackFlow.Models.dbforall.Shift item);

        public async Task<TrackFlow.Models.dbforall.Shift> DeleteShift(long shiftid)
        {
            var itemToDelete = Context.Shifts
                              .Where(i => i.ShiftID == shiftid)
                              .Include(i => i.Teams)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnShiftDeleted(itemToDelete);


            Context.Shifts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterShiftDeleted(itemToDelete);

            return itemToDelete;
        }
                public async Task ExportViolationTypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/violationtypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/violationtypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportViolationTypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/violationtypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/violationtypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnViolationTypesRead(ref IQueryable<TrackFlow.Models.dbforall.ViolationType> items);

        public async Task<IQueryable<TrackFlow.Models.dbforall.ViolationType>> GetViolationTypes(Query query = null)
        {
            var items = Context.ViolationTypes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnViolationTypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnViolationTypeGet(TrackFlow.Models.dbforall.ViolationType item);
        partial void OnGetViolationTypeByViolationId(ref IQueryable<TrackFlow.Models.dbforall.ViolationType> items);


        public async Task<TrackFlow.Models.dbforall.ViolationType> GetViolationTypeByViolationId(long violationid)
        {
            var items = Context.ViolationTypes
                              .AsNoTracking()
                              .Where(i => i.ViolationID == violationid);

 
            OnGetViolationTypeByViolationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnViolationTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnViolationTypeCreated(TrackFlow.Models.dbforall.ViolationType item);
        partial void OnAfterViolationTypeCreated(TrackFlow.Models.dbforall.ViolationType item);

        public async Task<TrackFlow.Models.dbforall.ViolationType> CreateViolationType(TrackFlow.Models.dbforall.ViolationType violationtype)
        {
            OnViolationTypeCreated(violationtype);

            var existingItem = Context.ViolationTypes
                              .Where(i => i.ViolationID == violationtype.ViolationID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ViolationTypes.Add(violationtype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(violationtype).State = EntityState.Detached;
                throw;
            }

            OnAfterViolationTypeCreated(violationtype);

            return violationtype;
        }

        public async Task<TrackFlow.Models.dbforall.ViolationType> CancelViolationTypeChanges(TrackFlow.Models.dbforall.ViolationType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnViolationTypeUpdated(TrackFlow.Models.dbforall.ViolationType item);
        partial void OnAfterViolationTypeUpdated(TrackFlow.Models.dbforall.ViolationType item);

        public async Task<TrackFlow.Models.dbforall.ViolationType> UpdateViolationType(long violationid, TrackFlow.Models.dbforall.ViolationType violationtype)
        {
            OnViolationTypeUpdated(violationtype);

            var itemToUpdate = Context.ViolationTypes
                              .Where(i => i.ViolationID == violationtype.ViolationID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(violationtype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterViolationTypeUpdated(violationtype);

            return violationtype;
        }

        partial void OnViolationTypeDeleted(TrackFlow.Models.dbforall.ViolationType item);
        partial void OnAfterViolationTypeDeleted(TrackFlow.Models.dbforall.ViolationType item);

        public async Task<TrackFlow.Models.dbforall.ViolationType> DeleteViolationType(long violationid)
        {
            var itemToDelete = Context.ViolationTypes
                              .Where(i => i.ViolationID == violationid)
                              .Include(i => i.Fines)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnViolationTypeDeleted(itemToDelete);


            Context.ViolationTypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterViolationTypeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportFinesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/fines/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/fines/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFinesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/fines/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/fines/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFinesRead(ref IQueryable<TrackFlow.Models.dbforall.Fine> items);

        public async Task<IQueryable<TrackFlow.Models.dbforall.Fine>> GetFines(Query query = null)
        {
            var items = Context.Fines.AsQueryable();

            items = items.Include(i => i.AspNetUser);
            items = items.Include(i => i.ViolationType1);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnFinesRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<IQueryable<TrackFlow.Models.dbforall.Fine>> GetUserFines(string userId, Query query = null)
        {
            var items = Context.Fines
                .Where(f => f.UserID == userId) // Filter fines for the current user
                .AsQueryable(); // Convert to IQueryable<T>

            if (query != null)
            {
                // Specify the type arguments explicitly when calling ApplyQuery
                ApplyQuery<TrackFlow.Models.dbforall.Fine>(ref items, query);
            }

            OnFinesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFineGet(TrackFlow.Models.dbforall.Fine item);
        partial void OnGetFineByFineId(ref IQueryable<TrackFlow.Models.dbforall.Fine> items);


        public async Task<TrackFlow.Models.dbforall.Fine> GetFineByFineId(long fineid)
        {
            var items = Context.Fines
                              .AsNoTracking()
                              .Where(i => i.FineID == fineid);

            items = items.Include(i => i.AspNetUser);
            items = items.Include(i => i.ViolationType1);
 
            OnGetFineByFineId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFineGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFineCreated(TrackFlow.Models.dbforall.Fine item);
        partial void OnAfterFineCreated(TrackFlow.Models.dbforall.Fine item);

        public async Task<TrackFlow.Models.dbforall.Fine> CreateFine(TrackFlow.Models.dbforall.Fine fine)
        {
            OnFineCreated(fine);

            var existingItem = Context.Fines
                              .Where(i => i.FineID == fine.FineID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Fines.Add(fine);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(fine).State = EntityState.Detached;
                throw;
            }

            OnAfterFineCreated(fine);

            return fine;
        }

        public async Task<TrackFlow.Models.dbforall.Fine> CancelFineChanges(TrackFlow.Models.dbforall.Fine item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFineUpdated(TrackFlow.Models.dbforall.Fine item);
        partial void OnAfterFineUpdated(TrackFlow.Models.dbforall.Fine item);

        public async Task<TrackFlow.Models.dbforall.Fine> UpdateFine(long fineid, TrackFlow.Models.dbforall.Fine fine)
        {
            OnFineUpdated(fine);

            var itemToUpdate = Context.Fines
                              .Where(i => i.FineID == fine.FineID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(fine);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFineUpdated(fine);

            return fine;
        }

        partial void OnFineDeleted(TrackFlow.Models.dbforall.Fine item);
        partial void OnAfterFineDeleted(TrackFlow.Models.dbforall.Fine item);

        public async Task<TrackFlow.Models.dbforall.Fine> DeleteFine(long fineid)
        {
            var itemToDelete = Context.Fines
                              .Where(i => i.FineID == fineid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFineDeleted(itemToDelete);


            Context.Fines.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFineDeleted(itemToDelete);

            return itemToDelete;
        }
        public async Task ExportActivityRecordsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/activityrecords/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/activityrecords/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportActivityRecordsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dbforall/activityrecords/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dbforall/activityrecords/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnActivityRecordsRead(ref IQueryable<TrackFlow.Models.dbforall.ActivityRecord> items);


        public async Task<IQueryable<TrackFlow.Models.dbforall.ActivityRecord>> GetActivityRecordsForUserInMonth(string userId, int year, int month, Query query = null)
        {
            var items = Context.ActivityRecords
                                .Where(ar => ar.AspNetUser.Id == userId && 
                                            !string.IsNullOrEmpty(ar.ShiftStartTime)) // Filtering by user ID and non-null ShiftStartTime
                                .AsQueryable();

            // Filter by year and month outside of the LINQ query
            var filteredItems = items.ToList().Where(ar => 
                                        DateTime.Parse(ar.ShiftStartTime).Year == year &&
                                        DateTime.Parse(ar.ShiftStartTime).Month == month)
                                        .AsQueryable();

            filteredItems = filteredItems.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        filteredItems = filteredItems.Include(p.Trim());
                    }
                }

                ApplyQuery(ref filteredItems, query);
            }

            OnActivityRecordsRead(ref filteredItems);

            return filteredItems;
        }




        public async Task<IQueryable<TrackFlow.Models.dbforall.ActivityRecord>> GetActivityRecords(Query query = null)
        {
            var items = Context.ActivityRecords.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnActivityRecordsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnActivityRecordGet(TrackFlow.Models.dbforall.ActivityRecord item);
        partial void OnGetActivityRecordByActivityId(ref IQueryable<TrackFlow.Models.dbforall.ActivityRecord> items);


        public async Task<TrackFlow.Models.dbforall.ActivityRecord> GetActivityRecordByActivityId(long activityid)
        {
            var items = Context.ActivityRecords
                              .AsNoTracking()
                              .Where(i => i.ActivityID == activityid);

            items = items.Include(i => i.AspNetUser);
 
            OnGetActivityRecordByActivityId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnActivityRecordGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnActivityRecordCreated(TrackFlow.Models.dbforall.ActivityRecord item);
        partial void OnAfterActivityRecordCreated(TrackFlow.Models.dbforall.ActivityRecord item);

        public async Task<TrackFlow.Models.dbforall.ActivityRecord> CreateActivityRecord(TrackFlow.Models.dbforall.ActivityRecord activityrecord)
        {
            OnActivityRecordCreated(activityrecord);

            var existingItem = Context.ActivityRecords
                              .Where(i => i.ActivityID == activityrecord.ActivityID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ActivityRecords.Add(activityrecord);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(activityrecord).State = EntityState.Detached;
                throw;
            }

            OnAfterActivityRecordCreated(activityrecord);

            return activityrecord;
        }

        public async Task<TrackFlow.Models.dbforall.ActivityRecord> CancelActivityRecordChanges(TrackFlow.Models.dbforall.ActivityRecord item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnActivityRecordUpdated(TrackFlow.Models.dbforall.ActivityRecord item);
        partial void OnAfterActivityRecordUpdated(TrackFlow.Models.dbforall.ActivityRecord item);

        public async Task<TrackFlow.Models.dbforall.ActivityRecord> UpdateActivityRecord(long activityid, TrackFlow.Models.dbforall.ActivityRecord activityrecord)
        {
            OnActivityRecordUpdated(activityrecord);

            var itemToUpdate = Context.ActivityRecords
                              .Where(i => i.ActivityID == activityrecord.ActivityID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(activityrecord);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterActivityRecordUpdated(activityrecord);

            return activityrecord;
        }

        partial void OnActivityRecordDeleted(TrackFlow.Models.dbforall.ActivityRecord item);
        partial void OnAfterActivityRecordDeleted(TrackFlow.Models.dbforall.ActivityRecord item);

        public async Task<TrackFlow.Models.dbforall.ActivityRecord> DeleteActivityRecord(long activityid)
        {
            var itemToDelete = Context.ActivityRecords
                              .Where(i => i.ActivityID == activityid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnActivityRecordDeleted(itemToDelete);


            Context.ActivityRecords.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterActivityRecordDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task DeleteAllActivityRecords()
        {
            try
            {
                var allActivityRecords = Context.ActivityRecords.ToList();

                foreach (var activityRecord in allActivityRecords)
                {
                    Context.ActivityRecords.Remove(activityRecord);
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting all activity records: {ex.Message}");
                throw;
            }
        }

        public async Task<(long? ShiftId, DateTime? StartTime, DateTime? EndTime)> GetShiftDetailsByUserId(string userId)
        {
            var user = await Context.AspNetUsers
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Include(u => u.Team)
                    .ThenInclude(t => t.Shift)
                .FirstOrDefaultAsync();

            var shiftId = user?.Team?.Shift?.ShiftID;
            // Assuming startTime and endTime are strings in the format "HH:mm:ss"
            string startTimeString = user?.Team?.Shift?.StartTime;
            string endTimeString = user?.Team?.Shift?.EndTime;

            // Parse the startTimeString and endTimeString to DateTime objects
            DateTime? startTime = null;
            DateTime? endTime = null;

            if (!string.IsNullOrEmpty(startTimeString))
            {
                startTime = DateTime.ParseExact(startTimeString, "HH:mm:ss", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endTimeString))
            {
                endTime = DateTime.ParseExact(endTimeString, "HH:mm:ss", CultureInfo.InvariantCulture);
                // Check if endTime is before startTime, indicating a shift that spans across midnight
                if (endTime < startTime)
                {
                    // Shift spans across midnight, and current time is after midnight but before the end time
                    if (DateTime.Now.TimeOfDay < endTime.Value.TimeOfDay)
                    {
                        // Adjust startTime by subtracting a day
                        startTime = startTime.Value.AddDays(-1);
                    }
                    // Shift spans across midnight, and current time is after the end time
                    else
                    {
                        // Adjust endTime by adding a day
                        endTime = endTime.Value.AddDays(1);
                    }
                }
            }

            return (shiftId, startTime, endTime);  
        }
        public async Task<string> GetViolationDescriptionById(long violationId)
        {
            var violationType = await Context.ViolationTypes
                .AsNoTracking()
                .Where(v => v.ViolationID == violationId)
                .Select(v => v.Description)
                .FirstOrDefaultAsync();

            return violationType;
        }
        public async Task<TrackFlow.Models.dbforall.ActivityRecord> GetMostRecentUserRecord(string UserId)
        {
            // Corrected UserID property is used in the LINQ query
            var mostRecentRecord = await Context.ActivityRecords
                .Where(ar => ar.UserID == UserId) // Note: 'UserID' matches the case of the property in 'ActivityRecord'
                .OrderByDescending(ar => ar.ActivityID) // Assuming 'ActivityID' is a suitable proxy for recency
                .FirstOrDefaultAsync(); // Retrieves the first result, or null if no matches are found

            return mostRecentRecord;
        }

        public async Task DeleteAllFines()
        {
            try
            {
                var allFines = Context.Fines.ToList();

                foreach (var fine in allFines)
                {
                    Context.Fines.Remove(fine);
                }

                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete all fines", ex);
            }
        }

        




        }
    }
