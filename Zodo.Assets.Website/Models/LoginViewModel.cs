using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Name { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "必须是6位以上文本")]
        public string Pw { get; set; }
    }
}
