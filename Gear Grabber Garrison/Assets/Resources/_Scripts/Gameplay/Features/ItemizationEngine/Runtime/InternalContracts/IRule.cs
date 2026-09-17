using System.Collections.Generic;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.InternalContracts
{
    internal interface IRule
    {
        Context Process(Context context, List<ResolutionContext> output);
    }
}