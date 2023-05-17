using System.ComponentModel.DataAnnotations;

namespace RestApi.Entities
{
    public class User
    {
        [Key]
        public int EmployeeID { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }
    }
}
