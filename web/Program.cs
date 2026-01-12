using Microsoft.AspNetCore.Identity;
using web.Data;
using web.Models;
using Microsoft.EntityFrameworkCore;
using web.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudyBuddyDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<web.Services.StudyPostCleanupService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StudyBuddyDbContext>();

builder.Services.Configure<web.Services.SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<web.Services.IEmailSender, web.Services.SmtpEmailSender>();

var app = builder.Build();

SeedData.Initialize(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRouting();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V3");
});


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();



app.Run();
