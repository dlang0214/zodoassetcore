using HZC.Infrastructure;
using System;

namespace Zodo.Assets.Application
{
    [Serializable]
    public class AppUserDto : IAppUser
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
