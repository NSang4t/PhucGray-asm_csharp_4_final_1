using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Models
{
    public class User : SignIn
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(maximumLength: 255, MinimumLength = 1, ErrorMessage = "Nhập tên từ 3 - 255 kí tự")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(maximumLength: 15, MinimumLength = 10, ErrorMessage = "Nhập số điện thoại từ 10 - 15 kí tự")]
        public string Phone { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
