using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPIdentityManager.Controllers
{
    [Authorize]
    public class AccessCheckerController : Controller
    {

        //Accessible by everyone, even if user are not logged in
        [AllowAnonymous]
        public IActionResult AllAccess()
        {
            return View();
        }

        //accessbile by logged in users
        [Authorize]
        public IActionResult AuthorizedAccess()
        {
            return View();
        }

        //accessbile by users who have user role
        [Authorize(Roles ="User")]
        public IActionResult UserAccess()
        {
            return View();
        }

        //accessbile by users Or Admin who have user role
        [Authorize(Roles = "User,Admin")]
        public IActionResult UserOrAdmiAccess()
        {
            return View();
        }

        //accessbile by users And Admin who have user role
        [Authorize(Policy = "UserAndAdmin")]
        public IActionResult UserAndAdmiAccess()
        {
            return View();
        }


        //accessbile by  user who have admin role
        [Authorize(Policy = "Admin")]
        public IActionResult AdminAccess()
        {
            return View();
        }

        //accessbile by Admin users with a claim of create to be True
        [Authorize(Policy = "Admin_createAccess")]
        public IActionResult Admin_createAccess()
        {
            return View();
        }

        //accessbile by Admin users with a claim of create edit and delete to be True 
        [Authorize(Policy = "Admin_create_edit_deleteAccess")]
        public IActionResult Admin_create_edit_deleteAccess()
        {
            return View();
        }

        //accessbile by Admin users with a claim of create edit and delete to be True  OR if the user is superAdmin 
        [Authorize(Policy = "Admin_create_edit_deleteAccess_SuperAdmin")]
        public IActionResult Admin_create_edit_deleteAccess_SuperAdmin()
        {
            return View();
        }

        //accessbile by Admin users with a claim of create edit and delete to be True  OR if the user is superAdmin 
        [Authorize(Policy = "AdminWithMoreThan100Days")]
        public IActionResult OnlyBhrugen()
        {
            return View();
        }

        //accessbile by Admin users with a claim of create edit and delete to be True  OR if the user is superAdmin 
        [Authorize(Policy = "FirsNameAuth")]
        public IActionResult FirsNameAuth()
        {
            return View();
        }

    }
}
