using ASPIdentityManager;
using ASPIdentityManager.Authorize;
using ASPIdentityManager.Data;
using ASPIdentityManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders().AddDefaultUI();
builder.Services.AddTransient<IEmailSender, MailJetEmailSender>();
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredLength = 5;  
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    opt.Lockout.MaxFailedAccessAttempts = 5;
});
//builder.Services.ConfigureApplicationCookie(opt =>
//{
//    opt.AccessDeniedPath =new Microsoft.AspNetCore.Http.PathString("/Home/AccessDenied");
//});
builder.Services.AddAuthentication().
    AddFacebook(options =>
{
    options.AppId = "1311305543134356";
    options.AppSecret = "e568b3b3873e047d4c46c36560b0da1e";
}).AddGoogle(options =>  
{
    options.ClientId = "822367868054-6sepboso2uepvqnf6jemf0b4ig78jfhs.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserAndAdmin", policy => policy.RequireRole("Admin").RequireRole("User"));
    options.AddPolicy("Admin_createAccess", policy => policy.RequireRole("Admin").RequireClaim("Create","True"));

    options.AddPolicy("Admin_create_edit_deleteAccess", policy => policy.RequireRole("Admin")
    .RequireClaim("Create", "True")
    .RequireClaim("Edit", "True")
    .RequireClaim("Delete", "True"));

    options.AddPolicy("Admin_create_edit_deleteAccess_SuperAdmin", policy => policy.RequireAssertion(context => PolicyLogic.AuthorizeAdminWithCalaimsOrSuperAdmin(context)));

    options.AddPolicy("OnlySuperAdminChecker", policy => policy.Requirements.Add(new OnlySuperAdminChecker()));
    options.AddPolicy("AdminWithMoreThan100Days", policy => policy.Requirements.Add(new AdminWithMoreThan1000days(1000)));
    options.AddPolicy("FirsNameAuth", policy => policy.Requirements.Add(new FirsNameAuthRequirement("Peter")));
});
builder.Services.AddScoped<IAuthorizationHandler, AdminWithOver1000DaysHandler>();
builder.Services.AddScoped<INumberOfDaysForAccount, NumberOfDaysForAccount>();
builder.Services.AddScoped<IAuthorizationHandler, FirsNameAuthHandler>();
builder.Services.AddRazorPages();
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();    
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );
app.MapRazorPages();

app.Run();
