using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace Normandy.Infrastructure.Util.Common
{
    /// <summary>
    /// ClassExtensions
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// 转换成Dic
        /// DescriptionAttribute 标注key,不存在时不添加到Dic中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">属性要自定义ToString方法</param>
        /// <param name="excepKeys"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary<T>(this T obj, List<string> excepKeys = null)
        {
            var type = obj.GetType();
            var dict = new Dictionary<string, string>();
            foreach (var prop in type.GetProperties())
            {
                var attributes = (DescriptionAttribute[])prop.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes == null
                    || !attributes.Any()
                    || (excepKeys != null && excepKeys.Contains(prop.Name)))
                {
                    continue;
                }

                var name = attributes[0].Description;
                var val = prop.GetValue(obj);
                var valStr = val == null ? null : JsonSerializer.Serialize(val);
                dict.Add(name, valStr);
            }

            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="excepKeys"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToObjDic<T>(this T obj, List<string> excepKeys = null)
        {
            var type = obj.GetType();
            var dict = new Dictionary<string, object>();
            foreach (var prop in type.GetProperties())
            {
                var attributes = (DescriptionAttribute[])prop.GetCustomAttributes(typeof(DescriptionAttribute), false);                
                var name = (attributes == null || !attributes.Any()) ? prop.Name : attributes[0].Description;

                if (excepKeys != null && excepKeys.Contains(prop.Name))
                {
                    continue;
                }
                var val = prop.GetValue(obj);
                dict.Add(name, val);
            }

            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertToModel<T>(this object obj) 
        {
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
