using System.ComponentModel.DataAnnotations;

namespace LTA.Mobile.Attributes;

public class PhoneOrEmailAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is not string emailOrPhone)
            return false;

        return IsValidEmail(emailOrPhone) || IsValidPhoneNumber(emailOrPhone);
    }

    public static string GetErrorMessageIfIsNotValid(string emailOrPhone)
    {
        if (IsValidEmail(emailOrPhone))
        {
            return string.Empty;
        }

        if (IsValidPhoneNumber(emailOrPhone))
        {
            return string.Empty;
        }

        return "Неверный номер телефона или E-Mail";
    }
    private static bool IsValidPhoneNumber(string phoneNumber)
        => new PhoneAttribute().IsValid(phoneNumber);
    private static bool IsValidEmail(string email)
        => new EmailAddressAttribute().IsValid(email);
}