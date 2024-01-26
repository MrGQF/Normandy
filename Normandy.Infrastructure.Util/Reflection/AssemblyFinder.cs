using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Normandy.Infrastructure.Util.Reflection
{
    public class AssemblyFinder
    {
        public static Assembly GetDomainAssembly()
            => Assembly.GetEntryAssembly();

        public static IEnumerable<AssemblyName> GetAssemblies()
            => GetDomainAssembly().GetReferencedAssemblies().Distinct().Append(GetDomainAssembly().GetName());

        public static IEnumerable<AssemblyName> GetCustomerAssemblyNames(string[] needAssemblyNamesPrefix)
            => GetAssemblies().Where(an => needAssemblyNamesPrefix.Any(nan => an.FullName.Contains(nan)));

        /// <summary>
        /// 这里有个坑，只能获取到当前域里面加载的程序集
        /// net程序集有延迟加载，如果程序集有引用但是未加载，是获取不到的
        /// https://www.cnblogs.com/qianxingmu/p/13363193.html
        /// </summary>
        /// <param name="needAssemblyNamesPrefix"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetCustomerAssemblies(string[] needAssemblyNamesPrefix)
        {
            var assemblys = new List<Assembly>();
            var names = GetCustomerAssemblyNames(needAssemblyNamesPrefix);
            foreach (var item in names)
            {
                var assembly = GetAssemblyByName(item);
                assemblys.Add(assembly);
            }

            return assemblys;
        }

        public static Assembly GetAssemblyByName(AssemblyName name)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(name);
        }        
    }
}
