using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AnswerUA.Data;
using AnswerUA.Models;
using AnswerUA.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using answer_ua.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>

//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    options.UseSqlite(connectionString));

// ANSWER Database
var connectionAnswearString = builder.Configuration.GetConnectionString("AnswerDatabase") ?? throw new InvalidOperationException("Connection string 'AnswerDatabase' not found.");
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite(connectionAnswearString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
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

// app.Use(async (context, next) =>
// {
//     var scheme = context.Request.Scheme;
//     var host = context.Request.Host;
//     var path = context.Request.Path;
//     var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();
//     var forwardedHost = context.Request.Headers["X-Forwarded-Host"].ToString();
//     var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
//     var referer = context.Request.Headers["Referer"].ToString();
//     var userAgent = context.Request.Headers["User-Agent"].ToString();

//     app.Logger.LogInformation("📡 Incoming Request Info:");
//     app.Logger.LogInformation("  Scheme: {Scheme}", scheme);
//     app.Logger.LogInformation("  Host: {Host}", host);
//     app.Logger.LogInformation("  Path: {Path}", path);
//     app.Logger.LogInformation("  X-Forwarded-Proto: {Proto}", forwardedProto);
//     app.Logger.LogInformation("  X-Forwarded-Host: {FHost}", forwardedHost);
//     app.Logger.LogInformation("  X-Forwarded-For: {FFor}", forwardedFor);
//     app.Logger.LogInformation("  Referer: {Referer}", referer);
//     app.Logger.LogInformation("  User-Agent: {UA}", userAgent);

//     app.Logger.LogInformation("Request: {Scheme}://{Host}, X-Forwarded-Proto={ForwardedProto}",
//     scheme, host, forwardedProto);

//     await next();
// });

// app.Use(async (context, next) =>
// {
//     var scheme = context.Request.Scheme;
//     var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();
//     var host = context.Request.Host;

//     app.Logger.LogInformation("Request: {Scheme}://{Host}, X-Forwarded-Proto={ForwardedProto}",
//         scheme, host, forwardedProto);

//     await next();
// });



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
