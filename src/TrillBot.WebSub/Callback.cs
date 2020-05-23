using System;
using System.Collections.Generic;
using System.Linq;

namespace TrillBot.WebSub
{
    internal abstract class Callback
    {
        private readonly IEnumerable<IWebSubSubscriptionMetadata> _subscriptionMetadata;

        protected Callback(IEnumerable<IWebSubSubscriptionMetadata> subscriptionMetadata)
        {
            _subscriptionMetadata = subscriptionMetadata;
        }

        protected IWebSubSubscriptionMetadata GetSubscriptionMetadata(
            Guid contentTypeId,
            Guid subscriptionMetadataId)
        {
            var subscriptionMetadata = _subscriptionMetadata.FirstOrDefault(_ => _.CurrentId == subscriptionMetadataId);
            if (subscriptionMetadata == null)
                throw new InvalidSubscriptionCallbackException();

            if (contentTypeId != subscriptionMetadata.ContentType.GUID)
                throw new InvalidSubscriptionCallbackException();

            return subscriptionMetadata;
        }
    }
}