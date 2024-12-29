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
using Microsoft.EntityFrameworkCore;
using PmsBlog.Data;

namespace PmsBlog.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<PmsBlogUser> _userManager;
        private readonly SignInManager<PmsBlogUser> _signInManager;
        private readonly PmsBlogContext _context;

        public IndexModel(
            UserManager<PmsBlogUser> userManager,
            SignInManager<PmsBlogUser> signInManager,
            PmsBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public string ImageBase64 { get; set; }

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

            [Display(Name = "URL")]
            public string Url { get; set; }

            [BindProperty]
            public IFormFile ImageFile { get; set; }

        }

        private async Task LoadAsync(PmsBlogUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var dbUser = await _context.Users.FindAsync(user.Id);

            Username = userName;
            ImageBase64 = user.ProfilePictureBase64;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = user.FullName,
                Description = user.Description,
                Url = user.Url
                
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

            if (Input.FullName != user.FullName)
            {
                var dbUser = await _context.Users.FindAsync(user.Id);
                dbUser.FullName = Input.FullName;
                await _context.SaveChangesAsync();
            }

            if (Input.Description != user.Description)
            {
                var dbUser = await _context.Users.FindAsync(user.Id);
                dbUser.Description = Input.Description;
                await _context.SaveChangesAsync();
            }

            if(Input.Url != user.Url)
            {
                var dbUser = await _context.Users.FindAsync(user.Id);
                dbUser.Url = Input.Url;
                await _context.SaveChangesAsync();
            }

            if(Input.ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var dbUser = await _context.Users.FindAsync(user.Id);
                    await Input.ImageFile.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    dbUser.ProfilePictureBase64 = Convert.ToBase64String(fileBytes);
                    await _context.SaveChangesAsync();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
