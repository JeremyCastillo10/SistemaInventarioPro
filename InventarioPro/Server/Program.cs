using InventarioPro.Server.DAL;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Contexto>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseSwaggerUI();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.UseSwagger();


app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
