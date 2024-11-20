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
        Console.WriteLine("please be correct: " + loggedInAuthor.Name);
        Author followAuthor = _service.ReadAuthorByName(AuthorName).Result;
        Console.WriteLine("please be correct 2: " + followAuthor.Name);
        Console.WriteLine("why is my life like this: ");
        Console.WriteLine(followAuthor.FollowingList==null);
        if (loggedInAuthor == null || followAuthor == null)
        {
            throw new Exception("OnPostToggleFollowAsync Exception");
        }
        
        if (loggedInAuthor.FollowingList == null)
        {
            Console.WriteLine("test");
            loggedInAuthor.FollowingList = new List<Author>();
        }
        Console.WriteLine(loggedInAuthor.FollowingList.Count);
        foreach (var huh in loggedInAuthor.FollowingList)
        {
            Console.WriteLine(huh.Name);
        }
        if (loggedInAuthor.FollowingList.Contains(followAuthor))
        {
            Console.WriteLine("Unfollowing time");
            Console.WriteLine(loggedInAuthor.FollowingList.Count);
            await _service.Unfollow(loggedInAuthor, followAuthor);

        }
        else
        {
            await _service.Follow(loggedInAuthor, followAuthor);
            Console.WriteLine("uhhhhhhhhhhh " + loggedInAuthor.FollowingList.Count);
            Console.WriteLine(loggedInAuthor.UserName==User.Identity.Name);
            
        }
        
        return RedirectToPage("Public");
    }
}