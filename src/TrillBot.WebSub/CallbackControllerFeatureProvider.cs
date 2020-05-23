using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using TrillBot.WebSub.Controllers;

namespace TrillBot.WebSub
{
    internal sealed class CallbackControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<ContentTypeInfo> _subscriptionContentTypeInfos;

        public CallbackControllerFeatureProvider(IEnumerable<ContentTypeInfo> subscriptionContentTypeInfos)
        {
            _subscriptionContentTypeInfos = subscriptionContentTypeInfos;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Add(typeof(VerificationCallbackController).GetTypeInfo());
            foreach (var subscriptionContentTypeInfo in _subscriptionContentTypeInfos)
                feature.Controllers.Add(
                    typeof(ContentCallbackController<>).MakeGenericType(
                            subscriptionContentTypeInfo.ContentType)
                        .GetTypeInfo());
        }
    }
}