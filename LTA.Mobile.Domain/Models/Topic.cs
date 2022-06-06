using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class Topic : BaseModel
    {
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
            UsersIn = new ObservableCollection<User>();
            Messages = new HashSet<Message>();
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
            get => UsersIn.Count;
            set => SetProperty(ref _currentUsersNumber, value);
        }

        public string CategoriesArray
        {
            get => _categoriesArray;
            set => SetProperty(ref _categoriesArray, value);
        }

        private string _categoriesArray;

        [NotMapped]
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

        [NotMapped]
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }
        
        private bool _isFavorite;

        private Color _onlineColor;

        [NotMapped]
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

        [NotMapped]
        public string CountPresentation => $"{_currentUsersNumber}/{_maxUsersNumber}";

        [NotMapped]
        public bool IsRoomFilled => _currentUsersNumber >= _maxUsersNumber;

        [NotMapped]
        public ICollection<string> Categories
            => !string.IsNullOrEmpty(CategoriesArray) ? CategoriesArray.Split(',') : null;

        public ICollection<Message>? Messages { get; set; }

        [NotMapped]
        public ICollection<User> UsersIn { get; set; }

        public override string ToString()
        {
            return $"---------------------\n" +
                   $"ID: {Id.ToString()}\n" +
                   $"Name: {Name}\n" +
                   $"Max users: {MaxUsersNumber.ToString()}\n" +
                   $"Last entry: {LastEntryDate.ToString(CultureInfo.CurrentCulture)}\n" +
                   $"Current users: {CurrentUsersNumber.ToString()}\n" +
                   $"Categories: {CategoriesArray}\n" +
                   $"Owner user id: {OwnerUserId.ToString()}\n" +
                   $"Hash code: {GetHashCode().ToString()}\n" +
                   $"--------------------------------------\n";
        }
    }
}