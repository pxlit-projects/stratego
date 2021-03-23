using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Stratego.Api.Authorization;
using Stratego.Api.Authorization.Contracts;
using Stratego.Api.Filters;
using Stratego.AppLogic;
using Stratego.AppLogic.Contracts;
using Stratego.AppLogic.Dto;
using Stratego.AppLogic.Dto.Contracts;
using Stratego.Domain;
using Stratego.Domain.Contracts;
using Stratego.Infrastructure.Storage;

namespace Stratego.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new StrategoExceptionFilterAttribute(provider.GetRequiredService<ILogger<Startup>>()));

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.AddService<StrategoExceptionFilterAttribute>();
            });

            services.AddCors();
            services.AddDbContext<StrategoDbContext>(options =>
            {
                string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StrategoDb;Integrated Security=True";
                options.UseSqlServer(connectionString);
            });
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 8;
                    options.Lockout.AllowedForNewUsers = true;

                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 5;

                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<StrategoDbContext>()
                .AddDefaultTokenProviders();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.SecurityDefinitions.Add("bearer", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description =
                            "Copy 'Bearer ' + valid token into field. You can retrieve a bearer token via '/api/authentication/token'",
                        In = OpenApiSecurityApiKeyLocation.Header
                    });
                    document.Schemes = new List<OpenApiSchema> { OpenApiSchema.Https };
                    document.Info.Title = "Stratego Api";
                    document.Info.Description =
                        "Data services for the Stratego web application";
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("bearer"));
            });

            var tokenSettings = new TokenSettings();
            Configuration.Bind("Token", tokenSettings);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenSettings.Issuer,
                        ValidAudience = tokenSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                    };
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<ITokenFactory>(new JwtTokenFactory(tokenSettings));
            services.AddScoped<IWaitingPool, WaitingPool>();
            services.AddSingleton<IGameCandidateFactory, GameCandidateFactory>();
            services.AddSingleton<IGameCandidateRepository, InMemoryGameCandidateRepository>();
            services.AddSingleton<IGameCandidateMatcher, BasicGameCandidateMatcher>();
            services.AddScoped<IGameService, GameService>();
            services.AddSingleton<IGameFactory, GameFactory>();
            services.AddSingleton<IGameRepository, InMemoryGameRepository>();
            services.AddSingleton<IBoardDtoFactory, BoardDtoFactory>();
            services.AddSingleton<IPlayerGameDtoFactory, PlayerGameDtoFactory>();
            services.AddScoped<IUserRepository, UserDbRepository>();
            services.AddSingleton<IRankingStrategy, RankingStrategy>();
            services.AddScoped<IFriendshipService, FriendshipService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
