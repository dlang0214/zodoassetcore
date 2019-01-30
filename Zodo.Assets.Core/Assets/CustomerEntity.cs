using HZC.Database;

namespace Zodo.Assets.Core
{
    public class CustomerEntity : Entity
    {
        public string UserId { get; set; }

        public string CustomerUnit { get; set; }

        public string PinYin { get; set; }

        public string Abbr { get; set; }

        public string Contacts { get; set; }
    }
}
