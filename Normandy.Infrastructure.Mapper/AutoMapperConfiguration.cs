using AutoMapper;
using Normandy.Infrastructure.Util.Reflection;

namespace Normandy.Infrastructure.Mapper
{
    public class AutoMapperConfiguration
    {
        private MapperConfiguration _instance;

        public AutoMapperConfiguration(IEnumerable<Profile> profiles)
        {
            _instance = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
        }

        public AutoMapperConfiguration(string[] assemblyNames)
        {
            _instance = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblyNames);
            });
        }

        /// <summary>
        /// 获取映射对象
        /// </summary>
        /// <returns></returns>
        public IMapper GetMapper()
            => _instance.CreateMapper();

        private static string[]? _assemblyNames;
        private static readonly Lazy<AutoMapperConfiguration> _Instance = new Lazy<AutoMapperConfiguration>(
            () =>
            {
                var assemblyNames = AppDomain.CurrentDomain.GetSolutionAssemblyNames(_assemblyNames ?? new string[] { "Normandy" });
                return new AutoMapperConfiguration(assemblyNames);
            });

        public static AutoMapperConfiguration GetInstance(string[]? assemblyNames = null)
        {
            if (assemblyNames != null)
            {
                _assemblyNames = assemblyNames;
            }
            return _Instance.Value;
        }
    }
}
