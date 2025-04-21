namespace MMA.Domain
{
    public class AddUserRoleRequestDto
    {
        public Guid UserId { get; set; }
        public List<UserRoleProperty> UserRoles { get; set; } = new List<UserRoleProperty>();
    }
}