// Concrete Mediator
public class ChatMediator : IMediator
{
    private readonly Dictionary<string, List<IUser>> _channels = new Dictionary<string, List<IUser>>();
    private readonly Dictionary<string, DateTime> _bannedUsers = new Dictionary<string, DateTime>();

    public void CreateChannel(string channelName)
    {
        if (!_channels.ContainsKey(channelName))
        {
            _channels[channelName] = new List<IUser>();
            Console.WriteLine($"✅ Channel '{channelName}' created");
        }
    }

    public void AddUser(IUser user, string channel)
    {
        if (!ChannelExists(channel))
        {
            CreateChannel(channel);
        }

        if (!_channels[channel].Contains(user))
        {
            _channels[channel].Add(user);
            
            // Notify all users in the channel
            foreach (var u in _channels[channel].Where(u => u != user))
            {
                u.NotifyUserJoined(user, channel);
            }
            
            Console.WriteLine($"👤 {user.Name} joined channel '{channel}'");
        }
    }

    public void RemoveUser(IUser user, string channel)
    {
        if (_channels.ContainsKey(channel) && _channels[channel].Contains(user))
        {
            _channels[channel].Remove(user);
            
            foreach (var u in _channels[channel])
            {
                u.NotifyUserLeft(user, channel);
            }
            
            Console.WriteLine($"👤 {user.Name} left channel '{channel}'");
        }
    }

    public void SendMessage(string message, IUser sender, string channel)
    {
        if (!ChannelExists(channel))
        {
            Console.WriteLine($"❌ Error: Channel '{channel}' doesn't exist");
            return;
        }

        if (!_channels[channel].Contains(sender))
        {
            Console.WriteLine($"❌ Error: {sender.Name} is not in channel '{channel}'");
            return;
        }

        if (IsUserBanned(sender.Name))
        {
            Console.WriteLine($"❌ Error: {sender.Name} is banned");
            return;
        }

        foreach (var user in _channels[channel].Where(u => u != sender))
        {
            user.ReceiveMessage(message, sender, channel);
        }
        
        Console.WriteLine($"💬 [{channel}] {sender.Name}: {message}");
    }

    public void SendPrivateMessage(string message, IUser sender, IUser receiver)
    {
        if (IsUserBanned(sender.Name))
        {
            Console.WriteLine($"❌ Error: {sender.Name} is banned");
            return;
        }

        receiver.ReceivePrivateMessage(message, sender);
        Console.WriteLine($"🔒 [PRIVATE] {sender.Name} → {receiver.Name}: {message}");
    }

    public void SendCrossChannelMessage(string message, IUser sender, string fromChannel, string toChannel)
    {
        if (!ChannelExists(fromChannel) || !_channels[fromChannel].Contains(sender))
        {
            Console.WriteLine($"❌ Error: User not in channel '{fromChannel}'");
            return;
        }

        if (!ChannelExists(toChannel))
        {
            CreateChannel(toChannel);
        }

        foreach (var user in _channels[toChannel].Where(u => u != sender))
        {
            user.ReceiveMessage($"[From {fromChannel}] {message}", sender, toChannel);
        }
        
        Console.WriteLine($"🔄 [CROSS] {fromChannel} → {toChannel}: {sender.Name}: {message}");
    }

    public void BanUser(IUser admin, IUser userToBan, string channel, TimeSpan duration)
    {
        if (!admin.IsAdmin)
        {
            Console.WriteLine($"❌ Error: {admin.Name} is not an administrator");
            return;
        }

        if (!ChannelExists(channel))
        {
            Console.WriteLine($"❌ Error: Channel '{channel}' doesn't exist");
            return;
        }

        var banUntil = DateTime.Now.Add(duration);
        _bannedUsers[userToBan.Name] = banUntil;
        RemoveUser(userToBan, channel);
        userToBan.NotifyBanned(channel, duration);
        
        Console.WriteLine($"🔨 {userToBan.Name} banned from '{channel}' for {duration.TotalMinutes} minutes");
        
        // Auto-unban timer
        System.Threading.Timer timer = null;
        timer = new System.Threading.Timer(_ => 
        {
            _bannedUsers.Remove(userToBan.Name);
            Console.WriteLine($"✅ {userToBan.Name} has been unbanned");
            timer?.Dispose();
        }, null, duration, Timeout.InfiniteTimeSpan);
    }

    public bool ChannelExists(string channel)
    {
        return _channels.ContainsKey(channel);
    }

    public List<string> GetChannels()
    {
        return _channels.Keys.ToList();
    }

    private bool IsUserBanned(string userName)
    {
        if (_bannedUsers.TryGetValue(userName, out var banUntil))
        {
            if (DateTime.Now < banUntil)
                return true;
            else
                _bannedUsers.Remove(userName);
        }
        return false;
    }
}
