using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Aggregates.Events;

public sealed class ApplicationEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;

    public Guid? CreatedBy { get; set; } = null!;

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

}
