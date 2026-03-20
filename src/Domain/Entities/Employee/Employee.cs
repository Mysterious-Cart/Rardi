namespace CHKS.Domain.Entities
{
    public record Employee(int Id, string Name, List<Group> Group);
}