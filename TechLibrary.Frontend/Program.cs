using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TechLibrary.Frontend;
using TechLibrary.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5079/") });



// Registra os serviços customizados
// builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();


// Configura a autenticação via JWT
builder.Services.AddOptions();

await builder.Build().RunAsync();