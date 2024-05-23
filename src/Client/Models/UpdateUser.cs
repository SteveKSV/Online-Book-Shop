using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class UpdateUser : IValidatableObject
    {
        [EmailAddress(ErrorMessage = "Invalid email address. Format: (yourText)@(mail).com. Example: whatToRead@gmail.com")]
        public string NewEmail { get; set; }
        public string OldEmail { get; set; }
        public string OldUsername { get; set; }
        public string NewUsername { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NewEmail) && string.IsNullOrEmpty(NewUsername))
            {
                yield return new ValidationResult(
                    "Either new email or new username must be provided.",
                    new[] { nameof(NewEmail), nameof(NewUsername) });
            }

            if (!string.IsNullOrEmpty(NewEmail) && !new EmailAddressAttribute().IsValid(NewEmail))
            {
                yield return new ValidationResult(
                    "Invalid email address format. Format: (yourText)@(mail).com. Example: whatToRead@gmail.com",
                    new[] { nameof(NewEmail) });
            }
        }
    }
}
