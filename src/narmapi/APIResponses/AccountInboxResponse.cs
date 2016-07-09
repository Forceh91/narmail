using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class AccountInboxResponse
    {
        [DataMember(Name = "kind")]
        public string kind { get; set; }

        [DataMember(Name = "data")]
        public InboxData data { get; set; }
    }

    [DataContract]
    public class InboxData
    {
        [DataMember(Name = "mod_hash")]
        public string modHash { get; set; }

        [DataMember(Name = "children")]
        public ObservableCollection<Message> messages { get; set; }

        [DataMember(Name = "before")]
        public string before { get; set; }

        [DataMember(Name = "after")]
        public string after { get; set; }
    }
}
