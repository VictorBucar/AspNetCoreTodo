using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.ViewModels.ManageUsersViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTodo.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageUsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public ManageUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var admins = (await _userManager.GetUsersInRoleAsync("Administrator"))
                .ToArray();

            var everyone = await _userManager.Users.ToArrayAsync();

            var model = new ManageUsersViewModel
            {
                Administrators = admins,
                Everyone = everyone
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();

            var user = await _userManager
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            var rolesForUser = await _userManager.GetRolesAsync(user);


            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if(rolesForUser.Count() > 0)
                {
                    var result = await _userManager.RemoveFromRolesAsync(user, rolesForUser);
                }

                await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}