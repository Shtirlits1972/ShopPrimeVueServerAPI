using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShopPrimeVueServerAPI.Crud;
using ShopPrimeVueServerAPI.Repositories;
using System.Text;



namespace ShopPrimeVueServerAPI
{
    public class Program
    {
        // Единое имя cookie (используется и в AuthController)
        public const string AuthCookieName = "auth_token";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // В Program.cs
            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IUsersService, UsersService>();

            builder.Logging.AddConsole();    // Вывод в консоль
            builder.Logging.AddDebug();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("GlobalAccess", builder =>
                {
                    builder
                        .SetIsOriginAllowed(origin => true) // Разрешить любой origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            });

            builder.Services.AddControllers();

            // JWT
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

            builder.Services
              .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(o =>
              {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateIssuerSigningKey = true,
                      ValidateLifetime = true,
                      ValidIssuer = jwtSection["Issuer"],
                      ValidAudience = jwtSection["Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(key)
                  };

                  // ВАЖНО: достаём токен из HttpOnly-cookie
                  o.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          if (context.Request.Cookies.TryGetValue(AuthCookieName, out var cookieToken)
                              && !string.IsNullOrWhiteSpace(cookieToken))
                          {
                              context.Token = cookieToken;
                          }
                          return Task.CompletedTask;
                      }
                  };
              });

            builder.Services.AddAuthorization();

            // Swagger + Bearer схема (в т.ч. удобно для ручной проверки)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "VueCore3 API",
                    Version = "v1"
                });

                var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Введите токен в формате: Bearer {token}",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securityScheme);

                var requirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(requirement);
            });

            var app = builder.Build();

            // Swagger доступен всегда (можно обернуть в if Dev)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VueCore3 API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            // Отдаём собранный Vue из wwwroot
            app.UseDefaultFiles();  // ищет index.html
            app.UseStaticFiles();

            app.UseCors("GlobalAccess");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Любые пути (кроме /api и статических) — в SPA
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}