using System.Collections.Generic;
using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class SendCommentResponse
    {
        [DataMember(Name = "json")]
        public SendCommentErrorsResponse json { get; set; }

        [DataMember(Name = "captcha")]
        public string captcha { get; set; }
    }

    [DataContract]
    public class SendCommentErrorsResponse
    {
        [DataMember(Name = "errors")]
        public List<string[]> errors { get; set; }
    }
}
