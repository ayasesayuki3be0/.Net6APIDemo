using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyDb>(opt => opt.UseInMemoryDatabase("MyDbList"));

//var connectionString = builder.Configuration.GetConnectionString("MyDb") ?? "Data Source=MyDb.db";
//builder.Services.AddDbContext<MyDb>(opt => opt.UseSqlite(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
app.MapGet("/", () => "Hello ASP.NET Core WebApplication");

app.MapPost("/addschool", async (School school, MyDb db) =>
{
    db.Schools.Add(school);
    await db.SaveChangesAsync();
    return Results.Created($"/addschool/{school.ID}", school);

});

app.MapGet("/schools", async (MyDb db) => await db.Schools.ToListAsync());

app.MapGet("/findschool/{ID}", async(int ID,MyDb db)=>await db.Schools.FindAsync(ID)is School school ? Results.Ok(school):Results.NotFound());

app.MapPut("/editschool/{ID}", async(int ID,School school,MyDb db)=>
{
var oschool = await db.Schools.FindAsync(ID);
    if(oschool==null)
         return Results.NotFound();
        oschool.Logo =school.Logo;
        oschool.Address=school.Address;
        oschool.Email=school.Email;
        oschool.Name=school.Name;
        oschool.Tel=school.Tel;
    await db.SaveChangesAsync();
    return Results.NoContent();
}
);
app.MapDelete("removeschool/{ID}",async(int ID,MyDb db)
=>{
var oschool = await db.Schools.FindAsync(ID);
if(oschool == null)
    return Results.NotFound();
 db.Schools.Remove(oschool);
 await db.SaveChangesAsync();
 return Results.NoContent();
}
);

app.Run();


public partial class Program { }