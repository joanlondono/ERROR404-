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

// 1. Configuración del HttpClient apuntando al puerto REAL de la API (Backend)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7087/") });

// 2. Registro de repositorios y servicios esenciales (SIN DUPLICADOS)
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddSweetAlert2();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>();
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>();

await builder.Build().RunAsync();
