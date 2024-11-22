// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChirpCore.DomainModel;
using ChirpWeb.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ChirpWeb.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : BasePage
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<Author> userManager,
            ILogger<DownloadPersonalDataModel> logger, ICheepService service) : base(service)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(Author).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                if (p.Name == "Cheeps")
                {
                    ICollection<CheepDTO> list = await _service.GetAllCheepsFromAuthor(user.UserId);
                    string cheepData = null;
                    foreach (var cheep in list)
                    {
                        cheepData = cheepData + "\n Cheep Text: " + cheep.Text + ". Cheep timestamp: " +
                                    cheep.TimeStamp.ToString();
                    }
                    
                    personalData.Add(p.Name, cheepData);

                }
                else
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }
    }
}
