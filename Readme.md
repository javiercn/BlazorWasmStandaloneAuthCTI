# Instructions on how to perform a test
* Make sure you are able to run run the client and server projects, register a new user and authenticate.
* On the server project you will have to run `dotnet ef database update`.
  * You might need to install the tool using `dotnet tool install dotnet-ef --global`

* Once you've verified that you can run the server project
* Create a new Blazor WebAssembly standalone application with authentication
  * Include the following details:
    Authority = "https://localhost:7251"
    ClientId = "BlazorWasmStandaloneAuthCTI.Client"
    DefaultScopes = "BlazorWasmStandaloneAuthCTI.ServerAPI"
    ResponseType = "code"

* Register a message handler as well as an HttpClient instance

builder.Services.AddHttpClient("BlazorWasmStandaloneAuthCTI.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7251"))
    .AddHttpMessageHandler<ServerApiAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWasmStandaloneAuthCTI.ServerAPI"));

* The message handler implementation is as follows

```csharp
public class ServerApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ServerApiAuthorizationMessageHandler(IAccessTokenProvider accessTokenProvider, NavigationManager navigationManager) 
        : base(accessTokenProvider, navigationManager)
    {
        ConfigureHandler(new[] { "https://localhost:7251" });
    }
}
```

* Ensure that the new client project runs in https://localhost:7233 otherwise authentication will fail

* The entire program file needs to look more or less like
```csharp
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
```