using Microsoft.EntityFrameworkCore;
using TinyUrlApi;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Get all URLs
app.MapGet("/api/url", async (AppDbContext db) =>
    await db.UrlMappings.ToListAsync());

// Get by Id
app.MapGet("/api/url/{id}", async (int id, AppDbContext db) =>
    await db.UrlMappings.FindAsync(id) is UrlMapping url
        ? Results.Ok(url)
        : Results.NotFound());

// Create Short URL
app.MapPost("/api/url", async (UrlMapping url, AppDbContext db) =>
{
    url.ShortCode = Guid.NewGuid().ToString("N")[..6]; // generate 6-char code
    db.UrlMappings.Add(url);
    await db.SaveChangesAsync();
    return Results.Created($"/api/url/{url.Id}", url);
});

// Delete by Id
app.MapDelete("/api/url/{id}", async (int id, AppDbContext db) =>
{
    var url = await db.UrlMappings.FindAsync(id);
    if (url is null) return Results.NotFound();

    db.UrlMappings.Remove(url);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Redirect by ShortCode
app.MapGet("/{shortURL}", async (string shortCode, AppDbContext db) =>
{
    var url = await db.UrlMappings.FirstOrDefaultAsync(u => u.shortURL == shortURL);
    if (url is null) return Results.NotFound();

    url.Clicks++;
    await db.SaveChangesAsync();

    return Results.Redirect(url.LongUrl);
});

app.Run();
