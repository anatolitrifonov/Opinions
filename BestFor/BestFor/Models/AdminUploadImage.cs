using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BestFor.Models
{
    /// <summary>
    /// This is used when testing image upload from admin
    /// </summary>
    public class AdminUploadImage
    {
        public string UserNameImage { get; set; }

        public IFormFile TheImageToUpload { get; set; }
    }
}
