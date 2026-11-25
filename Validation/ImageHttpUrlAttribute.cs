using System.ComponentModel.DataAnnotations;

namespace CharacterHub.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class ImageHttpUrlAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string text ||
            !Uri.TryCreate(text, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && !string.IsNullOrWhiteSpace(uri.Host);
    }
}
