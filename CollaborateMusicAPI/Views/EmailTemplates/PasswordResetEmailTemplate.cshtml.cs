using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollaborateMusicAPI.Views.EmailTemplates
{

    public class PasswordResetEmailTemplateModel : PageModel
    {
       
            public string Name { get; set; }
            public string ResetUrl { get; set; }


      

        
    }
}
