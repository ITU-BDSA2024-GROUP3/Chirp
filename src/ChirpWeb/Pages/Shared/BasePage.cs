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
        if (User.Identity.IsAuthenticated)
        {
            username = await _service.GetNameByEmail(User.Identity.Name);
        }
    }
}