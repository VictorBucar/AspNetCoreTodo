using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Services.Implementations
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext _context;
        public TodoItemService(ApplicationDbContext context)
        {
            _context = context;
        }
       

        public async Task<TodoItem[]> GetIncompleteItemsAsync(ApplicationUser user)
        {

            return  await _context.Items
                .Where(x => x.IsDone == false && x.UserId == user.Id)
                .ToArrayAsync();
            //simulate db result
            //var item1 = new TodoItem
            //{
            //    Title = "Learn ASP.NET Core",
            //    DueAt = DateTimeOffset.Now.AddDays(1)
            //};
            //var item2 = new TodoItem
            //{
            //    Title = "Build awesome apps",
            //    DueAt = DateTimeOffset.Now.AddDays(2)
            //};
            //return Task.FromResult(new[] { item1, item2 });
        }


        public async Task<bool> AddItemAsync(TodoItem newItem, ApplicationUser user)
        {
            int saveResult = 0;
            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
                    newItem.Id = Guid.NewGuid();
                    newItem.IsDone = false;
                    newItem.UserId = user.Id;
                    //newItem.DueAt = DateTimeOffset.Now.AddDays(10);

                    _context.Items.Add(newItem);

                    saveResult = await _context.SaveChangesAsync();
                    //transaction.Commit();

            //    }
            //    catch (Exception)
            //    {
            //        return saveResult == 1;
            //    }
            //}

            return saveResult == 1;
        }


        public async Task<bool> MarkDoneAsync(Guid id, ApplicationUser user)
        {
            var item = _context.Items
                .Where(x => x.Id == id && x.UserId == user.Id)
                .SingleOrDefault();

            if (item == null) return false;

            item.IsDone = true;

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
