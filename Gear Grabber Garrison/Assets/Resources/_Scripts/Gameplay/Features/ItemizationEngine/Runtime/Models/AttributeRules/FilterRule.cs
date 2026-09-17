using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    internal sealed class FilterRule : IRule
    {
        private readonly Func<Context, bool> _predicate;
        public FilterRule(Func<Context, bool> predicate) => _predicate = predicate;

        public Context Process(Context context, List<ResolutionContext> output)
        {
            return _predicate(context) ? context : null;
        }
    }
}