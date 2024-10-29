namespace Common.Utils
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectManyWhere<T>(this IEnumerable<IEnumerable<T>?> values, Func<T, bool> whereFunc)
        {
            var newList = new List<T>();
            foreach (var singArr in values)
            {
                if (singArr is not null)
                {
                    foreach (var value in singArr)
                    {
                        if (whereFunc.Invoke(value) is true)
                        {
                            newList.Add(value);
                        }
                    }
                }
            }
            return newList;
        }
        public static IEnumerable<TNew> SelectManyWhere<TOriginal, TNew>(this IEnumerable<IEnumerable<TOriginal>?> values, Func<TOriginal, bool> whereFunc, Func<TOriginal, TNew> transformFunc)
        {
            var newList = new List<TNew>();
            foreach (var singArr in values)
            {
                if (singArr is not null)
                {
                    foreach (var value in singArr)
                    {
                        if (whereFunc.Invoke(value) is true)
                        {
                            newList.Add(transformFunc.Invoke(value));
                        }
                    }
                }
            }
            return newList;
        }

        public static IEnumerable<TNew> SelectWhere<TOriginal, TNew>(this IEnumerable<TOriginal> values, Func<TOriginal, bool> whereFunc, Func<TOriginal, TNew> transformFunc)
        {
            var newList = new List<TNew>();
            foreach (var value in values)
            {
                if (whereFunc.Invoke(value) is true)
                {
                    newList.Add(transformFunc.Invoke(value));
                }
            }
            return newList;
        }
    }
}