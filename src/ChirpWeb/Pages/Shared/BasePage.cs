﻿using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

/// <summary>
///This is for all pages that require the user's username
/// </summary>

public class BasePage : PageModel
{
    protected readonly ICheepRepository _CheepRepo;
    protected readonly IAuthorRepository _AuthorRepo;
    
    public AuthorDTO? LoggedInAuthor { get; set; }
    
    public BasePage(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo)
    {
        _CheepRepo = CheepRepo;
        _AuthorRepo = AuthorRepo;
    } 
    
    /// <summary>
    ///Sets the client's username as a variable that can be accessed later
    /// </summary>
    public async void TrySetLoggedInAuthor() {
        if (User.Identity!.IsAuthenticated && LoggedInAuthor == null)
        {
            LoggedInAuthor = await _AuthorRepo.ReadAuthorDTOByEmail(User.Identity.Name!);//User.Identity.Name is the email
        }
    }
}