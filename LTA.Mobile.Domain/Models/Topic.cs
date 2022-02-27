using System;
using System.Collections.Generic;
using LTA.Mobile.Domain.Models.BaseModels;

namespace LTA.Mobile.Domain.Models
{
    public class Topic : BaseModel
    {
        public int[] UserIds { get; set; }

        private string _name;

        private int _ownerUserId;

        private float _rating;

        private int _maxUsersNumber;

        private DateTime _lastEntryDate;

        private int _currentUsersNumber;

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

        public int OwnerUserId { get; set; }


        public string CountPresentation => $"{_currentUsersNumber}/{_maxUsersNumber}";

        public bool IsRoomFilled => _currentUsersNumber >= _maxUsersNumber;

        public ICollection<string> Categories { get; set; }

        public ICollection<Message> Messages { get; set; }


    }
}