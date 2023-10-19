namespace Common.GroupPermissions
{
    internal class Permission
    {
        public string Action { get; set; }
        public object Subject { get; set; }
        public ICollection<string>? Fields { get; set; }
        public Permission(string action, object subject, ICollection<string>? fields = null)
        {
            Action = action;
            if (subject is null || subject is Array || subject is List<object>)
            {
                throw new ArgumentException(nameof(subject));
            }
            Subject = subject;
            if (fields != null)
            {
                if (subject is not string)
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
            }
            Fields = fields;
        }
    }
    public class PermissionSet
    {
        private readonly ICollection<Permission> _abilities = new List<Permission>();
        public PermissionSet()
        {
        }
        public PermissionSet AddCan(string action, string subject, ICollection<string>? fields = null)
        {
            _abilities.Add(new Permission(action, subject, fields));
            return this;
        }
        public PermissionSet AddCan<T>(string action, T subject, ICollection<string>? fields = null) where T : class
        {
            _abilities.Add(new Permission(action, subject, fields));
            return this;
        }
        public PermissionSet AddCan(string action, string subject, string fields)
        {
            _abilities.Add(new Permission(action, subject, new List<string> { fields }));
            return this;
        }
        public PermissionSet AddCan<T>(string action, T subject, string fields) where T : class
        {
            _abilities.Add(new Permission(action, subject, new List<string> { fields }));
            return this;
        }
        public bool Can(string action, string subject)
        {
            return _abilities.Any(x => x.Action == action && x.Subject is string newSubject && newSubject == subject && x.Fields is null);
        }
        public bool Can<T>(string action, T subject) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && x.Fields is null);
        }
        public bool Can(string action, string subject, string field)
        {
            return _abilities.Any(x => x.Action == action && x.Subject is string newSubject && newSubject == subject && x.Fields?.Contains(field) == true);
        }
        public bool Can<T>(string action, T subject, string field) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && newSubject.Equals(subject) && x.Fields?.Contains(field) == true);
        }
        public bool Can<T>(string action, T subject, ICollection<string> field) where T : class
        {
            return _abilities.Any(x => x.Action == action && x.Subject is T newSubject && newSubject == subject && field.All(y => x.Fields?.Contains(y) == true));
        }
    }
}