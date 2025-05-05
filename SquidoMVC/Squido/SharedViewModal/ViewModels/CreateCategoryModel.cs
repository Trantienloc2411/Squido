using System.ComponentModel.DataAnnotations;

namespace SharedViewModal.ViewModels;

public class CreateCategoryModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MinLength(3)]
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = null!;
}