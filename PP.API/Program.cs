using Microsoft.OpenApi.Models;
using PP.Application.ServiceExtensions;
using PP.Infra.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDIInfrastuctureServices(builder.Configuration)
				.AddDIApplicationServices(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
	swagger.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "ASP .NET 8 Web API",
		Description = "Authentication com Json Web Token"
	});

	//Trecho referente a habilitar autorização com swagger(JWT)
	swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header
	});

	swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			}, Array.Empty<string>()
		}
	});
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(
			opt => opt
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowAnyOrigin()
		);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
