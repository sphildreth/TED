using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

builder.Services.AddLogging(loggingBuilder => {
    var loggingSection = configuration.GetSection("Logging");
    loggingBuilder.AddFile(loggingSection);
});

builder.Services.Configure<TED.Models.Configuration>(builder.Configuration.GetSection("TED"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
