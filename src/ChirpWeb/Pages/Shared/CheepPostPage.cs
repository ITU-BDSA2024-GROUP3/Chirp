﻿using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class CheepPostPage : BasePage
{
    [BindProperty]
    [Required]
    [MaxLength(160)]
    public string? Text { get; set; }
    

  
    public CheepPostPage(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo)
    {
    }
    
    public async Task<ActionResult> OnPost()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToPage("Public");
        }
        
        if (!ModelState.IsValid)
        {
            return RedirectToPage("Public");
        }

        TrySetLoggedInAuthor();

        if (LoggedInAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        if (Text == null)
        {
            throw new Exception("Text is required");
        }
        
        CheepDTO newCheep = new CheepDTO(text: Text, userId: LoggedInAuthor.UserId, cheepId: -1 );
        await _CheepRepo.CreateCheep(newCheep);
        
        return RedirectToPage("Public");
    }
    
    public async Task<ActionResult> OnPostToggleFollowAsync(string AuthorName, string CurrentPage, string? CurrentAuthorID)
    {
        TrySetLoggedInAuthor();

        if (LoggedInAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        AuthorDTO? followAuthor = await _AuthorRepo.ReadAuthorDTOByName(AuthorName);

        if (followAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        if (LoggedInAuthor.FollowingList.Contains(followAuthor.UserId))
        {
            await _AuthorRepo.Unfollow(LoggedInAuthor.UserId, followAuthor.UserId);
        }
        else
        {
            await _AuthorRepo.Follow(LoggedInAuthor.UserId, followAuthor.UserId);
        }
       
        string page = CurrentPage;
        if (CurrentAuthorID != null)
        {
            var cheeps =  _CheepRepo.ReadCheeps(Int32.Parse(page), int.Parse(CurrentAuthorID));
            while (cheeps.Count == 0)
            {
                page = (Int32.Parse(page)-1).ToString();
                cheeps =  _CheepRepo.ReadCheeps(Int32.Parse(page), int.Parse(CurrentAuthorID));

            }
        }
        
        return Redirect($"?page={page}");//move logic up to constructor
    }
    
    public async Task<ActionResult> OnPostToggleLikeAsync(int CheepId, int CurrentPage)
    {
        TrySetLoggedInAuthor();

        if (LoggedInAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        Cheep? cheep = await _CheepRepo.ReadCheepByCheepId(CheepId);

        if (cheep == null)
        {
            // should be handled more gracefully
            throw new Exception("Cheep not recognized!");
        }
        
        if (LoggedInAuthor == null)
        {
            throw new Exception("OnPostToggleLikeAsync Exception: loggedInAuthor is null");
        }

        if (cheep == null)
        {

            throw new Exception("OnPostToggleLikeAsync Exception: cheep is null "+CheepId);
        }

        if (cheep.AuthorLikeList!.Contains(LoggedInAuthor.UserId))
        {
            await _CheepRepo.UnLikeCheep(cheep.CheepId, LoggedInAuthor.UserId);
        }
        else
        {
            await _CheepRepo.LikeCheep(cheep.CheepId, LoggedInAuthor.UserId);
        }
        
        return Redirect($"?page={CurrentPage}");//move logic up to constructor
    }

    public async Task<int> GetCheepLike(int CheepId)
    {
        int temp = await _CheepRepo.AmountOfLikes(CheepId);
        return temp;
    }
    
}