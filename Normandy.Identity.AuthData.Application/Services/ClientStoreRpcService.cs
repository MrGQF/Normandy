using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Grpc.Core;
using Normandy.Identity.AuthDataRpc;
using Normandy.Identity.Domain.Shared.Enums;
using System;
using System.Threading.Tasks;

namespace Normandy.Identity.AuthData.Application.Services
{
    public class ClientStoreRpcService : ClientStoreRpc.ClientStoreRpcBase
    {
        private readonly IClientStore clientStore;
        private readonly IMapper mapper;

        public ClientStoreRpcService(IClientStore clientStore, IMapper mapper)
        {
            this.clientStore = clientStore;
            this.mapper = mapper;
        }

        public override async Task<Result> FindClientByIdAsync(ClientRequest request, ServerCallContext context)
        { 
            var result = new Result
            {
                Code = (int)NormandyIdentityErrorCodes.Success
            };

            try
            {
                var client = await clientStore.FindClientByIdAsync(request.Id);
                if (client == null)
                {
                    result.Code = (int)NormandyIdentityErrorCodes.ClientByIdNotFound;
                    result.Message = NormandyIdentityErrorCodes.ClientByIdNotFound.ToString();
                    return result;
                }

                result.Data = mapper.Map<Client, ClientInfo>(client);
            }
            catch(Exception ex)
            {
                result.Code = (int)NormandyIdentityErrorCodes.ClientFindFailed;
                result.Message = $"{ex.Message}, {ex.StackTrace}";
            }
            
            return result;
        }
    }
}
