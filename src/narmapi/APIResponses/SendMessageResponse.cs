using System.Collections.Generic;
using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class SendMessageResponse
    {
        [DataMember(Name = "json")]
        public SendMessageErrorsResponse json { get; set; }

        [DataMember(Name = "captcha")]
        public string captcha { get; set; }
    }

    [DataContract]
    public class SendMessageErrorsResponse
    {
        [DataMember(Name = "errors")]
        public List<string[]> errors { get; set; }
    }
}
