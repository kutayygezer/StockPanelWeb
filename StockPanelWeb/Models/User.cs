using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPanelWeb.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DisplayName("Username")]
		public string UserName { get; set; }
		[Required]
		[DisplayName("User Password")]
		public string UserPassword { get; set; }
		[Required]
		[DisplayName("User Type")]
		public string? UserType { get; set; }
		public bool KeepLoggedIn { get; set; }
	}
}
