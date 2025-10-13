using Blazor.IndexedDB;
using DeltaDaily;
using DeltaDaily.Components.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<IDayStoreService, DayStoreService>();
builder.Services.AddScoped<IProjectStoreService, ProjectStoreService>();


await builder.Build().RunAsync();
