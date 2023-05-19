using ASPIdentityManager.Data;
using ASPIdentityManager.Models;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace ASPIdentityManager.Controllers
{

    public class UserController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<IdentityUser> _userManager; 
        public UserController( ApplicationDBContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager; 
        }


        public IActionResult Index()
        {
            var userList = _db.ApplicationUser.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles =_db.Roles.ToList();
            foreach(var user in userList)
            {
                var role = userRole.FirstOrDefault(u=>u.UserId == user.Id);
                if(role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View(userList);
        }

        public IActionResult Edit( string userId)
        {
            //get the user obj form the DB
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u=>u.Id == userId);
            if(objFromDb == null)
            {
                return NotFound();
            }
            //get user Roles 
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            //check if any role has been assigned to the user
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if(role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u=>u.Id == role.RoleId).Id;
                //objFromDb.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
            }

            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {


                //get the user obj form the DB
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                //get user Roles 
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    //if user has the role, get the name
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();

                    //remove the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);

                //update the name of the user
                objFromDb.Name = user.Name;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been updated Successfully";
                return RedirectToAction(nameof(Index));
            }
           
            user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock(string userId)
        {
            //get the user obj form the DB
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now) {
                //user is locked and will remain locked untill lockoutEnd Date

                //click will unlock then
                objFromDb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User Unlocked Successfully";


            }
            else
            {
                //user is not locked and we want to lock
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User Locked Successfully";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            //get the user obj form the DB
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.ApplicationUser.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "User Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();  
            }
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel()
            {
                UserId = user.Id,
            };
            foreach(Claim claim in ClaimStore.claimList)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if(existingUserClaims.Any(c=>c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel userClaimsViewModel)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userClaimsViewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            if(!result.Succeeded)
            {
                TempData[SD.Error] = "Error while removing claims";
                return View(userClaimsViewModel);
            }
            result = await _userManager.AddClaimsAsync(user,
                userClaimsViewModel.Claims.Where(c=>c.IsSelected).Select(c=> new Claim(c.ClaimType,c.IsSelected.ToString())));

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while removing claims";
                return View(userClaimsViewModel);
            }

            TempData[SD.Success] = "Claims updated successfully";
            return RedirectToAction (nameof(Index));
        }

    }
}
