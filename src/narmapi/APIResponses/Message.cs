using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "kind")]
        public string kind { get; set; }

        [DataMember(Name = "data")]
        public MessageData data { get; set; }
    }

    [DataContract]
    public class MessageData
    {
        [DataMember(Name = "body")]
        public string body { get; set; }

        [DataMember(Name = "body_html")]
        public string bodyHTML { get; set; }

        [DataMember(Name = "was_comment")]
        public bool wasComment { get; set; }

        [DataMember(Name = "first_message")]
        public long? firstMessage { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "first_message_name")]
        public string firstMessageName { get; set; }

        [DataMember(Name = "created")]
        public double created { get; set; }

        [DataMember(Name = "created_utc")]
        public double createdUTC { get; set; }

        [DataMember(Name = "dest")]
        public string dest { get; set; }

        [DataMember(Name = "author")]
        public string author { get; set; }

        [DataMember(Name = "subreddit")]
        public string subreddit { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "parent_id")]
        public string parentID { get; set; }

        [DataMember(Name = "likes")]
        public string likes { get; set; }

        [DataMember(Name = "context")]
        public string context { get; set; }

        [DataMember(Name = "replies")]
        public string replies { get; set; }

        [DataMember(Name = "new")]
        public bool unread { get; set; }

        [DataMember(Name = "distinguished")]
        public string distinguished { get; set; }

        [DataMember(Name = "subject")]
        public string subject { get; set; }

        [DataMember(Name = "link_title")]
        public string linkTitle { get; set; }
    }
}
