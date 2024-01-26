namespace Normandy.Identity.Server.Application.Contracts.Responses
{
    public class RiskResponse<T>
    {
        public string Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }
}
