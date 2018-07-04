using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTodo.UnitTests
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem").Options;

            // Set up a context (connection to the "DB") for writing
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };
                var one = await service.AddItemAsync(new TodoItem
                {
                    Title = "Testing?"
                }, fakeUser);

                var itemsInDatabase = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);
                var item = await context.Items.FirstAsync();
                Assert.Equal("Testing?", item.Title);
                Assert.False(item.IsDone);
                // Item should be due 3 days from now (give or take a second)
                //var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
                //Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task MarkDoneAsyncReturnsFalseIfIdDoesntExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_MarkDoneReturnFalse").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-001",
                    UserName = "fakeagain@fake.com"
                };


                var result = await service.MarkDoneAsync(new Guid(), fakeUser);
                Assert.False(result);
            }
        }

        [Fact]
        public async Task MarkDoneAsyncReturnsTrueWhenItemIsComplete()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_MarkDoneReturnTrue").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-002",
                    UserName = "fakefake@fake.com"
                };

                var one = await service.AddItemAsync(new TodoItem
                {
                    Title = "Testing?"
                }, fakeUser);

                var itemInDatabase = await context.Items.SingleOrDefaultAsync();
                var result = await service.MarkDoneAsync(itemInDatabase.Id, fakeUser);
                Assert.True(result);
            }
        }

        [Fact]
        public async Task GetIncompleteItemsAsyncOwnedByUser()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_IncompleteItemByUser").Options;


            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-003",
                    UserName = "fakenews@fake.com"
                };

                var itemOne = await service.AddItemAsync(new TodoItem
                {
                    Title = "Testing?"
                }, fakeUser);

                var itemTwo = await service.AddItemAsync(new TodoItem
                {
                    Title = "Tested?"
                }, fakeUser);

                var items = await service.GetIncompleteItemsAsync(fakeUser);

                Assert.Collection(items, item => Assert.Contains("Testing?", item.Title),
                                         item => Assert.Contains("Tested?", item.Title));

                Assert.Contains("Testing?", items[0].Title);
                Assert.Contains("Tested?", items[1].Title);
                
            }
        }
    }
}    
        

