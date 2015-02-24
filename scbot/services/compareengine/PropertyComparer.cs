using System;
using System.Linq.Expressions;
using scbot.services.compareengine;

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
}