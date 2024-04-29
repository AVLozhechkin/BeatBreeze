namespace BeatBreeze.Core.Models;

public abstract record BaseEntity
{
    public Guid Id { get; set; }
}
