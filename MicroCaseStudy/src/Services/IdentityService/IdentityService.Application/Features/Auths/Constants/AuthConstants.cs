namespace IdentityService.Application.Features.Auths.Constants;

public static class AuthConstants
{

    public const string EmailActivationKeyDontExists = "E-posta Aktivasyon Anahtarı bulunmuyor.";
    public const string UserDontExists = "Kullanıcı bulunmuyor.";
 
    public const string RefreshDontExists = "Yenileme bulunmuyor.";
    public const string InvalidRefreshToken = "Geçersiz yenileme token'ı.";
    public const string UserMailAlreadyExists ="Kullanıcı e-postası zaten mevcut.";
    public const string UsernameAlreadyExists = "Kullanıcı adı zaten mevcut.";
    public const string PasswordDontMatch = "Parola eşleşmiyor.";
    public const string EnsurePasswordSet = "EnsurePasswordSet";
    public const string ActivationCodeMismatch =
        "Activasyon kodu eşleşmiyor. Lütfen kodu kontrol edin ve tekrar deneyin.";
    public const string User2FAActive = "User2FAActive";
} 