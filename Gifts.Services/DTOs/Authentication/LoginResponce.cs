using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifts.Services.DTOs.Authentication
{
    public class LoginResponce
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FullName { get; set; }
        public int? UserId { get; set; }
    }
}