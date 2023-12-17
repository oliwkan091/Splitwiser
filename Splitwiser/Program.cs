using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splitwiser.EntityFramework;
using Splitwiser.Services;
using Splitwiser.Services.Interfaces;
using AutoMapper;
using Splitwiser;
using Splitwiser.Models.UserEntity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupPaymentHistoryService, GroupPaymentHistoryService>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();
builder.Services.AddScoped<IPaymentMemberEntityService, PaymentMemberEntityService>();
builder.Services.AddScoped<IPaymentInGroupService, PaymentInGroupService>();

//builder.Services.AddAuto(typeof(MappingProfile));

//Dodanie automapper
///////////
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMvc();
///////////////

builder.Services.AddDbContext<SplitwiserDbContext>(builder =>
{
	//builder.UseSqlServer(@"Data Source=DESKTOP-9V0MKJK;Initial Catalog=Groups");
	builder.UseSqlServer(@"Server=DESKTOP-9V0MKJK;Database=Splitwiser;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");
});

builder.Services.AddDbContext<DbSplitwiserAuthContext>(builder =>
{
    builder.UseSqlServer(@"Server=DESKTOP-9V0MKJK;Database=SplitwiserAuth;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");
});

builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 2;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

}).AddEntityFrameworkStores<DbSplitwiserAuthContext>();

//builder.Services.AddAuthentication(options =>
//{
//    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    //options.LoginPath = "/splitwiserAuth/login"; // Set your login page path here
//});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/splitwiserAuth/login";
    //options.start
});

//Response.Headers.Add("Cache-Control", "no-cache, no-store");
//Response.Headers.Add("Pragma", "no-cache");
//Response.Headers.Add("Expires", "-1");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
