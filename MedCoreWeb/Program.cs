using MedCore.Data;
using MedCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Workflow;
using Service;
using Service.AI;
using Service.Workflow;

namespace MedCoreWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            builder.Services.AddDbContextPool<MedCoreDbContext>(options =>
            {
                //options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
                options.UseSqlServer(builder.Configuration.GetConnectionString("monster"),
                    sqloptions => sqloptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                        ));
            });
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<MedCoreDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
            });

            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IDoctorRepo, DoctorRepo>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IPatientRepo, PatientRepo>();
            builder.Services.AddScoped<IPatientWorkflowRepo, PatientWorkflowRepo>();
            builder.Services.AddScoped<IDoctorWorkflowRepo, DoctorWorkflowRepo>();
            builder.Services.AddScoped<IAdminWorkflowRepo, AdminWorkflowRepo>();
            builder.Services.AddScoped<IPatientWorkflowService, PatientWorkflowService>();
            builder.Services.AddScoped<IDoctorWorkflowService, DoctorWorkflowService>();
            builder.Services.AddScoped<IAdminWorkflowService, AdminWorkflowService>();

            // AI Services
            builder.Services.Configure<AiAgentModelsOptions>(builder.Configuration.GetSection("AI:Models"));
            builder.Services.AddHttpClient<IOllamaService, OllamaService>();
            builder.Services.AddScoped<IChatAgentService, ChatAgentService>();
            builder.Services.AddScoped<Service.AI.IChatHistoryService, Service.AI.ChatHistoryService>();

            var app = builder.Build();

            // Ensure Identity roles exist
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "Doctor", "Patient", "Admin" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();

            // Security Headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                await next();
            });

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
