using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class AuthorizedAccessToken
    {
        [DataMember(Name = "access_token")]
        public string accessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string refreshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int expiresIn { get; set; }

        [DataMember(Name = "token_type")]
        public string tokenType { get; set; }

        [DataMember(Name = "scope")]
        public string scope { get; set; }
    }
}
