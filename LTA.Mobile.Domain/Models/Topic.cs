using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class Topic : BaseModel
    {
        public int[] UserIds { get; set; }

        private string _name;

        private bool _isSelected;

        private float _rating;

        private int _maxUsersNumber;

        private DateTime _lastEntryDate;

        private int _currentUsersNumber;
        private bool? _isFull;

        public Topic()
        {
            //TODO:: Return observable collection instead dictionary
            UsersIn = new Dictionary<string, Color>();
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public float Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public int MaxUsersNumber
        {
            get => _maxUsersNumber;
            set => SetProperty(ref _maxUsersNumber, value);
        }

        public DateTime LastEntryDate
        {
            get => _lastEntryDate;
            set => SetProperty(ref _lastEntryDate, value);
        }

        public int CurrentUsersNumber
        {
            get => _currentUsersNumber;
            set => SetProperty(ref _currentUsersNumber, value);
        }

        public bool? IsFull
        {
            get
            {
                return CurrentUsersNumber switch
                {
                    0 => null,
                    > 0 when CurrentUsersNumber != MaxUsersNumber => false,
                    _ => true
                };
            }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }



        private bool _isFavorite;

        private Color _onlineColor;

        public Color OnlineColor
        {
            get => CurrentUsersNumber switch
            {
                0 => Color.Transparent,
                > 0 when CurrentUsersNumber != MaxUsersNumber => Color.Green,
                _ => Color.Red
            };
            set => SetProperty(ref _onlineColor, value);
        }



        public int OwnerUserId { get; set; }


        public string CountPresentation => $"{_currentUsersNumber}/{_maxUsersNumber}";

        public bool IsRoomFilled => _currentUsersNumber >= _maxUsersNumber;

        public ICollection<string> Categories { get; set; }

        public ICollection<Message> Messages { get; set; }

        [NotMapped]
        public Dictionary<string, Color> UsersIn { get; set; }
    }
}