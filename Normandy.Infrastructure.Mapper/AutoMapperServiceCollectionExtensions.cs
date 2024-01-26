using Microsoft.Extensions.DependencyInjection;

namespace Normandy.Infrastructure.Mapper
{
    public static class AutoMapperServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="assemblyName">需要加载的程序集名称,默认:Normandy</param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection service, string[]? assemblyName = null)
            => service.AddSingleton(AutoMapperConfiguration.GetInstance(assemblyName).GetMapper());
    }
}
