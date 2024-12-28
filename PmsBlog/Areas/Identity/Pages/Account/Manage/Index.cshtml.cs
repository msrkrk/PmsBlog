// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PmsBlog.Areas.Identity.Data;

namespace PmsBlog.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<PmsBlogUser> _userManager;
        private readonly SignInManager<PmsBlogUser> _signInManager;
       

        public IndexModel(
            UserManager<PmsBlogUser> userManager,
            SignInManager<PmsBlogUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Fullname")]
            public string FullName { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }
        }

        private async Task LoadAsync(PmsBlogUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var fullName = claims.FirstOrDefault(x=> x.Type == "FullName")?.Value;
            var description = claims.FirstOrDefault(x => x.Type == "Description")?.Value;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = fullName,
                Description = description
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var fullNameClaim = claims.FirstOrDefault(x => x.Type == "FullName");
            var fullName = fullNameClaim?.Value;
            if (Input.FullName != fullName)
            {
                if(fullNameClaim is not null)
                {
                    await _userManager.RemoveClaimAsync(user, fullNameClaim);
                }
               
                var setFullNameResult = await _userManager.AddClaimAsync(user, new Claim("FullName",Input.FullName));

                if (!setFullNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set fullname.";
                    return RedirectToPage();
                }
            }

            var descriptionClaim = claims.FirstOrDefault(x => x.Type == "Description");
            var description = descriptionClaim?.Value;
            if (Input.Description != description)
            {
                if(descriptionClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, descriptionClaim);
                }
                var setDescriptionResult = await _userManager.AddClaimAsync(user, new Claim("Description", Input.Description));

                if (!setDescriptionResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set description.";
                    return RedirectToPage();
                }
            }





            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
