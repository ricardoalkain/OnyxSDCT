using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Onyx.ProductService.Endpoints;
using Onyx.ProductService.Extensions;
using Onyx.ProductService.Entities;
using Onyx.ProductService.Endpoints.Authentication;
using Microsoft.OpenApi.Models;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Onyx.ProductService.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName);
    options.IncludeXmlComments(xmlCommentsFileFullPath);

    options.AddSecurityDefinition(AuthConsts.API_KEY_CONFIG, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = AuthConsts.API_KEY_HEADER,
        In = ParameterLocation.Header,
        Description = "API Key needed to access the endpoints."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AuthConsts.API_KEY_CONFIG
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IValidator<Product>, ProductValidator>();

builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthentication();
//app.UseMiddleware<ApiKeyAuthorizationMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();

// Health check
app.MapGet("/health", () => Results.Ok())
   .WithSummary("Health check endpoint")
   .WithTags("Service")
   .WithOpenApi()
   .AllowAnonymous();

app.RegisterProductEndpoints();

app.Run();


public partial class Program { } // Required for integration tests