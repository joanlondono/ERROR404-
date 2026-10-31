using CurrieTechnologies.Razor.SweetAlert2;
using DevWork.WEB;
using DevWork.WEB.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSweetAlert2();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7087") });
builder.Services.AddScoped<IRepository, Repository>();

await builder.Build().RunAsync();
