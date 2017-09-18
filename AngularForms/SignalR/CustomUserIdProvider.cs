using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BrasaoHamburgueriaWeb.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            string userId = "";

            if (request.User != null)
            {
                userId = request.User.Identity.Name;
            }
            
            return userId;
        }
    }
}