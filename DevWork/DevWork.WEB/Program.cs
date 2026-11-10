using CurrieTechnologies.Razor.SweetAlert2;
using DevWork.WEB;
using DevWork.WEB.Pages.Auth;
using DevWork.WEB.Repositories;
using DevWork.WEB.Services; 
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7031/") });
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddSweetAlert2();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7031";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<IRepository, Repository>();

await builder.Build().RunAsync();
