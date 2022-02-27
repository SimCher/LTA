using System.Collections.Generic;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class User : BaseModel
    {
        private bool _isOnline;
        private string _code;

        private Color _color;

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public bool IsOnline
        {
            get => _isOnline;
            set => _isOnline = value;
        }

        public ICollection<Topic> UserTopics { get; set; }
        public ICollection<Topic> SaveTopics { get; set; }
    }
}