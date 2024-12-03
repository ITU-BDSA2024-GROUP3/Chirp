using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class CheepPostPage : BasePage
{
    [BindProperty]
    [Required]
    [MaxLength(160)]
    public string Text { get; set; }
  
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
    
    public async Task<ActionResult> OnPostToggleFollowAsync(string AuthorName)
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
        
        return RedirectToPage("Public");
    }
    
    public async Task<ActionResult> OnPostToggleLikeAsync(int CheepId)
    {

        Author loggedInAuthor = _AuthorRepo.ReadAuthorByEmail(User.Identity.Name).Result;
        Cheep cheep = await _CheepRepo.ReadCheepByCheepId(CheepId);
        if (loggedInAuthor == null)
        {
            throw new Exception("OnPostToggleLikeAsync Exception: loggedInAuthor is null");
        }

        if (cheep == null)
        {

            throw new Exception("OnPostToggleLikeAsync Exception: cheep is null "+CheepId);
        }

        if (cheep.AuthorLikeList.Contains(loggedInAuthor.UserId))
        {
            await _CheepRepo.UnLikeCheep(cheep.CheepId, loggedInAuthor.UserId);
        }
        else
        {
            await _CheepRepo.LikeCheep(cheep.CheepId, loggedInAuthor.UserId);
        }

        return RedirectToPage("Public");
    }

    public async Task<int> GetCheepLike(int CheepId)
    {
        int temp = _CheepRepo.AmountOfLikes(CheepId).Result;
        return temp;
    }
    
}