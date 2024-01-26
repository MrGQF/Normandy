using Microsoft.AspNetCore.Builder;
using Normandy.Identity.UserInfo.Api.Extensions;
using Normandy.Infrastructure.Config;
using Normandy.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLog(builder.Configuration);
builder.Host.AddApolloConfig();
builder.Services.ConfigureServices(builder.Environment, builder.Configuration);

var app = builder.Build();
app.ConfigureApplicationBuilder(builder.Configuration, builder.WebHost);

app.Run();
