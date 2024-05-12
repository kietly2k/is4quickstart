using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

/*
    We are using a cookie to locally sign-in the user (via "Cookies" as the DefaultScheme), 
    and we set the DefaultChallengeScheme to oidc because when we need the user to login, we will be using the OpenID Connect protocol.
 */
builder.Services.AddAuthentication(options =>
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

    options.Scope.Add("profile");
    options.GetClaimsFromUserInfoEndpoint = true;

    options.SaveTokens = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute()
        .RequireAuthorization();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
