using fasttests.teamcity;
using scbot.services.compareengine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Helpers;

namespace scbot.processors.teamcity
{
    class PropertyComparer<T> {
        public readonly Func<Update<T>, bool> Compare;
        public readonly Func<Update<T>, Response> Describe;

        public PropertyComparer(Func<Update<T>, bool> comparer, Func<Update<T>, Response> describer)
        {
            Compare = comparer;
            Describe = describer;
        }

        public static PropertyComparer<T> BasicEqualityComparer<TProp>(Expression<Func<Update<T>, TProp>> property, Func<TProp, TProp, string> describer)
        {
            return null; // TODO
        }
    }

    class ComparisonResult<T>
    {
        public readonly Response Response;
        public readonly T NewValue;

        public ComparisonResult(Response response, T newValue)
        {
            Response = response;
            NewValue = newValue;
        }
    }

    class CompareEngine<T>
    {
        private services.ListPersistenceApi<Tracked<T>> m_Persistence;
        private const string c_PersistenceKey = "tracked-tc-builds";
        private readonly Func<T, string> m_PrefixStringGenerator;
        private readonly List<PropertyComparer<T>> m_Comparers;

        public CompareEngine(services.ListPersistenceApi<Tracked<T>> persistence, Func<T, string> prefixString, IEnumerable<PropertyComparer<T>> comparers)
        {
            m_Persistence = persistence;
            m_PrefixStringGenerator = prefixString;
            m_Comparers = new List<PropertyComparer<T>>(comparers);
        }

        internal IEnumerable<ComparisonResult<T>> CompareBuildStates(IEnumerable<Update<T>> comparison)
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
            foreach (var comparer in m_Comparers)
            {
                if (comparer.Compare(x))
                {
                    yield return comparer.Describe(x);
                }
            }
        }
    }
}
