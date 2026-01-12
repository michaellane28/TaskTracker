using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string Status { get; set; } = "Open";

    [Required]
    public string Priority { get; set; } = "Medium";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}