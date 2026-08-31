using ProgressGateway.UI.Helpers;
using ProgressGateway.UI.Services;

var builder =
    WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


// =============================================
// Progress Gateway API Client
// =============================================

builder.Services.AddHttpClient<
    ProgressGatewayClient
>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration[
                    "ProgressGateway:ApiBaseUrl"
                ]
            );
    }
);


// =============================================
// Register Helpers
// =============================================

builder.Services.AddScoped<
    FileProcessingHelper
>();

builder.Services.AddScoped<
    WorkOrderHelper
>();

builder.Services.AddScoped<
    EmployeeOnboardingHelper
>();
builder.Services.AddScoped<ReportGenerationHelper>();

var app =
    builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}"
);


app.Run();