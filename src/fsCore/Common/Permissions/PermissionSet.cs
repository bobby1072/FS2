namespace Common.Permissions
{
    internal class Permission
    {
        public string Action { get; set; }
        private object _subject;
        public object Subject
        {
            get => _subject;
            set
            {
                if (value is null || value is Array || value is List<object>)
                {
                    throw new ArgumentException(nameof(value));
                }
                Subject = value;
            }
        }
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
        private readonly ICollection<Permission> _abilities = new HashSet<Permission>();
        public PermissionSet()
        {
        }
        public static PermissionSet CreateSet() => new();
        private void _add(ICollection<Permission> permissionList, Permission newPerm)
        {
            if (newPerm.Fields is not null)
            {
                var similarPermissionsBySubjectAndAction = permissionList.Where(x => x.Action == newPerm.Action && x.Subject.Equals(x.Subject));
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
                permissionList.Add(new Permission(newPerm.Action, newPerm.Subject, combinedFields.SelectMany(x => x).ToHashSet()));
            }
            permissionList.Add(newPerm);
        }
        public PermissionSet AddCan<T>(string action, T subject, ICollection<string>? fields = null) where T : class
        {
            _add(_abilities, new Permission(action, subject, fields?.ToHashSet()));
            return this;
        }
        public PermissionSet AddCan<T>(string action, T subject, string field) where T : class
        {
            _add(_abilities, new Permission(action, subject, new HashSet<string> { field }));
            return this;
        }
        public PermissionSet AddCan(string action, string subject)
        {
            _add(_abilities, new Permission(action, subject));
            return this;
        }
        public bool Can<T>(string action, T subject) where T : class
        {
            return _abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && x.Fields is null) || (x.Action == action && x.Subject is true));
        }
        public bool Can<T>(string action, T subject, string field) where T : class
        {
            return _abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && (x.Fields?.Contains(field) == true || x.Fields is null)) || (x.Action == action && x.Subject is true));
        }
        public bool Can<T>(string action, T subject, ICollection<string> fields) where T : class
        {
            return _abilities.Any(x => (x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && (fields.All(y => x.Fields?.Contains(y) == true) || x.Fields is null)) || (x.Action == action && x.Subject is true));
        }
    }
}