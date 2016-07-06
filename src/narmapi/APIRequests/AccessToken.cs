using System.Runtime.Serialization;

namespace narmapi.APIRequests
{
    [DataContract]
    public class AccessToken
    {
        public AccessToken(string code_, string redirectURI_)
        {
            code = code_;
            redirectURI = redirectURI_;
            grantType = "authorization_code";
        }

        [DataMember(Name = "code")]
        public string code { get; set; }

        [DataMember(Name = "redirect_uri")]
        public string redirectURI { get; set; }

        [DataMember(Name = "grant_type")]
        public string grantType { get; set; }
    }
}
