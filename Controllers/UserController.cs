using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using JwtAuthDemo.Helpers;
using Microsoft.AspNetCore.Identity;
using TemplateApi.Models;
using TemplateApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.DirectoryServices.AccountManagement;

namespace Quickly_PriceQuotationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IMapper mapper, JwtHelpers jwt, IConfiguration config, UserManager<User> userManager, IPasswordHasher<User> passwordHasher) : ControllerBase
    {
        private IMapper _mapper = mapper;
        private JwtHelpers _jwt = jwt;
        private IConfiguration _config = config;
        private UserManager<User> _userManager = userManager;
        private IPasswordHasher<User> _passwordHasher = passwordHasher;

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(VUser vUser)
        {
            try
            {
                var vToken = new VToken();

                var user = await _userManager.FindByNameAsync(vUser.UserName);

                var userName = vUser.UserName;
                var password = vUser.Password;

                if (user != null && user.PasswordHash != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        vToken.AccessToken = await _jwt.GenerateTokenAsync(userName, 180);
                        vToken.RefreshToken = await _jwt.GenerateTokenAsync(userName, 600);
                        return Ok(vToken);
                    }
                }

                var context = new PrincipalContext(ContextType.Domain, _config.GetValue<string>("DomainAD"), userName, password);
                var adUser = UserPrincipal.FindByIdentity(context, userName);
                if (adUser != null)
                {
                    if (user == null)
                    {
                        var split = adUser.Name.Split(' ');
                        user = new User
                        {
                            UserName = userName,
                            Email = adUser.EmailAddress,
                            EmployeeName = adUser.Surname + adUser.GivenName,
                            DepartmentId = split[split.Length - 1],
                            EmployeeId = userName
                        };
                        var result = await _userManager.CreateAsync(user);
                    }
                    vToken.AccessToken = await _jwt.GenerateTokenAsync(userName, 180);
                    vToken.RefreshToken = await _jwt.GenerateTokenAsync(userName, 600);
                }
                
                return Ok(vToken);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}