using Microsoft.AspNetCore.Authentication.JwtBearer;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using Google;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Microsoft.Extensions.Configuration;
using CollaborateMusicAPI.Authorization;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using CollaborateMusicAPI.Services.Email;
using CollaborateMusicAPI.Services.PasswordReset;
using ALIVEMusicAPI.SeedData;
using ALIVEMusicAPI.Services;
using ALIVEMusicAPI.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<RefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IExternalAuthService, ExternalAuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, BCryptPasswordHasher<ApplicationUser>>();
builder.Services.AddSingleton<ITokenValidationService, TokenValidationService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<RoleSeeder>();

builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IAzureBlobService>(sp => new AzureBlobService(builder.Configuration.GetConnectionString("AzureBlobStorage")));
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackService, TrackService>();

builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<ILikesService, LikesService>();

builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<ICommentsService, CommentsService>();

builder.Services.AddScoped<ICommentLikesRepository, CommentLikesRepository>();
builder.Services.AddScoped<ICommentLikesService, CommentLikesService>();




builder.Services.AddHttpClient<IGoogleTokenService, GoogleTokenService>();
builder.Services.AddTransient<GenerateTokenService>();

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.Configure<IdentityOptions>(options =>
     options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});




// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

// DbContext registration
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var tokenValidationService = serviceProvider.GetRequiredService<ITokenValidationService>();
        options.TokenValidationParameters = tokenValidationService.GetTokenValidationParameters();
        // Additional configuration...
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

//.AddJwtBearer(jwtOptions =>
//{
//    jwtOptions.Events = new JwtBearerEvents
//    {
//        OnTokenValidated = context =>
//        {
//            var claims = context.Principal.Claims;
//            foreach (var claim in claims)
//            {
//                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
//            }
//            return Task.CompletedTask;
//        },
//        OnMessageReceived = context =>
//        {
//            Console.WriteLine($"Token received: {context.Token}");
//            return Task.CompletedTask;
//        },
//        OnAuthenticationFailed = context =>
//        {
//            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
//            return Task.CompletedTask;
//        }
//    };
//    jwtOptions.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],

//    };
//})
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyHeader()
                         .AllowAnyMethod();
        });
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
    options.AddPolicy("SubscribingMember", policy => policy.RequireRole("SubscribingMember"));
    options.AddPolicy("NonPayingMember", policy => policy.RequireRole("NonPayingMember"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Create a new scope to retrieve scoped services
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get our RoleSeeder from the DI system and execute the SeedRoles method
        var seeder = services.GetRequiredService<RoleSeeder>();
        await seeder.SeedRoles();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the roles.");
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
