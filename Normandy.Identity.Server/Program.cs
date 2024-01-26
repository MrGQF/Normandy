using Microsoft.AspNetCore.Builder;
using Normandy.Identity.Server.Extensions;
using Normandy.Infrastructure.Config;
using Normandy.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLog(builder.Configuration);
builder.Host.AddApolloConfig();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.Configure(builder.Configuration, builder.WebHost);
app.Run();


