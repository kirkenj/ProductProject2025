using Front;
using Front.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLogging();

builder.Services.ConfigureAuthenticationServices();
builder.Services.ConfigureClients(builder.Configuration);

builder.Services.AddAuthorizationCore();

var app = builder.Build();

await app.RunAsync();