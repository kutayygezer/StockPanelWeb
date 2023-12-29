using System.ComponentModel.DataAnnotations;

namespace StockPanelWeb.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LogUser { get; set; }
        public DateTime LogTime { get; set; }
        public string LogMessage { get; set; }
    }
}
