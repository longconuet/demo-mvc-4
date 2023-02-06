using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using WebApplication4;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var dbcontext = new StudentDbContext())
{
    String databasename = dbcontext.Database.GetDbConnection().Database;// mydata

    Console.WriteLine("Create database " + databasename);

    bool result = await dbcontext.Database.EnsureCreatedAsync();
    string resultstring = result ? "create db successfully" : "has already exist";
    Console.WriteLine($"CSDL {databasename} : {resultstring}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



