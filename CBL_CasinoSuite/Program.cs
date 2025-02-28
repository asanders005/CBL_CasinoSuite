using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using CBL_CasinoSuite.Data.NavConstraints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IDal, Dal>();
builder.Services.AddSingleton<IUser, UserSingleton>();

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

app.MapRazorPages();

app.Run();
