// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
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

        public List<CheepDTO> Cheeps { get; set; }
        
        public Author author { get; set; }
        public int currentPage;


        public PersonalDataModel(
            UserManager<Author> userManager,
            ILogger<PersonalDataModel> logger, ICheepService service) : base(service)
        {
            _userManager = userManager;
            _logger = logger;
            
        }

        public async Task<IActionResult> OnGet([FromQuery] int page)
        {
            setUsername();
            
            var authorTask = await _service.ReadAuthorByEmail(User.Identity.Name);

            var authorTaskGetAuthor = await _service.GetAuthorById(authorTask.UserId);

            var cheepsTask = await _service.GetCheepsFromAuthor(authorTask.UserId, page);
        
            author = authorTaskGetAuthor;
            Cheeps = cheepsTask;
            
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
