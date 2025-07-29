// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlite("Data Source=bookstore.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Models/Book.cs
namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }
        public bool InStock { get; set; }
    }
}

// Data/BookStoreContext.cs
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}

// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreContext _context;

        public BooksController(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
    }
}

// Views/Books/Index.cshtml
@model IEnumerable<BookStore.Models.Book>
<h2>Book List</h2>
<table class="table">
    <thead>
        <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Price</th>
            <th>Genre</th>
            <th>In Stock</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var book in Model)
        {
            <tr>
                <td>@book.Title</td>
                <td>@book.Author</td>
                <td>@book.Price</td>
                <td>@book.Genre</td>
                <td>@(book.InStock ? "Yes" : "No")</td>
            </tr>
        }
    </tbody>
</table>

<a href="/Books/Create" class="btn btn-primary">Add New Book</a>

// Views/Books/Create.cshtml
@model BookStore.Models.Book

<h2>Add New Book</h2>
<form asp-action="Create" method="post">
    <div class="form-group">
        <label>Title</label>
        <input asp-for="Title" class="form-control" />
    </div>
    <div class="form-group">
        <label>Author</label>
        <input asp-for="Author" class="form-control" />
    </div>
    <div class="form-group">
        <label>Price</label>
        <input asp-for="Price" class="form-control" />
    </div>
    <div class="form-group">
        <label>Genre</label>
        <input asp-for="Genre" class="form-control" />
    </div>
    <div class="form-group">
        <label>In Stock</label>
        <input asp-for="InStock" type="checkbox" />
    </div>
    <button type="submit" class="btn btn-success">Save</button>
</form>
