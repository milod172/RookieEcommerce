namespace NovaFashion.API.Shared.Abstractions
{
    public interface IHasAudit
    {
        DateTime CreatedTime { get; set; }
        string? CreatedBy { get; set; }

        DateTime? ModifiedTime { get; set; }
        string? ModifiedBy { get; set; }
    }
}
