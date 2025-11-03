using System.ComponentModel.DataAnnotations;

namespace Contract.Dto;

public class PaginationRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int CurrentPage { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; }
}
