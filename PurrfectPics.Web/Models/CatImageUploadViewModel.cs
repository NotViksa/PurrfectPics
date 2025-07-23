using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PurrfectPics.Web.ViewModels
{
    public class CatImageUploadViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }

        public string Tags { get; set; }
    }
}