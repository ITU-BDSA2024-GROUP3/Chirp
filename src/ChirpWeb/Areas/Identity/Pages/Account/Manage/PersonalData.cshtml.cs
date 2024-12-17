// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using ChirpCore;
using ChirpCore.DomainModel;
using ChirpWeb.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ChirpWeb.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : BasePage
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public List<CheepDTO>? Cheeps { get; set; }
        
        public Author Author { get; set; }
        public List<AuthorDTO> FollowingAuthors { get; set; }

        public int currentPage;

        #pragma warning disable 8618
        public PersonalDataModel(
            UserManager<Author> userManager,
            ILogger<PersonalDataModel> logger, ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo)
        {
            _userManager = userManager;
            _logger = logger;
            
        }
        #pragma warning restore 8618

        public async Task<IActionResult> OnGet([FromQuery] int page)
        {
            TrySetLoggedInAuthor();

            if (LoggedInAuthor == null)
            {
                throw new Exception("User not recognized!");
            }
            
            var authorTaskGetAuthor = await _AuthorRepo.ReadAuthorById(LoggedInAuthor.UserId);
            var cheepsTask = _CheepRepo.ReadCheeps(page, LoggedInAuthor.UserId);
            var followingTask = await _CheepRepo.ReadFollowingAsync(LoggedInAuthor.UserId);
        
            Author = authorTaskGetAuthor!;
            Cheeps = cheepsTask;
            FollowingAuthors = followingTask;
            
            currentPage = page;

            if (currentPage < 1)
            {
                currentPage = 1;
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
