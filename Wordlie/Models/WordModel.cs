using System.ComponentModel.DataAnnotations;
namespace Wordlie.Models;

public class WordModel
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Word { get; set; }
    
}