using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TestProject.Data;
using TestProject.Middleware;
using TestProject.Models;
using TestProject.Models.JWT;
using TestProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSeeding((context, _) =>
    {
        var roleStudent = context.Set<IdentityRole<int>>().FirstOrDefault(b => b.Name == UserRole.Student);
        if (roleStudent == null)
        {
            context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Name = UserRole.Student, NormalizedName = "STUDENT" });
            context.SaveChanges();
        }

        var roleTeacher = context.Set<IdentityRole<int>>().FirstOrDefault(b => b.Name == UserRole.Teacher);
        if (roleTeacher == null)
        {
            context.Set<IdentityRole<int>>().Add(new IdentityRole<int> { Name = UserRole.Teacher, NormalizedName = "TEACHER" });
            context.SaveChanges();
        }

        var user = context.Set<User>().FirstOrDefault(b => b.UserName == "Teacher");

        if (user == null)
        {
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

            var newUser = new User
            {
                UserName = "Teacher",
                FullName = "Teacher",
                NormalizedUserName = "TEACHER",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            newUser.PasswordHash = passwordHasher.HashPassword(newUser, "Teacher");

            context.Set<User>().Add(newUser);

            context.SaveChanges();

            var roleId = context.Set<IdentityRole<int>>().Where(b => b.Name == UserRole.Teacher).Select(i => i.Id).FirstOrDefault();

            context.Set<IdentityUserRole<int>>().Add(new IdentityUserRole<int> { RoleId = roleId, UserId = newUser.Id });

            context.SaveChanges();
        }
    }));

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddTransient<SubjectService>();

// JWT
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "JWT Token Authentication API"
    });
    // To Enable authorization using Swagger (JWT)  
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
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
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Description = "This is open api demo";

        // Add when authorization headers are required
        var authComponent = new OpenApiComponents
        {
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "Authorization", new OpenApiHeader { Required = true } }
            }
        };
        document.Components = authComponent;

        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
