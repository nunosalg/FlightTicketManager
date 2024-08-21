using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Description { get; set; }


        [Required]
        [MaxLength(50)]
        public string Airline { get; set; }


        [Required]
        public int Capacity { get; set; }


        [Display(Name = "Image")]
        public string ImageUrl { get; set; } 


        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


        public User User { get; set; }


        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "~/images/noimage.png";
                }

                return $"https://localhost:44306{ImageUrl.Substring(1)}";
            }
        }
    }
}
