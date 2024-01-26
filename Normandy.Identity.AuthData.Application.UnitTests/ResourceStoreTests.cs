using IdentityServer4.EntityFramework.DbContexts;
using IdentityModel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace Normandy.Identity.AuthData.Application.UnitTests
{
    public class Tests
    {
        private readonly ConfigurationDbContext context = new ConfigurationDbContext(
            new DbContextOptionsBuilder<ConfigurationDbContext>().UseMySql(
                "Server=localhost;Database=auth;User=root;Password=root;Character Set=utf8mb4;", 
                new MySqlServerVersion(new Version(5,7,20))).Options,
            new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions());

        [OneTimeSetUp]
        public void Init()
        {
        }

        [OneTimeTearDown]
        public void Down()
        {
        }

        [Test]
        public void ApiResourceInsert()
        {

            var resource = new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Id = 1,
                Name = "api",
                DisplayName = "接口资源",

                Secrets = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceSecret>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceSecret
                    {
                        ApiResourceId = 1,
                        Id = 1,
                        Value = "secrets".ToSha256(),
                    }
                },
                UserClaims = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceClaim>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceClaim
                    {
                        ApiResourceId = 1,
                        Id = 1,
                        Type = JwtClaimTypes.Name,
                    },
                    new IdentityServer4.EntityFramework.Entities.ApiResourceClaim
                    {
                        ApiResourceId = 1,
                        Id = 2,
                        Type = JwtClaimTypes.NickName,
                    }
                },
                Scopes = new System.Collections.Generic.List<IdentityServer4.EntityFramework.Entities.ApiResourceScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiResourceScope
                    {
                        ApiResourceId = 1,
                        Id = 1,
                        Scope = "apiscope"
                    }
                }
            };
            context.ApiResources.Add(resource);

            context.SaveChanges();
        }

        [Test]
        public void Select()
        {
            var val = context.ApiResources.FirstAsync(t => t.Id == 1);

            context.SaveChanges();
        }
    }
}