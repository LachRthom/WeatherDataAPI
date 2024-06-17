using MongoDB.Driver;
using WeatherDataAPI.Repository;
using WeatherDataAPI.Settings;
using WeatherDataAPI.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// This is setting up swagger which will be our test environment and documentation tool
// provides a GUI for testing endpoints with the documentation displayed with it
// We also setup the security requirement so we can test our authentication via an apiKey
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WeatherDataAPI.xml"));

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather Data API", Version = "v1" });

    options.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "apiKey",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "apiKey"
                },
                Name = "apiKey",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// Configure MongoDB settings
var mongoDbSettings = builder.Configuration.GetSection("MongoConnectionSettings").Get<MongoConnectionSettings>();
var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

// Register the repository and database services to the container
builder.Services.AddSingleton(database);
builder.Services.AddScoped<IDataPointRepository, MongoDataPointRepository>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();
builder.Services.AddScoped<MongoConnectionBuilder>();

// Adding CORS to the services
builder.Services.AddCors(options =>
{
    options.AddPolicy("Google", p =>
    {
        // These domains can make requests to our API
        // By specifying Google's domains, we allow requests originating from 
        // https://www.google.com and https://www.google.com.au to access our API resources.
        // This enables secure communication between our API and Google's services 
        // without violating the same-origin policy enforced by most web browsers.
        p.WithOrigins("https://wwww.google.com", "https://www.google.com.au");

        // Allow any header to be sent along with the request.
        // This allows clients to include custom headers when making requests to our API.
        // Must validate and sanitize any incoming headers on the server side 
        // to prevent security vulnerabilities such as injection attacks.
        p.AllowAnyHeader();

        // Specify the allowed HTTP methods for cross-origin requests.
        p.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Data API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();