global using MiniProject_Take1.Domain.Enums;
using MiniProject_Take1.Components;
using MiniProject_Take1.Services;

namespace MiniProject_Take1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSingleton<InterviewService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<RubricService>();

            // add this block after building the app
            var app = builder.Build();

            // load questions
            var interviewService = app.Services.GetRequiredService<InterviewService>();
            var questionsPath = Path.Combine(app.Environment.ContentRootPath, "Data", "Seed", "Top20Behavioral.json");
            await interviewService.LoadQuestionsAsync(questionsPath);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
