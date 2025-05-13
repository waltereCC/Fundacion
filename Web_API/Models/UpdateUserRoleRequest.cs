namespace Web_API.Models
{
    public class UpdateUserRoleRequest
    {
        public int UserId { get; set; }
        public string NewRole { get; set; }
    }
}
