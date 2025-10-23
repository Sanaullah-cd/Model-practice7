// Concrete User
public class User : IUser
{
    public string Name { get; }
    public bool IsAdmin { get; }
    private readonly IMediator _mediator;

    public User(string name, IMediator mediator, bool isAdmin = false)
    {
        Name = name;
        _mediator = mediator;
        IsAdmin = isAdmin;
    }

    public void SendMessage(string message, string channel)
    {
        Console.WriteLine($"\n[{channel}] {Name} sending: {message}");
        _mediator.SendMessage(message, this, channel);
    }

    public void SendPrivateMessage(string message, IUser receiver)
    {
        Console.WriteLine($"\n{Name} sending private to {receiver.Name}: {message}");
        _mediator.SendPrivateMessage(message, this, receiver);
    }

    public void SendCrossChannelMessage(string message, string fromChannel, string toChannel)
    {
        Console.WriteLine($"\n{Name} cross-sending from {fromChannel} to {toChannel}: {message}");
        _mediator.SendCrossChannelMessage(message, this, fromChannel, toChannel);
    }

    public void ReceiveMessage(string message, IUser sender, string channel)
    {
        Console.WriteLine($"    📩 [{channel}] {Name} received from {sender.Name}: {message}");
    }

    public void ReceivePrivateMessage(string message, IUser sender)
    {
        Console.WriteLine($"    🔐 {Name} received private from {sender.Name}: {message}");
    }

    public void NotifyUserJoined(IUser user, string channel)
    {
        Console.WriteLine($"    👋 {Name} sees: {user.Name} joined {channel}");
    }

    public void NotifyUserLeft(IUser user, string channel)
    {
        Console.WriteLine($"    🚪 {Name} sees: {user.Name} left {channel}");
    }

    public void NotifyBanned(string channel, TimeSpan duration)
    {
        Console.WriteLine($"    🔨 {Name} notified: Banned from {channel} for {duration.TotalMinutes}min");
    }
}
