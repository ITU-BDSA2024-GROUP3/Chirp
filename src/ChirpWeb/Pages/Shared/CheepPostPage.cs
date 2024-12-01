using System.ComponentModel.DataAnnotations;
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
    public string Text { get; set; }

  
    public CheepPostPage(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo)
    {
    }
    
    public async Task<ActionResult> OnPost()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("Public");
        }
        
        if (!ModelState.IsValid)
        {
            return RedirectToPage("Public");
        }

        Author author = _AuthorRepo.ReadAuthorByEmail(User.Identity.Name).Result;

        if (author.UserId == null)
        {
         Console.WriteLine("Userid is null");   
        }
        
        if (Text == null)
        {
            Console.WriteLine("Text is null");   
        }
        
        CheepDTO newCheep = new CheepDTO() { Text = Text, UserId = author.UserId};
        await _CheepRepo.CreateCheep(newCheep);
        
        return RedirectToPage("Public");
    }
    
    public async Task<ActionResult> OnPostToggleFollowAsync(string AuthorName, string CurrentPage, string? CurrentAuthorID)
    {
        Author loggedInAuthor = _AuthorRepo.ReadAuthorByEmail(User.Identity.Name).Result;
        Author followAuthor = _AuthorRepo.ReadAuthorByName(AuthorName).Result;
        if (loggedInAuthor == null || followAuthor == null)
        {
            throw new Exception("OnPostToggleFollowAsync Exception");
        }
        
        if (loggedInAuthor.FollowingList.Contains(followAuthor.UserId))
        {
            
            await _AuthorRepo.Unfollow(loggedInAuthor.UserId, followAuthor.UserId);

        }
        else
        {
            await _AuthorRepo.Follow(loggedInAuthor.UserId, followAuthor.UserId);
            
            
        }
       
        string page = CurrentPage;
        if (CurrentAuthorID != null)
        {
            var cheeps = await _CheepRepo.ReadCheeps(Int32.Parse(page), int.Parse(CurrentAuthorID));
            while (cheeps.Count == 0)
            {
                page = (Int32.Parse(page)-1).ToString();
                cheeps = await _CheepRepo.ReadCheeps(Int32.Parse(page), int.Parse(CurrentAuthorID));

            }
        }
        
        return Redirect($"?page={page}");//move logic up to constructor

    }
    
}