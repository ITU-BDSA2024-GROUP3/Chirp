using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class BasePage : PageModel
{
    protected readonly ICheepRepository _repo;
    
    public string username { get; set; }
    
    public BasePage(ICheepRepository repo)
    {

        _repo = repo;  
    } 
    
    public async void setUsername() {
        if (User.Identity.IsAuthenticated && User.Identity.Name != null)
        {
            username = await _repo.GetNameByEmail(User.Identity.Name);
        }
    }





}