namespace Razor_Front_End.Models
{
    public class UpdateUserRoleRequest
    {
        public int UserId { get; set; }
        public string NewRole { get; set; }
    }
}
