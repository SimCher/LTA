using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using Newtonsoft.Json;

namespace LTA.Mobile.Application.Services;

public class MessageService : IMessageService
{
    private IMessageRepository MessageRepository { get; }
    private IChatService ChatService { get; }

    public MessageService(IChatService chatService, IMessageRepository messageRepository)
    {
        ChatService = chatService;
        MessageRepository = messageRepository;
    }

    public ValueTask<Message> GetMessage(int id)
    {
        return MessageRepository.GetAsync(id);
    }

    public async Task<Message> SendMessage(Message message)
    {
        await ChatService.SendMessage(message, (int)message.TopicId);

        await MessageRepository.AddMessageAsync(message);

        return message;
    }

    public async Task<Message> ReceiveMessage(dynamic message, int topicId)
    {
        var dynamicMessage = JsonConvert.DeserializeObject(message.ToString());

        if (dynamicMessage is null)
        {
            throw new NullReferenceException("deserialize dynamic message was null");
        }

        var replyToId = dynamicMessage.m.replyToId is null
            ? null
            : (int?)dynamicMessage.m.replyToId;

        var replyTo = JsonConvert.DeserializeObject<Message>(dynamicMessage.m.replyTo.ToString()) as Message;

        var newMessage = new Message
        {
            Id = (int) dynamicMessage.m.id,
            Content = (string)dynamicMessage.m.content,
            Image = dynamicMessage.m.image,
            CreationDate = (DateTime) dynamicMessage.m.creationDate,
            UserCode = (string)dynamicMessage.m.userCode,
            IsSent = false,
            TopicId = topicId,
            ReplyTo = replyTo
        };
        
        Debug.WriteLine(newMessage.ToString());

        try
        {
            await MessageRepository.AddMessageAsync(newMessage);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{ex.Source}: {ex.Message}");
        }
        

        return newMessage;

    }

    public IEnumerable<Message> GetAllMessagesForTopic(int topicId)
    {
        return MessageRepository.GetAll().Where(m => m.TopicId == topicId);
    }
}