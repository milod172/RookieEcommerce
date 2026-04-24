namespace NovaFashion.API.Shared.Abstractions
{
    public interface IHasAudit
    {
        DateTime CreatedTime { get; set; }
        string? CreatedBy { get; set; }

        DateTime? ModifiedTime { get; set; }
        string? ModifiedBy { get; set; }

        DateTime? DeletedAt { get; set; }
        bool IsDeleted { get; set; }
        string? DeletedBy { get; set; }
    }
}
