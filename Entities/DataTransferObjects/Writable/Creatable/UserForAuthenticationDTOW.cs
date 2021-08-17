using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.Writable.Creatable
{
    public class UserForAuthenticationDTOW
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public  string Password { get; set; }
    }
}
