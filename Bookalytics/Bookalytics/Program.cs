using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Bookalytics.Data;
using Bookalytics.Data.Models;
using Bookalytics.Seeding;
using Bookalytics.Services;
using Bookalytics.Services.Contracts;
using Bookalytics.ViewModels;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("Bookalytics")));
builder.Services.AddTransient<IBookScrapperService, BookScrapperService>();
builder.Services.AddTransient<IBookAnalyzerService, BookAnalyzerService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Auto Mapper Configurations
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bookalytics API",
        Description = "An ASP.NET Core Web API for books",
    });
});

var app = builder.Build();

//Seeding books on application startup
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbContext.Database.EnsureDeleted();  //REMOVE!!!!!!!!!!!!!!!!!
    dbContext.Database.Migrate();
    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Refactoring endpoints in Minimal API style

app.MapGet("/books",
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
[ProducesResponseType(StatusCodes.Status404NotFound)]
(ApplicationDbContext dbContext,
     IMapper mapper) =>
{
    var books = mapper.Map<IEnumerable<BookViewModel>>(
        dbContext.Books.Take(10).ToList());

    foreach (var book in books)
    {
        book.Text = string.Empty;
    }

    return books;
});

app.MapGet("/books/id/{id}",
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
[ProducesResponseType(StatusCodes.Status404NotFound)]
async (ApplicationDbContext dbContext,
           IMapper mapper,
           int id) =>
{
    var book = mapper.Map<BookViewModel>(
        await dbContext.Books.FirstOrDefaultAsync(x => x.Id == id));

    if (book == null)
    {
        return Results.NotFound();
    }

    book.Text = string.Empty;

    return Results.Ok(book);
});

app.MapPost("/books/id/{id}",
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
async (
    AddBookInputModel input,
    IBookAnalyzerService bookAnalyzerService,
    ApplicationDbContext dbContext,
    IMapper mapper) =>
{
    // No model state validation in .Net 6 minimal APIs!!!
    // You can use some library

    var book = bookAnalyzerService.Analyze(input);

    await dbContext.Books.AddAsync(book);
    await dbContext.SaveChangesAsync();

    var response = mapper.Map<BookViewModel>(book);

    return Results.Ok(response);
});

app.MapPut("books/{id}",
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
async (
    int id,
    UpdateBookInputModel inputModel,
    ApplicationDbContext dbContext) =>
    {
        var originalBook = await dbContext.Books.FindAsync(id);

        if (originalBook == null)
        {
            return Results.BadRequest();
        }

        originalBook.Title = inputModel.Title;
        originalBook.Author = inputModel.Author;
        originalBook.Year = inputModel.Year;
        originalBook.ImageUrl = inputModel.ImageUrl;

        await dbContext.SaveChangesAsync();

        return Results.Ok(originalBook);
    });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Swagger}/{action=Index}/{id?}");

await app.RunAsync();