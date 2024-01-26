using System.Text.RegularExpressions;

namespace Normandy.Infrastructure.Util.Common
{
    public static class RegexMatch
    {
        /// <summary>
        /// 正则匹配邮件格式
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            var regex = @"^(\w+)([\-+.][\w]+)*@(\w[\-\w]*\.){1,5}([A-Za-z]){2,6}$";
            return Regex.IsMatch(input, regex);
        }

        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(this string input)
        {
            var regex = @"^[0-9]*$";
            return Regex.IsMatch(input, regex);
        }
    }
}
