using System.Net;
using System.Net.Mail;

namespace Common.RuntimeTypes
{
    public class User : BaseRuntime
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public User(string name, string email, bool verified)
        {

            Name = name;
            if (!_isValidEmail(email)) throw new ApiException(ErrorConstants.InvalidUserEmail,HttpStatusCode.UnprocessableEntity);
            Email = email;
            EmailVerified = verified;
        }
        private static bool _isValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}