using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class AccountInformationResponse
    {
        [DataMember(Name = "has_mail")]
        public bool hasMail { get; set; }

        [DataMember(Name = "has_mod_mail")]
        public bool hasModMail { get; set; }

        [DataMember(Name = "hide_from_robots")]
        public bool hideFromRobots { get; set; }

        [DataMember(Name = "over_18")]
        public bool isOverEighteen { get; set; }

        [DataMember(Name = "is_gold")]
        public bool hasGold { get; set; }

        [DataMember(Name = "is_mod")]
        public bool isMode { get; set; }

        [DataMember(Name = "has_verified_email")]
        public bool hasVerifiedEmail { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "created")]
        public double created { get; set; }

        [DataMember(Name = "created_utc")]
        public double createdUTC { get; set; }

        [DataMember(Name = "gold_creddits")]
        public int goldCredits { get; set; }

        [DataMember(Name = "link_karma")]
        public int linkKarma { get; set; }

        [DataMember(Name = "comment_karma")]
        public int commentKarma { get; set; }

        [DataMember(Name = "inbox_count")]
        public int inboxCount { get; set; }
    }
}
