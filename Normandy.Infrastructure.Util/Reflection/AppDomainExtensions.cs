using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Normandy.Infrastructure.Util.Reflection
{
    public static class AppDomainExtensions
    {
        /// <summary>
        /// 递归获取所有依赖程序集
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="needAssemblyNamesPrefix"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetReferanceAssemblies(this AppDomain domain, string[] needAssemblyNamesPrefix)
        {
            var result = new List<Assembly>();
            result.Add(Assembly.GetEntryAssembly());
            var domainAssemblys = domain.GetAssemblies(); 
            foreach (var item in domainAssemblys)
            {
                GetReferanceAssemblies(item, result, needAssemblyNamesPrefix);
            }
            return result.Where(an => needAssemblyNamesPrefix.Any(nan => an.FullName.Contains(nan))); 
        }
        static void GetReferanceAssemblies(Assembly assembly, List<Assembly> result, string[] needAssemblyNamesPrefix)
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (var item in referencedAssemblies)
            {
                var ass = Assembly.Load(item);
                if (!result.Contains(ass))
                {
                    result.Add(ass);
                    GetReferanceAssemblies(ass, result, needAssemblyNamesPrefix);
                }
            }
        }

        /// <summary>
        /// 遍历路径下所有程序集
        /// </summary>
        /// <param name="needAssemblyNamesPrefix"></param>
        /// <returns></returns>
        public static Assembly[] GetSolutionAssemblies(this AppDomain domain, string[] needAssemblyNamesPrefix)
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
                                .Where(ans => needAssemblyNamesPrefix.Any(prefix => ans.FullName.Contains(prefix))); ;
            return assemblies.ToArray();
        }

        /// <summary>
        /// 遍历路径下所有程序集名
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="needAssemblyNamesPrefix"></param>
        /// <returns></returns>
        public static string[] GetSolutionAssemblyNames(this AppDomain domain, string[] needAssemblyNamesPrefix)
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Select(x => AssemblyName.GetAssemblyName(x).FullName)
                                .Where(ans => needAssemblyNamesPrefix.Any(prefix => ans.Contains(prefix))); 
            return assemblies.ToArray();
        }
    }
}
