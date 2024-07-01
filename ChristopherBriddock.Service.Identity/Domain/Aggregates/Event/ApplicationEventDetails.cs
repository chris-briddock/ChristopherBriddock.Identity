using Domain.Aggregates.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Aggregates.Events;

namespace Domain.Aggregates.Event;

public class ApplicationEventDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationEventId { get; set; }
    public ApplicationEvent ApplicationEvent { get; set; } = default!;

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
}
