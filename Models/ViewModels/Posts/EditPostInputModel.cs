using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.ViewModels.Posts
{
    public class EditPostInputModel
    {
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }

        [StringLength(1000)]
        public string Caption { get; set; } = string.Empty;
    }
}
