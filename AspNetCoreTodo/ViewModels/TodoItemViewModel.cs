using AspNetCoreTodo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.ViewModels
{
    public class TodoItemViewModel
    {
        public TodoItem[] Items { get; set; }
    }
}
