using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Config
{
    public static class JsonFileHelper
    {
		/// <summary>
		/// 更新json配置文件
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="pairs"></param>
		/// <returns></returns>
        public static Task Update(string filePath, Dictionary<string, object> pairs)
        {
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}

			var jsonString = File.ReadAllText(filePath, Encoding.Default);
			var jobject = JObject.Parse(jsonString);
			foreach(var pair in pairs)
            {
				jobject[pair.Key] = pair.Value.ToString();
            }
	
			var convertString = Convert.ToString(jobject);
			return File.WriteAllTextAsync(filePath, convertString);
		}

        /// <summary>
        /// 读取或创建配置文件
        /// 不存在则创建
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="defaultValue"></param>
        public static void GetOrCreateConfigFiles(string filePath, string defaultValue)
        {
            if (File.Exists(filePath))
            {
                return;
            }

            File.WriteAllText(filePath, defaultValue);
            return;
        }

        /// <summary>
        /// 重建配置文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        public static void RebuildConfigFiles(string path, string defaultValue)
        {
            File.WriteAllText(path, defaultValue);
            return;
        }
    }
}
