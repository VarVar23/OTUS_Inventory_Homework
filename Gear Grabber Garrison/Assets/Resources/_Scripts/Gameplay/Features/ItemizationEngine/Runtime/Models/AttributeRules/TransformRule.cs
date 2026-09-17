using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    internal sealed class TransformRule : IRule
    {
        private readonly Action<Context> _transform;

        public TransformRule(Action<Context> transformFunction)
            => _transform = transformFunction;

        public Context Process(Context context, List<ResolutionContext> output)
        {
            _transform(context);
            return context;
        }
    }
}