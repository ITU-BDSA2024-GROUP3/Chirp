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
        Author loggedInAuthor = await _service.ReadAuthorByEmail(User.Identity.Name);
        Console.WriteLine("please be correct: " + AuthorName);
        Author followAuthor = await _service.ReadAuthorByName(AuthorName);

        if (loggedInAuthor == null || followAuthor == null)
        {
            Console.WriteLine(loggedInAuthor.Name + " exists");
            Console.WriteLine(followAuthor.Name + " exists");
            throw new Exception("OnPostToggleFollowAsync Exception");
        }
        
        if (loggedInAuthor.FollowingList == null)
        {
            loggedInAuthor.FollowingList = new List<Author>();
        }
        
        if (loggedInAuthor.FollowingList.Contains(followAuthor))
        {
            _service.Unfollow(loggedInAuthor, followAuthor);

        }
        else
        {
            _service.Follow(loggedInAuthor, followAuthor);
        }

        return Page();
    }
}