using ASPIdentityManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPIdentityManager.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(ApplicationDBContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();

            return View(roles);
        }

        [HttpGet]
        public IActionResult Upsert( string id)
        {

            if(string.IsNullOrEmpty(id)) 
            {
                return View();
            }
            else
            {
                //update
                var obj = _db.Roles.FirstOrDefault(u => u.Id == id);
                return View(obj);   
            }
        }

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChecker")]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Upsert(IdentityRole roleObj)
        {
           if( await _roleManager.RoleExistsAsync(roleObj.Name) )
            {
                //error
                TempData[SD.Error] = "Role already exists";
                return RedirectToAction(nameof(Index));
            } 
           if(string.IsNullOrEmpty(roleObj.Id))
            {
                //create
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleObj.Name});
                TempData[SD.Success] = "Role created successfully";
            }
            else
            {
                //update the role
                var objroleFromDb = _db.Roles.FirstOrDefault(u=>u.Id == roleObj.Id);
                if(objroleFromDb == null)
                {
                    TempData[SD.Error] = "Role not found";
                    return RedirectToAction(nameof(Index));
                }
                objroleFromDb.Name = roleObj.Name;
                objroleFromDb.NormalizedName = roleObj.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(objroleFromDb);
                TempData[SD.Success] = "Role updated successfully";
            }
           return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChecker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var objroleFromDb = _db.Roles.FirstOrDefault(u => u.Id == id);
            var userRolesForThisRole = _db.UserRoles.Where(u => u.RoleId == id).Count();  

            if(userRolesForThisRole > 0)
            {
                TempData[SD.Error] = "Cannot delete this role, since there are users assigned to this role";
                return RedirectToAction(nameof(Index));
            }
            if(objroleFromDb == null) {
                TempData[SD.Error] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(objroleFromDb);
            TempData[SD.Success] = "Role deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
 