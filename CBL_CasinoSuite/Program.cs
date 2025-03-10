using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using CBL_CasinoSuite.Data.NavConstraints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".99Prcent.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IDal, Dal>();
builder.Services.AddSingleton<IUser, UserSingleton>();

// Add IHttpContextAccessor service
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("pageConstraint", typeof(PageConstraint));
    options.ConstraintMap.Add("leaderboardFilterConstraint", typeof(LeaderboardFilterConstraint));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
