using Lumora.Domain.Constants;
using Lumora.Infrastructure.Extensions;
using Lumora.Infrastructure.Identity;
using Lumora.Web;
using Lumora.Web.Extensions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Localization. No ResourcesPath: the resx embeds as Lumora.Web.SharedResources
// (manifest namespace comes from the SharedResources.cs file next to it), and the
// localizer must look up exactly that name. Setting ResourcesPath would make it
// search Lumora.Web.Resources.SharedResources and silently return raw keys.
builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = LanguageConstants.All
        .Select(code => new CultureInfo(code))
        .ToArray();

    options.DefaultRequestCulture = new RequestCulture(LanguageConstants.Default);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

// MVC with Localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

// Bundling & minification (WebOptimizer). Development serves the raw files for easy
// debugging; outside Development the shared bundles are served minified and the
// per-page css/js files are minified in place (same URLs, no view changes needed).
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddWebOptimizer(minifyJavaScript: false, minifyCss: false);
}
else
{
    builder.Services.AddWebOptimizer((WebOptimizer.IAssetPipeline pipeline) =>
    {
        pipeline.AddCssBundle("/css/shared.min.css", "css/lumora-theme.css", "css/site.css");
        pipeline.AddJavaScriptBundle("/js/shared.min.js", "js/site.js");
        pipeline.MinifyCssFiles("css/pages/**/*.css", "css/components/**/*.css", "css/admin.css");
        pipeline.MinifyJsFiles("js/pages/**/*.js", "js/admin.js");
    });
}

// Must be registered BEFORE AddInfrastructure: AddIdentity uses TryAddScoped for
// IdentityErrorDescriber, so the first registration (ours) wins and UserManager
// picks up the localized descriptions.
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IdentityErrorDescriber,
    Lumora.Web.Services.LocalizedIdentityErrorDescriber>();

// Infrastructure (DB + Repositories + Identity)
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddApplicationServices();

// Live chat (student <-> support team)
builder.Services.AddSignalR();

// Admin accounts are created only from inside the panel, so there is no public
// registration path for them; Student self-registration is the only open sign-up.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminAccess", policy =>
        policy.RequireRole(RoleConstants.AdminRoles));

    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole(RoleConstants.SuperAdmin));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseWebOptimizer(); // must run before UseStaticFiles so bundle URLs are handled
app.UseStaticFiles();
app.UseRouting();

// Localization Middleware (must come before UseAuthentication/UseAuthorization)
var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

// Admin Area Route
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default Public Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<Lumora.Web.Hubs.ChatHub>("/hubs/chat");

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);

    if (app.Environment.IsDevelopment())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<Lumora.Infrastructure.Data.ApplicationDbContext>();
        await Lumora.Infrastructure.Data.DevDataSeeder.SeedAsync(dbContext);
    }
}

app.Run();
