using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Models
{
    public class SignIn
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [StringLength(maximumLength: 255, MinimumLength = 10, ErrorMessage = "Nhập email từ 10 - 255 kí tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(maximumLength: 255, MinimumLength = 6, ErrorMessage = "Nhập mật khẩu từ 6 - 255 kí tự")]
        public string Password { get; set; }
    }
}
