namespace Tools
{
    public static class FieldComparator
    {
        public static IEnumerable<string> GetNotEqualPropertiesAndFieldsNames<T>(T a, T b, params string[] ignoreFieldNames)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            var type = a.GetType();

            var props = type.GetProperties();

            var fields = type.GetFields();

            var filteredProps = props.Where(f =>
            {
                if (ignoreFieldNames.Contains(f.Name))
                {
                    return false;
                }

                var valueB = f.GetValue(b);
                var valueA = f.GetValue(a);

                if (valueB == null)
                {
                    return valueA != null;
                }

                return !valueB.Equals(valueA);
            }).Select(f => f.Name).ToList();

            var filteredFields = fields.Where(f =>
            {
                if (ignoreFieldNames.Contains(f.Name))
                {
                    return false;
                }

                var valueB = f.GetValue(b);
                var valueA = f.GetValue(a);

                if (valueB == null)
                {
                    return valueA != null;
                }

                return !valueB.Equals(valueA);
            }).Select(f => f.Name);

            filteredProps.AddRange(filteredFields);

            return filteredProps;
        }
    }
}
