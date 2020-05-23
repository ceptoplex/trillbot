using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using TrillBot.WebSub.Controllers;

namespace TrillBot.WebSub
{
    internal sealed class CallbackControllerConvention : IControllerModelConvention
    {
        private readonly string _callbackControllerRouteTemplate;

        public CallbackControllerConvention(string callbackControllerRouteTemplate)
        {
            _callbackControllerRouteTemplate = callbackControllerRouteTemplate;
        }

        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType != typeof(VerificationCallbackController) &&
                (!controller.ControllerType.IsGenericType ||
                 controller.ControllerType.GetGenericTypeDefinition() != typeof(ContentCallbackController<>)))
                return;

            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(
                    new RouteAttribute(
                        controller.ControllerType == typeof(VerificationCallbackController)
                            ? _callbackControllerRouteTemplate
                            : $"{_callbackControllerRouteTemplate}/" +
                              $"{controller.ControllerType.GenericTypeArguments[0].GUID}"))
            });
        }
    }
}