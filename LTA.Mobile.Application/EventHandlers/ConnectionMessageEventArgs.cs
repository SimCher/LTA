using System;

namespace LTA.Mobile.Application.EventHandlers;

public class ConnectionMessageEventArgs : EventArgs
{
    public string Title { get; set; }
    public string Message { get; set; }
}