using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;

namespace Normandy.Infrastructure.Util.Filter
{
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 定义一个路由前缀变量
        /// </summary>
        private readonly AttributeRouteModel _centralPrefix;

        /// <summary>
        /// 调用时传入指定的路由前缀
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);

        }


        /// <summary>
        /// 接口的Apply方法
        /// </summary>
        /// <param name="application"></param>
        /// 已经标记了 RouteAttribute 的 Controller
        /// 如果在控制器中已经标注有路由了，则会在路由的前面再添加指定的路由内容。
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {                
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_centralPrefix, selectorModel.AttributeRouteModel);
                    }
                }


                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = _centralPrefix;

                    }
                }
            }
        }
    }

    public static class MvcOptionsExtensions 
    {
        /// <summary>
        /// 添加通用路由匹配，默认：[controller]/[action]
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="routeAttribute"></param>
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeProvider = null)
        {
            if (routeProvider == null)
            {
                routeProvider = new RouteAttribute("[controller]/{action=Index}");
            }
            opts.Conventions.Insert(0, new RouteConvention(routeProvider));
        }
    }
}
