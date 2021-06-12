using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin, Super Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole newRole = new IdentityRole()
                {
                    Name = createRoleViewModel.RoleName
                };

                IdentityResult role = await roleManager.CreateAsync(newRole);
                if (role.Succeeded)
                {
                    return RedirectToAction("getroles", "administration");
                }

                foreach (var error in role.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(createRoleViewModel);
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                EditRoleViewModel editRole = new EditRoleViewModel()
                {
                    Id = role.Id,
                    RoleName = role.Name
                };

                IList<ApplicationUser> usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    editRole.Users = usersInRole.Select(u => u.UserName).ToList();
                }
                return View(editRole);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel editedRole)
        {
            if(ModelState.IsValid)
            {
                IdentityRole role = await roleManager.FindByIdAsync(editedRole.Id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {editedRole.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    role.Name = editedRole.RoleName;

                    IdentityResult editResult = await roleManager.UpdateAsync(role);
                    if (editResult.Succeeded)
                    {
                        return RedirectToAction("getroles");
                    }

                    foreach (var error in editResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(editedRole);
                }
            }

            return View(editedRole);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            List<UserRoleViewModel> userRoleList = new List<UserRoleViewModel>();

            IdentityRole role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            foreach (var user in userManager.Users.ToList())
            {
                UserRoleViewModel viewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = false
                };

                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    viewModel.IsSelected = true;
                }
                userRoleList.Add(viewModel);
            }

            return View(userRoleList);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> roleList, string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            foreach (var userRole in roleList)
            {
                ApplicationUser user = await userManager.FindByIdAsync(userRole.UserId);
                IdentityResult result = null;
                if (userRole.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!userRole.IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("editrole", new { roleId = role.Id});
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            IList<string> userRoles =  await userManager.GetRolesAsync(user);
            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            List<string> userClaimsList = userClaims.Select(c => c.Type + " : " + c.Value).ToList();

            EditUserViewModel editUser = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Roles = userRoles,
                Claims = userClaimsList
            };

            return View(editUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel editedUser)
        {
            ApplicationUser user = await userManager.FindByIdAsync(editedUser.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {editedUser.Id} cannot be found";
                return View("NotFound");
            }

            user.UserName = editedUser.UserName;
            user.Email = editedUser.Email;
            user.City = editedUser.City;

            IdentityResult result = await userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction("getusers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(editedUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            else
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("getusers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return RedirectToAction("getusers");
            }            
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    IdentityResult result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("getroles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return RedirectToAction("getroles");
                }
                catch(DbUpdateException ex)
                {
                    logger.LogError($"Exception Occured : {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("Error");
                }
            }
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            ViewBag.userId = userId;

            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var result = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles.ToList())
            {
                var viewModel = new UserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = false
                };

                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    viewModel.IsSelected = true;
                }

                result.Add(viewModel);
            }

            return View(result);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageRoles(List<UserRolesViewModel> userRoles, string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var currentUserRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, currentUserRoles);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(userRoles);
            }
            var selectedRolesList = userRoles.Where(r => r.IsSelected).Select(r => r.RoleName);
            result = await userManager.AddToRolesAsync(user, selectedRolesList);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(userRoles);
            }

            return RedirectToAction("EditUser", new { userId = userId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageClaims(string userId)
        {
            ViewBag.userId = userId;

            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var allClaims = ClaimsStore.AllClaims;
            var userClaims = new List<UserClaim>();
            var claimsForUser = await userManager.GetClaimsAsync(user);

            foreach (var claim in allClaims)
            {
                var usrClaim = new UserClaim()
                {
                    ClaimType = claim.Type,
                    IsSelected = false
                };

                if(claimsForUser.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    usrClaim.IsSelected = true;
                }

                userClaims.Add(usrClaim);
            }

            var result = new UserClaimsViewModel
            {
                UserId = userId,
                Claims = userClaims
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClaims(UserClaimsViewModel userClaimsViewModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userClaimsViewModel.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userClaimsViewModel.UserId} cannot be found";
                return View("NotFound");
            }

            var currentUserClaims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, currentUserClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(userClaimsViewModel);
            }
            var selectedClaimsList = userClaimsViewModel.Claims.Select(r => new Claim(r.ClaimType, r.IsSelected ? "true" : "false"));
            result = await userManager.AddClaimsAsync(user, selectedClaimsList);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(userClaimsViewModel);
            }

            return RedirectToAction("EditUser", new { userClaimsViewModel.UserId });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
