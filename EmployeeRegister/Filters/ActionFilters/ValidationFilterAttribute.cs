using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace EmployeeRegister.Filters.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        private readonly ILoggerManager _logger;

        public ValidationFilterAttribute(ILoggerManager logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the name of the action object
            var action = context.RouteData.Values["action"];
            // Get the name of the controller object
            var controller = context.RouteData.Values["controller"];

            

            // Get the arguments passed
            var param = context.ActionArguments
                .SingleOrDefault(x => x.Value.ToString().Contains("DTOW")).Value;

            // Do this if no arguments are passed
            if(param == null)
            {
                _logger.LogError($"Object sent from client is null. Controller : {controller}, action: {action}");
                context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
            }

            // If arguments are passed, check the model state and do this
            if (!context.ModelState.IsValid)
            {
                _logger.LogError($"Invalid model state for the object. Controller: {controller}, action: {action}");
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }
    }
}
