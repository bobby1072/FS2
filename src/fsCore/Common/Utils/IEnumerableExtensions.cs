namespace Common.Utils
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectManyWhere<T>(this IEnumerable<IEnumerable<T>?> values, Func<T, bool> func)
        {
            var newList = new List<T>();
            foreach (var singArr in values)
            {
                if (singArr is not null)
                {
                    foreach (var value in singArr)
                    {
                        if (func.Invoke(value) is true)
                        {
                            newList.Add(value);
                        }
                    }
                }
            }
            return newList;
        }
    }
}