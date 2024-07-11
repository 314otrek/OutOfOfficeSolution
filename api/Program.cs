using api.Controllers;
using api.Data;
using api.Repositories;
using api.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();
builder.Services.AddScoped<ApprovalRequestService>();
builder.Services.AddScoped<LeaveRequestService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ProjectService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1-pr-manager", new OpenApiInfo { Title = "PROJECT_MANAGER API", Version = "v1" });
    c.SwaggerDoc("v1-hr", new OpenApiInfo { Title = "HR_MANAGER API", Version = "v1" });
    c.SwaggerDoc("v1-employee", new OpenApiInfo { Title = "EMPLOYEE API", Version = "v1" });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var swaggerGroupsAttribute = controllerActionDescriptor.MethodInfo
                .GetCustomAttributes(true)
                .OfType<SwaggerGroupAttribute>()
                .FirstOrDefault();

            if (swaggerGroupsAttribute != null)
            {
                return swaggerGroupsAttribute.GroupNames.Any(g => docName.Contains(g, StringComparison.OrdinalIgnoreCase));
            }
        }
        return false;
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedDataAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1-pr-manager/swagger.json", "PROJECT_MANAGER API");
        c.SwaggerEndpoint("/swagger/v1-hr/swagger.json", "HR_MANAGER API");
        c.SwaggerEndpoint("/swagger/v1-employee/swagger.json", "EMPLOYEE API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
