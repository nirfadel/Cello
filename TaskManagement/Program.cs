using TaskManagement.Core.DB;
using TaskManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Repository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Amazon.CognitoIdentityProvider;
using Serilog;
using TaskManagement.Core.Authentication;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using TaskManagement.Core.Extensions;
using TaskManagement.Core.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management System API", Version = "v1" });

    // Configure Swagger to use Bearer token authentication  
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
   {
       {
           new OpenApiSecurityScheme
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
           },
           Array.Empty<string>()
       }
   });
});

builder.Services.AddExceptionHandler(options=> {
    options.ExceptionHandler = async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = exception?.Message });
    };

});
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddScoped<IAuthService, CognitoAuthService>();
builder.Services.AddScoped<IUserClaimsService, UserClaimsService>();
//builder.Services.AddScoped<IRepository<Project>, ProjectRepository>();
//builder.Services.AddScoped<IRepository<TaskObj>, TaskRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddDbContext<TaskManageDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));
builder.Services.Configure<CognitoSettings>(builder.Configuration.GetSection("AWS:Cognito"));
builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>(new AWSOptions
{
    Region = RegionEndpoint.GetBySystemName(builder.Configuration.GetSection("AWS:Cognito:Region").Value),
    Credentials = new BasicAWSCredentials(
        builder.Configuration.GetSection("AWS:Cognito:AccessKey").Value,
        builder.Configuration.GetSection("AWS:Cognito:SecretKey").Value)
});
builder.Services.AddCognitoAuthentication(builder.Configuration.GetSection("AWS:Cognito:Region").Value,
    builder.Configuration.GetSection("AWS:Cognito:UserPoolId").Value,
     builder.Configuration.GetSection("AWS:Cognito:AppClientId").Value);
    
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddConsole();
    builder.AddDebug();
});
var app = builder.Build();

//app.UseCors(configurePolicy: policy =>
//{
//    policy.WithOrigins(builder.Configuration["AllowedOrigins"]);
//});
app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TaskManageDbContext>();
        db.Database.Migrate();
    }
}
app.Run();
