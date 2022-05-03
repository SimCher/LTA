using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LTA.Mobile.Domain.Models.BaseModels;

namespace LTA.Mobile.Domain.Models
{
    public class User : BaseModel
    {
        private string _code;

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        [NotMapped]
        public string Alias
        {
            get => _code.Substring(0, 6);
        }

        public ICollection<Topic> UserTopics { get; set; }
        public ICollection<Topic> SaveTopics { get; set; }
    }
}