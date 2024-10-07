using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "City")]
        public string Name { get; set; }


        [Required]
        [StringLength(2)]
        [Display(Name = "Country")]
        public string CountryCode { get; set; }


        [Display(Name = "Country Flag")]
        public string FlagUrl
        {
            get
            {
                return $"https://flagsapi.com/{CountryCode}/shiny/64.png";
            }
        }
    }
}
