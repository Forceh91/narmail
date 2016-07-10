using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace narmapi.APIResponses
{
    [DataContract]
    public class AccountFriendsResponse
    {
        [DataMember(Name = "kind")]
        public string kind { get; set; }

        [DataMember(Name = "data")]
        public AccountFriendsList data { get; set; }
    }

    [DataContract]
    public class AccountFriendsList
    {
        [DataMember(Name = "children")]
        public ObservableCollection<FriendData> friends { get; set; }
    }

    [DataContract]
    public class FriendData
    {
        [DataMember(Name = "date")]
        public double date { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }
    }
}
