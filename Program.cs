using ExpenseTracker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// configure file path for storage under <contentroot>/Data/expenses.json
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
var filePath = Path.Combine(dataDir, "expenses.json");

// register the file-based service as a singleton
builder.Services.AddSingleton<IExpenseService>(_ => new FileExpenseService(filePath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Expenses}/{action=Index}/{id?}");

app.Run();
