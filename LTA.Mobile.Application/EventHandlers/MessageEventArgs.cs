using LTA.Mobile.Application.EventHandlers.Interfaces;

namespace LTA.Mobile.Application.EventHandlers;

public class MessageEventArgs : IMessageEventArgs
{
    public MessageEventArgs(string message, string topicId)
    {
        Message = message;
        Topic = topicId;
    }
    public string Message { get; }
    public string Topic { get; }
}