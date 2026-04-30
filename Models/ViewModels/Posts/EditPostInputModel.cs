using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.ViewModels.Posts
{
    public class EditPostInputModel
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        [StringLength(1000)]
        public string Caption { get; set; } = string.Empty;
    }
}
