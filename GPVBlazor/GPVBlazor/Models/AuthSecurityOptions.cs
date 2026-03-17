using Microsoft.AspNetCore.Http;

namespace GPVBlazor.Models
{
    public class AuthSecurityOptions
    {
        public string AuthCookieName { get; set; } = "gpv.auth";
        public SameSiteMode AuthCookieSameSite { get; set; } = SameSiteMode.Lax;
        public CookieSecurePolicy AuthCookieSecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;
        public string AntiforgeryCookieName { get; set; } = "gpv.af";
        public string AntiforgeryHeaderName { get; set; } = "X-CSRF-TOKEN";
        public string AntiforgeryRequestTokenCookieName { get; set; } = "gpv.csrf";
        public SameSiteMode AntiforgeryCookieSameSite { get; set; } = SameSiteMode.Strict;
        public SameSiteMode AntiforgeryRequestTokenCookieSameSite { get; set; } = SameSiteMode.Strict;
        public CookieSecurePolicy AntiforgeryCookieSecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;
        public string OAuthStateCookieName { get; set; } = "gpv.oauth.state";
        public SameSiteMode OAuthStateCookieSameSite { get; set; } = SameSiteMode.Lax;
        public CookieSecurePolicy OAuthStateCookieSecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;
        public int OAuthStateTtlMinutes { get; set; } = 10;
    }
}
