using GPVBlazor.Components;
using GPVBlazor.Services.Configuration;

using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
ServiceConfiguration.Configure(builder.Services);

builder.Services.Configure<CircuitOptions>(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.DetailedErrors = false;
});

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

// Add Controllers for API endpoints
builder.Services.AddControllers();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add OpenAPI services
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (document, context, ct) =>
        {
            document.Info.Title = "GPVBlazor API";
            document.Info.Version = "v1";
            document.Info.Description =
                "GitHub Profile Viewer API - Browse and explore GitHub user profiles and repositories";
            document.Info.Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "GPVBlazor",
                Url = new Uri("https://github.com/sametcn99/GPVBlazor"),
            };
            return Task.CompletedTask;
        }
    );
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Disabled for simplified Docker deployment

app.UseStaticFiles();
app.UseAntiforgery();

// Map OpenAPI endpoint
app.MapOpenApi();

// Configure Scalar API Reference at /docs
app.MapScalarApiReference(
    "/docs",
    options =>
    {
        options
            .WithTitle("GPVBlazor API Documentation")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }
);

app.MapStaticAssets();

// Map API Controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
