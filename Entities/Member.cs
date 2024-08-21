using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Entities;

public class Member : IdentityUser
{
    public string MemberId
    {
        get { return base.Id; }
        set { base.Id = value; }
    }
    public decimal TotalCredit { get; set; }
}