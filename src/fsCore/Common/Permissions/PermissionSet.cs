using System.Text.Json.Serialization;

namespace Common.Permissions
{
    public class Permission<T>
    {
        [JsonConstructor]
        public Permission() { }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("subject")]
        public T Subject { get; set; }
        [JsonPropertyName("fields")]
        public ICollection<string>? Fields { get; set; }
        public Permission(string action, T subject, ICollection<string>? fields = null)
        {
            Action = action;
            Subject = subject;
            Fields = fields?.ToHashSet();
        }
    }
    public class PermissionSet<TModel>
    {
        [JsonPropertyName("abilities")]
        public ICollection<Permission<TModel>> Abilities { get; set; } = new HashSet<Permission<TModel>>();
        [JsonConstructor]
        public PermissionSet()
        {
        }
        public Permission<TModel>? GetPermission(string action, TModel subject)
        {
            return Abilities.FirstOrDefault(x => x.Action == action && x.Subject.Equals(subject));
        }
        public Permission<TModel>? GetPermissionSingleSubjectProperty(string action, TModel singleSubjectProperty, string propertyName)
        {
            return Abilities.FirstOrDefault(x => x.Action == action && x.Subject.GetType().GetProperties().FirstOrDefault(x => x.Name == propertyName && x.GetType() == typeof(TModel))?.Equals(singleSubjectProperty) is not null);
        }
        private void _add(Permission<TModel> newPerm)
        {

            var similarPermissionsBySubjectAndAction = Abilities.Where(x => x.Action == newPerm.Action && x.Subject.Equals(x.Subject));
            if (similarPermissionsBySubjectAndAction.Any(x => x.Fields is null)) return;
            var allSimilarFieldsFound = similarPermissionsBySubjectAndAction
                .Where(x => x.Fields is not null)
                .Select(x => x.Fields)
                .SelectMany(x => x);
            var combinedFields = new string[][]
                {
                    newPerm.Fields.ToArray(),
                    allSimilarFieldsFound.ToArray()
                };
            Abilities.Add(new Permission<TModel>(newPerm.Action, newPerm.Subject, combinedFields.SelectMany(x => x).ToHashSet()));
        }
        public PermissionSet<TModel> AddCan(string action, TModel subject, ICollection<string>? fields = null)
        {
            var newPerm = new Permission<TModel>(action, subject, fields?.ToHashSet());
            if (newPerm.Fields is not null)
            {
                _add(newPerm);
            }
            else
            {
                Abilities.Add(newPerm);
            }
            return this;
        }
        public PermissionSet<TModel> AddCan(string action, TModel subject, string field)
        {
            var newPerm = new Permission<TModel>(action, subject, new HashSet<string> { field });
            if (newPerm.Fields is not null)
            {
                _add(newPerm);
            }
            else
            {
                Abilities.Add(newPerm);
            }
            return this;
        }
        public bool Can(string action, TModel subject)
        {
            return Abilities.Any(x => x.Action == action && x.Subject.Equals(subject) && x.Fields is null);
        }
        public bool Can(string action, TModel subject, string field)
        {
            return Abilities.Any(x => x.Action == action && x.Subject.Equals(subject) && (x.Fields is null || x.Fields?.Contains(field) == true));
        }
        public bool Can(string action, TModel subject, ICollection<string> fields)
        {
            return Abilities.Any(x => x.Action == action && x.Subject.Equals(subject) && (fields.All(y => x.Fields?.Contains(y) == true) || x.Fields is null));
        }
    }
}