using System;
using System.Collections.Generic;

namespace LTA.Mobile.ViewModels;

public class TopicViewModel
{
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }
    public DateTime LastEntryDate { get; set; }

    public ICollection<string> Categories { get; set; }
}