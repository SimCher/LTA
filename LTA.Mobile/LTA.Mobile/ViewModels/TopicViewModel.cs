using System;
using System.Collections.Generic;
using Prism.Mvvm;

namespace LTA.Mobile.ViewModels;

public class TopicViewModel : BindableBase
{
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }
    public DateTime LastEntryDate { get; set; }

    public ICollection<string> Categories { get; set; }

    public int CurrentUsersNumber { get; set; }

    public string CountUsersPresentation => $"{CurrentUsersNumber}/{MaxUsersNumber}";

    public bool IsRoomFilled => CurrentUsersNumber >= MaxUsersNumber;
}