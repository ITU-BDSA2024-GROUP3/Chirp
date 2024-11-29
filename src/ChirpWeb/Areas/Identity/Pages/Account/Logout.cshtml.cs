// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using ChirpCore;
using ChirpCore.DomainModel;
using ChirpWeb.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ChirpWeb.Areas.Identity.Pages.Account
{
    public class LogoutModel : BasePage
    {
        private readonly SignInManager<Author> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<Author> signInManager, ILogger<LogoutModel> logger, ICheepRepository CheepRepo, IAuthorRepository AuthorRepo): base(CheepRepo, AuthorRepo)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async void OnGetAsync()
        {
            TrySetLoggedInAuthor();
        }
        
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
