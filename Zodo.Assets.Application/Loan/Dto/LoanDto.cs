using Newtonsoft.Json;
using System;
using HZC.Infrastructure;

namespace Zodo.Assets.Application
{
    public class LoanDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetName { get; set; }
        
        /// <summary>
        /// 原使用部门
        /// </summary>
        public string FromDeptName { get; set; }
        
        /// <summary>
        /// 原使用人
        /// </summary>
        public string FromAccountName { get; set; }
        
        /// <summary>
        /// 目标部门
        /// </summary>
        public string TargetDeptName { get; set; }
        
        /// <summary>
        /// 目标使用人
        /// </summary>
        public string TargetAccountName { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime LoanAt { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime ExpectedReturnAt { get; set; }

        public bool IsReturn { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? ReturnAt { get; set; }

    }
}
