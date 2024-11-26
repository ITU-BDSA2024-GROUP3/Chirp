using System.ComponentModel.DataAnnotations;
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
    
    public CheepPostPage(ICheepService service) : base(service)
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

        AuthorDTO author = _service.ReadAuthorDTOByEmail(User.Identity.Name).Result;

        if (author.UserId == null)
        {
         Console.WriteLine("Userid is null");   
        }
        
        if (Text == null)
        {
            Console.WriteLine("Text is null");   
        }
        
        CheepDTO newCheep = new CheepDTO() { Text = Text, AuthorID = author.UserId};
        await _service.CreateCheep(newCheep);
        
        return RedirectToPage("Public");
    }
    
    public async Task<ActionResult> OnPostToggleFollowAsync(string AuthorName)
    {
        Author loggedInAuthor = _service.ReadAuthorByEmail(User.Identity.Name).Result;
        Author followAuthor = _service.ReadAuthorByName(AuthorName).Result;
        if (loggedInAuthor == null || followAuthor == null)
        {
            throw new Exception("OnPostToggleFollowAsync Exception");
        }
        
        if (loggedInAuthor.FollowingList.Contains(followAuthor.UserId))
        {
            await _service.Unfollow(loggedInAuthor.UserId, followAuthor.UserId);
        }
        else
        {
            await _service.Follow(loggedInAuthor.UserId, followAuthor.UserId);
        }
        
        return RedirectToPage("Public");
    }
}