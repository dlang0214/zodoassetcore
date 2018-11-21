using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Models
{
    public class ChangePwViewModel
    {
        [Display(Name = "原始密码")]
        public string OldPw { get; set; }

        [Display(Name = "新密码")]
        [Required(ErrorMessage = "新密码不能为空"), MinLength(6, ErrorMessage = "密码必须大于等于6位")]
        public string NewPw { get; set; }

        [Display(Name = "重复密码")]
        [Required(ErrorMessage = "重复密码不能为空"), Compare(nameof(NewPw), ErrorMessage = "重复密码必须与新密码一致") ]
        public string RepeatPw { get; set; }
    }
}
