using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using RedditBots.Web;
using RedditBots.Web.Data;
using RedditBots.Web.Helpers;
using RedditBots.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<LogService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR().AddMessagePackProtocol();

// In production, the Angular files will be served from this directory
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(builder.Configuration["KeyVault:Uri"]),
        new DefaultAzureCredential());
}

builder.Services.AddDbContext<LogsDbContext>(options => options.UseCosmos(
    accountEndpoint: builder.Configuration["CosmosDb:Account"],
    accountKey: builder.Configuration["cosmosDbKey"],
    databaseName: builder.Configuration["CosmosDb:DatabaseName"]));

builder.Services.AddSingleton(typeof(AppSettings), new AppSettings
{
    ApiKey = builder.Configuration["logsApiKey"]
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetService<LogsDbContext>().Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = (cnt) =>
    {
        cnt.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(app.Environment.IsDevelopment() ? 0 : 60)
        };
    }
});

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseRouting();

app.MapHub<LogHub>("/loghub");
app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start");
    }
});

app.Run();