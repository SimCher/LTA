using System.ComponentModel.DataAnnotations;
using LTA.Mobile.Enums;

namespace LTA.Mobile.Attributes;

public class PhoneOrEmailAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string emailOrPhone) return false;
        return IsValidEmail(emailOrPhone) || IsValidPhoneNumber(emailOrPhone);
    }

    public static string GetKeyword(string phoneOrEmail)
    {
        if (IsValidPhoneNumber(phoneOrEmail))
        {
            return PhoneOrEmail.Phone.ToString();
        }

        return IsValidEmail(phoneOrEmail) ? PhoneOrEmail.Email.ToString() : string.Empty;
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
        => new PhoneAttribute().IsValid(phoneNumber);

    private static bool IsValidEmail(string email)
        => new EmailAddressAttribute().IsValid(email);
}