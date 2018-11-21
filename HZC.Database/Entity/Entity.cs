using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace HZC.Database
{
    public class Entity<T>
    {   
        [MyDataField(IsPrimaryKey = true)]
        public T Id { get; set; }

        public bool IsDel { get; set; }

        [MyDataField(UpdateIgnore = true)]
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime CreateAt { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public T CreateBy { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string Creator { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime UpdateAt { get; set; }

        public T UpdateBy { get; set; }

        public string Updator { get; set; }

        public virtual void BeforeCreate(IAppUser<T> user)
        {
            CreateAt = DateTime.Now;
            CreateBy = user.Id;
            Creator = user.Name;
            UpdateAt = DateTime.Now;
            UpdateBy = user.Id;
            Updator = user.Name;
        }

        public virtual void BeforeUpdate(IAppUser<T> user)
        {
            UpdateAt = DateTime.Now;
            UpdateBy = user.Id;
            Updator = user.Name;
        }
    }

    public class Entity : Entity<int>
    { }
}
