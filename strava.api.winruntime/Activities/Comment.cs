﻿using System;
using com.strava.api.Athletes;
using Newtonsoft.Json;

namespace com.strava.api.Activities
{
    /// <summary>
    /// Represents a comment of an activity.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The comment id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The resource state.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// The activity to which the comment was posted.
        /// </summary>
        [JsonProperty("activity_id")]
        public long ActivityId { get; set; }

        /// <summary>
        /// The comment's text.
        /// </summary>
        [JsonProperty("text")]
        public String Text { get; set; }

        /// <summary>
        /// The athlete who wrote the comment.
        /// </summary>
        [JsonProperty("athlete")]
        public Athlete Athlete { get; set; }

        /// <summary>
        /// The time when the comment was crated.
        /// </summary>
        [JsonProperty("created_at")]
        public String TimeCreated { get; set; }
    }
}
