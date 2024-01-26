using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Normandy.Identity.AuthData.Rpc.Extensions;
using Normandy.Infrastructure.Config;
using Normandy.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLog(builder.Configuration);
builder.Host.AddApolloConfig();
builder.Services.ConfigureServices(builder.Environment, builder.Configuration, builder.WebHost);

var app = builder.Build();
app.Configure(builder.Configuration);
app.Run();
