using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPanelWeb.Models
{
    public class LoginViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }
        [Required]
        [DisplayName("User Password")]
        public string UserPassword { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
