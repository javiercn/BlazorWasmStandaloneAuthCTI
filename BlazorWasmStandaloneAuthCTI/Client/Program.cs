using BlazorWasmStandaloneAuthCTI.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<ServerApiAuthorizationMessageHandler>();

builder.Services.AddHttpClient("BlazorWasmStandaloneAuthCTI.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7251"))
    .AddHttpMessageHandler<ServerApiAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWasmStandaloneAuthCTI.ServerAPI"));

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://localhost:7251";
    options.ProviderOptions.ClientId = "BlazorWasmStandaloneAuthCTI.Client";
    options.ProviderOptions.DefaultScopes.Add("BlazorWasmStandaloneAuthCTI.ServerAPI");
    options.ProviderOptions.ResponseType = "code";
});

await builder.Build().RunAsync();

public class ServerApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ServerApiAuthorizationMessageHandler(IAccessTokenProvider accessTokenProvider, NavigationManager navigationManager) 
        : base(accessTokenProvider, navigationManager)
    {
        ConfigureHandler(new[] { "https://localhost:7251" });
    }
}