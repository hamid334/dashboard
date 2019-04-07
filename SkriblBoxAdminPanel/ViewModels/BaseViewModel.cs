using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.ViewModels
{
    public abstract class BaseViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int UserId { get; set; }
        public int StoreId { get; set; }
        public RoleTypes Role { get; set; }

        public void SetSharedData(IPrincipal User)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");
            UserName = fullName == null ? "John Doe" : fullName.Value;
            ProfilePictureUrl = string.IsNullOrEmpty(profilePictureUrl.Value) ? null : profilePictureUrl.Value;
            Role = (RoleTypes)(Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value));


            if ((int)Role == 3 || (int)Role == 2)
            {
                UserId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "AdminId").Value);
                StoreId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "StoreId").Value);
            }
            else
            {
                UserId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);
            }


        }
    }
}