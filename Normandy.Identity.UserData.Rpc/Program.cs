using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Normandy.Identity.UserData.Api.Extensions;
using Normandy.Infrastructure.Log;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddApollo();
builder.Host.UseLog(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration, builder.WebHost);

var app = builder.Build();
app.Configure(builder.Configuration);
app.Run();