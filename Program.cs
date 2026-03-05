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

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();
            builder.Services.AddSingleton<InterviewService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<RubricService>();

            var app = builder.Build();

            // load questions
            var interviewService = app.Services.GetRequiredService<InterviewService>();
            var contentRoot = app.Environment.ContentRootPath;
            await interviewService.LoadQuestionsAsync(
                Path.Combine(contentRoot, "Data/Seed/Behavioral.json"),
                Path.Combine(contentRoot, "Data/Seed/CSharp_technical.json"),
                Path.Combine(contentRoot, "Data/Seed/CSharp_coding.json"));

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
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
