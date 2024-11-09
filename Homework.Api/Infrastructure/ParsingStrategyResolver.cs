using System.Collections.Generic;
using Homework.Api.Interfaces;

namespace Homework.Api.Infrastructure
{
    public class ParsingStrategyResolver
    {
        private readonly Dictionary<string, IApiResponseParsingStrategy> _strategies;

        public ParsingStrategyResolver(IEnumerable<IApiResponseParsingStrategy> strategies)
        {
            _strategies = new Dictionary<string, IApiResponseParsingStrategy>();

            foreach (var strategy in strategies)
            {
                _strategies[strategy.GetType().Name] = strategy;
            }
        }

        public IApiResponseParsingStrategy GetStrategy(string strategyName)
        {
            _strategies.TryGetValue(strategyName, out var strategy);
            return strategy;
        }
    }
}
