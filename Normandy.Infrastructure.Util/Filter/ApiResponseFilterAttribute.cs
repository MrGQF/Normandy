using Microsoft.AspNetCore.Mvc;
using System;

namespace Normandy.Infrastructure.Util.Filter
{
    public class ApiResponseFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        {
            if (context.Result == null
                || context.Result is Microsoft.AspNetCore.Mvc.EmptyResult)
            {
                context.Result = new ObjectResult(new
                {
                    Code = 0,
                    Flag = 0,
                });
            }

            if (context.Result is ObjectResult)
            {
                var result = context.Result as ObjectResult;
                context.Result = new ObjectResult(new 
                {
                    Code = 0,
                    Flag = 0,
                    Data = result.Value
                });
            }

            base.OnActionExecuted(context);
        }
    }
}
