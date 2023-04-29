using CoreAuthAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreAuthAPI
{
    public class CustomServiceCollection
    {
        public static WebApplicationBuilder AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            //Register authentication service
            builder.Services.AddAuthentication(options =>
            { hub desktop
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
            builder.Services.AddAuthorization();
            return builder;
        }

        //Build Http Pipeline
        public static WebApplication BuildHttpPipeline(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            //Generate token
            app.MapPost("/security/createToken",
                [AllowAnonymous] (User user) =>
                {
                    if (user.UserName == "abhishek" && user.Password == "Abhishek123")
                    {
                        var issuer = builder.Configuration["Jwt:Issuer"];
                        var audience = builder.Configuration["Jwt:Audience"];
                        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new System.Security.Claims.ClaimsIdentity(new[]
                            {

                new Claim("Id",Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                            }),
                            Expires = DateTime.UtcNow.AddMinutes(5),
                            Issuer = issuer,
                            Audience = audience,
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var jwtToken = tokenHandler.WriteToken(token);
                        var stringToken = tokenHandler.WriteToken(token);
                        return Results.Ok(jwtToken);
                    }
                    return Results.Unauthorized();
                });


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            return app;
           
        }
    }
}
