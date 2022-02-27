using System.Windows.Input;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.EventHandlers;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public partial class ChatRoomPageModel
{
    public ICommand MessageSwippedCommand { get; private set; }
    public ICommand CancelReplyCommand { get; private set; }
    public ICommand ReplyMessageSelectedCommand { get; private set; }
    private bool _isTyping;

    public bool IsTyping
    {
        get => _isTyping;
        set => _isTyping = value;
    }

    private Message _replyMessage;

    public Message ReplyMessage
    {
        get => _replyMessage;
        set => this.RaiseAndSetIfChanged(ref _replyMessage, value);
    }

    private void InitializeReactive()
    {
        ReplyMessageSelectedCommand = ReactiveCommand.Create<Message>(ReplyMessageSelected);
        MessageSwippedCommand = ReactiveCommand.Create<Message>(MessageSwiped);
        SendMsgCommand = ReactiveCommand.CreateFromTask(SendMsg,
            this.WhenAnyValue(vm => vm.Message, curm => !string.IsNullOrEmpty(curm)));
        CancelReplyCommand = ReactiveCommand.Create(CancelReply);
    }

    private void CancelReply()
    {
        ReplyMessage = null;

        MessagingCenter.Send(this, "ShowKeyboard", new LTAFocusEventArgs { IsFocused = false });
    }

    private void ReplyMessageSelected(Message message)
        => ScrollToMessage(message);

    private void MessageSwiped(Message message)
    {
        ReplyMessage = message;
        MessagingCenter.Send(this, "ShowKeyboard", new LTAFocusEventArgs { IsFocused = true });
    }

    private void ScrollToMessage(Message message)
    {
        MessagingCenter.Send(this, "ScrollToItem",
            new ScrollToItemEventArgs { Item = message });
    }
}