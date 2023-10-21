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
            if (fields != null)
            {
                var subjectPropertyInfo = subject.GetType().GetProperties();
                foreach (var field in fields)
                {
                    if (!subjectPropertyInfo.Any(x => x.Name == field))
                    {
                        throw new ArgumentException($"Field {field} not found in subject {subject.GetType().Name}");
                    }
                }
            }
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
        public PermissionSet AddCan<T>(string action, T subject, ICollection<string>? fields = null) where T : class
        {
            _abilities.Add(new Permission(action, subject, fields?.ToHashSet()));
            return this;
        }
        public PermissionSet AddCan<T>(string action, T subject, string field) where T : class
        {
            _abilities.Add(new Permission(action, subject, new HashSet<string> { field }));
            return this;
        }
        public PermissionSet AddCan(string action, string subject)
        {
            _abilities.Add(new Permission(action, subject));
            return this;
        }
        public bool Can<T>(string action, T subject, bool exactSubjectObjectMatch = false) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && (!exactSubjectObjectMatch || newSubject.Equals(subject)) && x.Fields is null);
        }
        public bool Can<T>(string action, T subject, string field, bool exactSubjectObjectMatch = false) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && (!exactSubjectObjectMatch || newSubject.Equals(subject)) && x.Fields?.Contains(field) == true);
        }
        public bool Can<T>(string action, T subject, ICollection<string> fields, bool exactSubjectObjectMatch = false) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && (!exactSubjectObjectMatch || newSubject.Equals(subject)) && fields.All(y => x.Fields?.Contains(y) == true));
        }
        public bool Can(string action, string subject)
        {
            return _abilities.Any(x => x.Action == action && x.Subject is string newSubject && newSubject == subject);
        }
    }
}