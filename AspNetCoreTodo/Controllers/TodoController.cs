using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services.Interfaces;
using AspNetCoreTodo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTodo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        // Services injections
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoItemService todoItemService, UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentLooggedInUser = await _userManager.GetUserAsync(User);
            if (currentLooggedInUser == null) return Challenge();
            var items = await _todoItemService.GetIncompleteItemsAsync(currentLooggedInUser);

            var model = new TodoItemViewModel()
            {
                Items = items
            };
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(TodoItem newItem)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser == null) return Challenge();

            var success = await _todoItemService.AddItemAsync(newItem, currentLoggedInUser);
            if (!success)
            {
                return BadRequest("Could not add item");
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }


            var currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser == null) return Challenge();

            var successful = await _todoItemService.MarkDoneAsync(id, currentLoggedInUser);

            if (!successful)
            {
                return BadRequest("Could not mark item as done.");
            }
            return RedirectToAction("Index");
        }

    }
}