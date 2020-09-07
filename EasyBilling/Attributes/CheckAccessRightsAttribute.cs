﻿using EasyBilling.Models.Pocos;
using EasyBilling.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Attributes
{
    public class CheckAccessRightsAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private AccessRightsManager _arm;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _arm = context.HttpContext.RequestServices
                .GetRequiredService<AccessRightsManager>();

            //Приведение к типу для получения функций и действий контроллера
            var ad = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)
                context.ActionDescriptor;
            var controllerName = ad.ControllerName + "Controller";

            AccessRight accessRights = _arm.GetRights(context.HttpContext.User.Identity.Name,
                controllerName).Result;
            //При каких условиях давать доступ
            if (controllerName.Equals("HomeController") ||
                (accessRights != null && accessRights.IsAvailable))
                return;

            context.HttpContext.Response.Redirect($"/Home/ErrorAccess");
        }
    }
}