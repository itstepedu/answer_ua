// LOCALHOST VERSION

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AnswerUA.Data;
using AnswerUA.Models;
using AnswerUA.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using answer_ua.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddDistributedMemoryCache(); // кеш для сесії
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ANSWER Database
var connectionAnswearString = builder.Configuration.GetConnectionString("AnswerDatabase") ?? throw new InvalidOperationException("Connection string 'AnswerDatabase' not found.");
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite(connectionAnswearString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<EmailSender>();

builder.Services.AddControllersWithViews();

// Stripe
var publishableKey = builder.Configuration["Stripe:PublishableKey"];
var secretKey = builder.Configuration["Stripe:SecretKey"];
Stripe.StripeConfiguration.ApiKey = secretKey;



builder.Services.AddAuthentication()
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

        options.CallbackPath = "/signin-microsoft";

        options.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            var redirectUri = context.RedirectUri.Replace("http://", "https://");
            context.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AnswerUA");

    logger.LogInformation("📡 Incoming Request Info:");
    logger.LogInformation("    Scheme: {Scheme}", context.Request.Scheme);
    logger.LogInformation("    Host: {Host}", context.Request.Host);
    logger.LogInformation("    Path: {Path}", context.Request.Path);
    logger.LogInformation("    QueryString: {Query}", context.Request.QueryString);
    logger.LogInformation("    X-Forwarded-Proto: {XFP}", context.Request.Headers["X-Forwarded-Proto"].ToString());
    logger.LogInformation("    Referer: {Referer}", context.Request.Headers["Referer"].ToString());
    logger.LogInformation("    User-Agent: {UserAgent}", context.Request.Headers["User-Agent"].ToString());

    await next.Invoke();
});



app.UseStaticFiles();

app.UseRouting();

// builder.Services.AddSession();
app.UseSession();


app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string email = "admin_answer@gmail.com";
    string password = "Admin123*";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");

    }

    var users = userManager.Users.ToList();

    foreach (var user in users)
    {
        if (!await userManager.IsInRoleAsync(user, "User") && !await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}

app.Run();


// PRODUCTION VERSION
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using AnswerUA.Data;
// using AnswerUA.Models;
// using AnswerUA.Services;
// using Microsoft.AspNetCore.Identity.UI.Services;
// using answer_ua.Data;
// using Microsoft.AspNetCore.HttpOverrides;

// var builder = WebApplication.CreateBuilder(args);

// builder.Configuration
//     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//     .AddUserSecrets<Program>(optional: true)
//     .AddEnvironmentVariables();

// //робота по https
// builder.Services.Configure<ForwardedHeadersOptions>(options =>
// {
//     options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

//     // дозволяємо всі приватні IP (Docker мережа)
//     options.KnownNetworks.Clear();
//     options.KnownProxies.Clear();
// });

// // Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlite(connectionString));

// // ANSWER Database
// var connectionAnswearString = builder.Configuration.GetConnectionString("AnswerDatabase") ?? throw new InvalidOperationException("Connection string 'AnswerDatabase' not found.");
// builder.Services.AddDbContext<ShopDbContext>(options =>
//     options.UseSqlite(connectionAnswearString));

// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// // IDENTITY
// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddRoles<IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// // Налаштування пошти EmailSettings з env
// builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// builder.Services.PostConfigure<EmailSettings>(options =>
// {
//     options.SmtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? options.SmtpHost;
//     options.SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : options.SmtpPort;
//     options.SmtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? options.SmtpUser;
//     options.SmtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? options.SmtpPass;
//     options.FromEmail = Environment.GetEnvironmentVariable("SMTP_USER") ?? options.FromEmail;
// });

// builder.Services.AddTransient<IEmailSender, EmailSender>();
// builder.Services.AddTransient<EmailSender>();

// builder.Services.AddControllersWithViews();

// // Stripe
// var publishableKey = builder.Configuration["Stripe:PublishableKey"];
// var secretKey = builder.Configuration["Stripe:SecretKey"];
// Stripe.StripeConfiguration.ApiKey = secretKey;

// // OAuth Authentication
// builder.Services.AddAuthentication()
//     .AddMicrosoftAccount(options =>
//     {
//         options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
//         options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

//         options.CallbackPath = "/signin-microsoft";

//         options.Events.OnRedirectToAuthorizationEndpoint = context =>
//         {
//             var redirectUri = context.RedirectUri.Replace("http://", "https://");
//             context.Response.Redirect(redirectUri);
//             return Task.CompletedTask;
//         };
//     })
//     .AddGoogle(options =>
//     {
//         options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//     })
//     .AddFacebook(options =>
//     {
//         options.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
//         options.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
//     });

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseMigrationsEndPoint();
// }
// else
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseForwardedHeaders();

// app.UseHttpsRedirection();

// app.Use(async (context, next) =>
// {
//     var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AnswerUA");

//     logger.LogInformation("📡 Incoming Request Info:");
//     logger.LogInformation("    Scheme: {Scheme}", context.Request.Scheme);
//     logger.LogInformation("    Host: {Host}", context.Request.Host);
//     logger.LogInformation("    Path: {Path}", context.Request.Path);
//     logger.LogInformation("    QueryString: {Query}", context.Request.QueryString);
//     logger.LogInformation("    X-Forwarded-Proto: {XFP}", context.Request.Headers["X-Forwarded-Proto"].ToString());
//     logger.LogInformation("    Referer: {Referer}", context.Request.Headers["Referer"].ToString());
//     logger.LogInformation("    User-Agent: {UserAgent}", context.Request.Headers["User-Agent"].ToString());

//     await next.Invoke();
// });



// app.UseStaticFiles();

// app.UseRouting();


// app.MapControllers();

// app.UseAuthentication();
// app.UseAuthorization();



// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages();

// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//     var roles = new[] { "Admin", "User" };

//     foreach (var role in roles)
//     {
//         if (!await roleManager.RoleExistsAsync(role))
//         {
//             await roleManager.CreateAsync(new IdentityRole(role));
//         }
//     }

// }

// using (var scope = app.Services.CreateScope())
// {
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//     //Адмін логін через .env 
//     string email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin_answer@gmail.com";
//     string password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin123*";

//     if (await userManager.FindByEmailAsync(email) == null)
//     {
//         var user = new ApplicationUser();
//         user.UserName = email;
//         user.Email = email;
//         user.EmailConfirmed = true;

//         await userManager.CreateAsync(user, password);
//         await userManager.AddToRoleAsync(user, "Admin");

//     }

//     var users = userManager.Users.ToList();

//     foreach (var user in users)
//     {
//         if (!await userManager.IsInRoleAsync(user, "User") && !await userManager.IsInRoleAsync(user, "Admin"))
//         {
//             await userManager.AddToRoleAsync(user, "User");
//         }
//     }
// }

// app.Run();
