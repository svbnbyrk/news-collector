using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NewsCollector.Core.Services;
using static NewsCollector.Helpers.JwtMiddleware;
using Microsoft.Extensions.DependencyInjection;

namespace NewsCollector.Helpers
{
    public class JwtMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        //IUserService dependency injection ile alamayız Middleware katmanı her zaman singleton olarak ayağa kalkar ama service methodunu InvokeAsync içinde parametre olarak geçebilirim 
        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context, token, userService);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, string token, IUserService userService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                var user = userService.GetUserById(userId);
                context.Items["User"] = user;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}