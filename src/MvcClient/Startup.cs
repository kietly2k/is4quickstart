using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            /*
                We are using a cookie to locally sign-in the user (via "Cookies" as the DefaultScheme), 
                and we set the DefaultChallengeScheme to oidc because when we need the user to login, we will be using the OpenID Connect protocol.
             */
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            // We then use AddCookie to add the handler that can process cookies.
            .AddCookie("Cookies")
            /*
                AddOpenIdConnect is used to configure the handler that performs the OpenID Connect protocol. 
                The Authority indicates where the trusted token service is located. We then identify this client via the ClientId and the ClientSecret.
                SaveTokens is used to persist the tokens from IdentityServer in the cookie (as they will be needed later).
             */
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";

                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                
                options.Scope.Add("api1");
                // Since SaveTokens is enabled, ASP.NET Core will automatically store the resulting access and refresh token in the authentication session
                options.SaveTokens = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}
