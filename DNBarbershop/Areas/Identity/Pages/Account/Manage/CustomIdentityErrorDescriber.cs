using Microsoft.AspNetCore.Identity;

namespace DNBarbershop.Areas.Identity.Pages.Account.Manage
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Имейл адресът '{userName}' вече е зает. Изберете друг!"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Имейл адресът '{email}' вече е използван. Изберете друг!"
            };
        }
    }
}
