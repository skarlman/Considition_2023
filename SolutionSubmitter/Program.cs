using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Mvc;
using Shared.Game;
using SolutionSubmitter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Api>(new Api(new HttpClient(), false));
builder.Services.AddSingleton<SolutionProcessor>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();


GlobalConfiguration.Configuration.UseSQLiteStorage();
builder.Services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage());
builder.Services.AddHangfireServer();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.MapPost("/api/Game/submitSolution", async ([FromQuery] string mapName, [FromBody] SubmitSolution solution) =>
//{
//    // Your logic to handle the solution submission
//    await solutionProcessor.ProcessSubmissionAsync(mapName, solution);
//    // Respond with the GameData object
//    return Results.Ok;
//});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseHangfireDashboard();
BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));


app.MapRazorPages();
//app.MapControllerRoute("default", "api/{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

var sumbissionApi = new Api(new HttpClient());
var solutionProcessor = new SolutionSubmitter.SolutionProcessor(sumbissionApi);



app.Run();



