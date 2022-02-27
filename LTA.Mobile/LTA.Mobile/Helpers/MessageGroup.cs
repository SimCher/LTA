using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Helpers;

public class MessageGroup : ObservableCollection<Message>
{
    public string GroupHeader { get; set; }
    public DateTime DateTime { get; set; }

    public MessageGroup(DateTime dateTime, string groupHeader, IEnumerable<Message> messages) : base(messages)
    {
        DateTime = dateTime;
        GroupHeader = groupHeader;
    }
}