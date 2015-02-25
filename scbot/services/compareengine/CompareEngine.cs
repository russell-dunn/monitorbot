using System;
using System.Collections.Generic;
using System.Linq;
using scbot.bot;
using scbot.utils;

namespace scbot.services.compareengine
{
    class CompareEngine<T>
    {
        private readonly Func<T, string> m_PrefixStringGenerator;
        private readonly List<PropertyComparer<T>> m_Comparers;

        public CompareEngine(Func<T, string> prefixString, IEnumerable<PropertyComparer<T>> comparers)
        {
            m_PrefixStringGenerator = prefixString;
            m_Comparers = new List<PropertyComparer<T>>(comparers);
        }

        internal IEnumerable<ComparisonResult<T>> Compare(IEnumerable<Update<T>> comparison)
        {
            var differences = comparison.Select(x => new
            {
                differences = GetDifferenceString(x),
                update = x
            }).ToList();

            differences = differences.Where(x => x.differences != null).ToList();

            return differences.Select(x => new ComparisonResult<T>(
                new Response(x.differences.Message, x.update.Channel, x.differences.Image), 
                x.update.NewValue));
        }

        private Response GetDifferenceString(Update<T> ttc)
        {
            var differences = GetDifferences(ttc).ToList();
            if (differences.Any())
            {
                var image = differences.Select(x => x.Image).FirstOrDefault(x => x.IsNotDefault());
                return new Response(string.Format("{0} {1}",
                    m_PrefixStringGenerator(ttc.NewValue), string.Join(", ", differences.Select(x => x.Message))), null, image);
            }
            return null;
        }

        private IEnumerable<Response> GetDifferences(Update<T> x)
        {
            return m_Comparers
                .Where(comparer => comparer.Compare(x))
                .Select(comparer => comparer.Describe(x));
        }
    }
}
