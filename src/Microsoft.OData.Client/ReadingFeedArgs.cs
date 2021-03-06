//---------------------------------------------------------------------
// <copyright file="ReadingFeedArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using Microsoft.OData.Core;

    /// <summary>
    /// The reading feed arguments
    /// </summary>
    public sealed class ReadingFeedArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadingFeedArgs" /> class.
        /// </summary>
        /// <param name="feed">The feed.</param>
        public ReadingFeedArgs(ODataFeed feed)
        {
            Util.CheckArgumentNull(feed, "feed");
            this.Feed = feed;
        }

        /// <summary>
        /// Gets the feed.
        /// </summary>
        /// <value>
        /// The feed.
        /// </value>
        public ODataFeed Feed { get; private set; }
    }
}
