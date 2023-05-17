using System.ComponentModel.DataAnnotations;

namespace RestApi.Dtos
{
    public class UserForCreateDto 
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Designation is required.")]     
        public string Designation { get; set; }
       
    }
}
