namespace Normandy.Identity.Client.Configs
{
    /// <summary>
    /// 配置选项
    /// </summary>
    public class ConfigOptions
    {
        /// <summary>
        /// 配置文件地址目录(文件名由SDK 指定)
        /// 默认: 程序目录
        /// 初始化时，会自动在目录下创建配置文件
        /// </summary>
        public string Path { get; set; }     
    }
}
