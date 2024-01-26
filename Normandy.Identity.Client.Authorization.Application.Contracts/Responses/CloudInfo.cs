using Normandy.Legacy.Client.Protos;
using System;
using System.Collections.Generic;

namespace Normandy.Identity.Client.Authorization.Application.Contracts.Responses
{
    public class CloudInfo
    {
        public Int64 Version { get; set; } = 0;
        public System.Net.HttpStatusCode Code { get; set; } = System.Net.HttpStatusCode.OK;
        public List<EntityFile> files { get; set; } = new List<EntityFile>();
    }
}
