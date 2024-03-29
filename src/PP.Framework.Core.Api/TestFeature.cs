﻿using OpenTracing;
using PaulPhillips.Framework.Feature.Commands;

namespace PP.Framework.Core.Api
{
    public class TestFeature: Command<Request, bool>
    {
        public override async Task<dynamic> ProcessAsync(ISpan tracingSpan)
        {
            return await Task.FromResult("Hello");
        }    
    }

 

    public class Request
    {
        public string Name { get; set; }
    }
}
