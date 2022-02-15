using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Range(1, double.PositiveInfinity, ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(maximumLength: 255, MinimumLength = 10, ErrorMessage = "Vui lòng nhập tên sản phẩm từ 10 - 255 kí tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập alias")]
        [StringLength(maximumLength: 255, MinimumLength = 10, ErrorMessage = "Vui lòng nhập alias từ 10 - 255 kí tự")]
        public string Alias { get; set; }

        [StringLength(maximumLength: 255, ErrorMessage = "Vui lòng nhập tên thương hiệu từ 10 - 255 kí tự")]
        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        public string Brand { get; set; }

        [Range(1, double.PositiveInfinity, ErrorMessage = "Vui lòng nhập giá nhập kho > 0")]
        public double ImportPrice { get; set; }

        [Range(1, double.PositiveInfinity, ErrorMessage = "Vui lòng nhập giá > 0")]
        public double Price { get; set; }
        public double? HotPrice { get; set; }

        [Range(1, double.PositiveInfinity, ErrorMessage = "Vui lòng nhập số lượng > 0")]
        public int Quantity { get; set; }

        [MinLength(10, ErrorMessage = "Vui lòng nhập mô tả ít nhất 10 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsTop { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFreeShip { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
