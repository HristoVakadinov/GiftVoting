using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts.Services.DTOs.Authentication;

namespace Gifts.Services.Interfaces.Authentication
{
    public interface IAuthenticationService 
    {
        Task<LoginResponce> LoginAsync(LoginRequest request);
    }
}