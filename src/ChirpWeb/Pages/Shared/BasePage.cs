using System.ComponentModel.DataAnnotations;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class BasePage : PageModel
{
    protected readonly ICheepService _service;
    
    public string username { get; set; }
    
    public BasePage(ICheepService service)
    {
        _service = service;
    }

    public async void setUsername()
    {
        if (User.Identity.IsAuthenticated && User.Identity.Name != null)
        {
            username = await _service.GetNameByEmail(User.Identity.Name);
        }
    }

    public Author GetAuthorByEmail(string authorName)
    {
        return _service.ReadAuthorByEmail(authorName).Result;
    }
}