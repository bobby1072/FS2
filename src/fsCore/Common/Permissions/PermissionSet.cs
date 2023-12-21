using System.Text.Json.Serialization;

namespace Common.Permissions
{
    public class Permission
    {
        [JsonConstructor]
        public Permission() { }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("subject")]
        public object Subject { get; set; }
        [JsonPropertyName("fields")]
        public ICollection<string>? Fields { get; set; }
        public Permission(string action, object subject, ICollection<string>? fields = null)
        {
            Action = action;
            Subject = subject;
            Fields = fields?.ToHashSet();
        }
    }
    public class PermissionSet
    {
        [JsonPropertyName("abilities")]
        public ICollection<Permission> Abilities { get; set; } = new HashSet<Permission>();
        [JsonConstructor]
        public PermissionSet()
        {
        }
        public Permission? GetPermission(string action, object subject)
        {
            return Abilities.FirstOrDefault(x => x.Action == action && x.Subject.Equals(subject));
        }
        public Permission? GetPermissionSingleSubjectProperty<T>(string action, T singleSubjectProperty, string propertyName)
        {
            return Abilities.FirstOrDefault(x => x.Action == action && x.Subject.GetType().GetProperties().FirstOrDefault(x => x.Name == propertyName && x.GetType() == typeof(T))?.Equals(singleSubjectProperty) is not null);
        }
        public static PermissionSet CreateSet() => new();
        private void _add(Permission newPerm)
        {

            var similarPermissionsBySubjectAndAction = Abilities.Where(x => x.Action == newPerm.Action && x.Subject.Equals(x.Subject));
            if (similarPermissionsBySubjectAndAction.Any(x => x.Fields is null)) return;
            var allSimilarFieldsFound = similarPermissionsBySubjectAndAction
                .Where(x => x.Fields is not null)
                .Select(x => x.Fields)
                .SelectMany(x => x);
            var combinedFields = new List<List<string>>
                {
                    newPerm.Fields.ToList(),
                    allSimilarFieldsFound.ToList()
                };
            Abilities.Add(new Permission(newPerm.Action, newPerm.Subject, combinedFields.SelectMany(x => x).ToHashSet()));
        }
        public PermissionSet AddCan<T>(string action, T subject, ICollection<string>? fields = null) where T : class
        {
            var newPerm = new Permission(action, subject, fields?.ToHashSet());
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
        public PermissionSet AddCan<T>(string action, T subject, string field) where T : class
        {
            var newPerm = new Permission(action, subject, new HashSet<string> { field });
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
        public PermissionSet AddCan(string action, string subject)
        {
            var newPerm = new Permission(action, subject);
            Abilities.Add(newPerm);
            return this;
        }
        public bool Can<T>(string action, T subject) where T : class
        {
            return Abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && x.Fields is null) || (x.Action == action && x.Subject is true));
        }
        public bool Can<T>(string action, T subject, string field) where T : class
        {
            return Abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && (x.Fields?.Contains(field) == true || x.Fields is null)) || (x.Action == action && x.Subject is true));
        }
        public bool Can<T>(string action, T subject, ICollection<string> fields) where T : class
        {
            return Abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && (fields.All(y => x.Fields?.Contains(y) == true) || x.Fields is null)) || (x.Action == action && x.Subject is true));
        }
    }
}