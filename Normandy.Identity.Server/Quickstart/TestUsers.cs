// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "ths_test",
                        Password = "123456",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "ths_test"),
                            new Claim(JwtClaimTypes.GivenName, "ths_test"),
                            new Claim(JwtClaimTypes.FamilyName, "ths"),
                            new Claim(JwtClaimTypes.Email, "gaoqifei@myhexin.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "https://www.10jqka.com.cn/"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),

                            new Claim(JwtClaimTypes.Role,"admin")  //添加角色
                        },

                    }
                };
            }
        }
    }
}