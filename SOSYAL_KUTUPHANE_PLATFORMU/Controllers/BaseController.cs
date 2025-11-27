using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
       
   if (userIdClaim == null)
      throw new UnauthorizedAccessException("Kullanýcý kimliði bulunamadý.");

      return int.Parse(userIdClaim.Value);
        }

      protected string GetCurrentUserName()
   {
    var userNameClaim = User.FindFirst(ClaimTypes.Name);
      
    if (userNameClaim == null)
  throw new UnauthorizedAccessException("Kullanýcý adý bulunamadý.");

    return userNameClaim.Value;
        }
    }
}
