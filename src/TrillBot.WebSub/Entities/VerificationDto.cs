using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrillBot.WebSub.ModelBinding;

namespace TrillBot.WebSub.Entities
{
    internal sealed class VerificationDto
    {
        [FromQuery(Name = "hub.topic")]
        [Required]
        public Uri Topic { get; set; }

        [FromQuery(Name = "hub.mode")]
        [Required]
        public SubscriptionMode Mode { get; set; }

        [FromQuery(Name = "hub.lease_seconds")]
        [ModelBinder(typeof(SecondsToTimeSpanModelBinder))]
        public TimeSpan? Lease { get; set; }

        [FromQuery(Name = "hub.challenge")]
        [Required]
        public string Challenge { get; set; }

        [FromQuery(Name = "hub.reason")]
        public string Reason { get; set; }
    }
}