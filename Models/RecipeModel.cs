namespace WebApplication2.Models
{
    public enum RecipeStatusType
    {
        Testing,
        Rejected,
        RolledOut,
        Archived
    }
    public class RecipeModel
    {
        Guid RecipeId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public List<string>? Ingredients { get; set; }

        public List<string>? Instructions { get; set; }

        public List<string>? Tags { get; set; }

        public RecipeStatusType? Status { get; set; }
    }
}
