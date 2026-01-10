using Server.Interfaces;

namespace Server.ActionFilters;

using Microsoft.AspNetCore.Mvc.Filters;

public class UpdateCacheStockList : IActionFilter
{
    private readonly IDataCenterService _dataCenterService; 

    public UpdateCacheStockList(IDataCenterService dataCenterService)
    {
        _dataCenterService = dataCenterService;
    }
    
    public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context)
    {

        if (context.Exception == null && context.HttpContext.Response.StatusCode == 200)
        {
            // _dataCenterService.();
        }
    }
}