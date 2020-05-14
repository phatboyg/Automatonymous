﻿namespace Automatonymous
{
    using System;


    [Flags]
    public enum CompositeEventOptions
    {
        None = 0,

        /// <summary>
        /// Include the composite event in the initial state
        /// </summary>
        IncludeInitial = 1,
    }
}
