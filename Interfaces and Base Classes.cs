using System;
using System.Collections.Generic;
using System.Linq;

// Interface for the Mediator
public interface IMediator
{
    void SendMessage(string message, IUser sender, string channel);
    void AddUser(IUser user, string channel);
    void RemoveUser(IUser user, string channel);
    void CreateChannel(string channelName);
    void SendPrivateMessage(string message, IUser sender, IUser receiver);
    void SendCrossChannelMessage(string message, IUser sender, string fromChannel, string toChannel);
    void BanUser(IUser admin, IUser userToBan, string channel, TimeSpan duration);
    bool ChannelExists(string channel);
    List<string> GetChannels();
}

// Interface for Users
public interface IUser
{
    string Name { get; }
    bool IsAdmin { get; }
    void SendMessage(string message, string channel);
    void SendPrivateMessage(string message, IUser receiver);
    void SendCrossChannelMessage(string message, string fromChannel, string toChannel);
    void ReceiveMessage(string message, IUser sender, string channel);
    void ReceivePrivateMessage(string message, IUser sender);
    void NotifyUserJoined(IUser user, string channel);
    void NotifyUserLeft(IUser user, string channel);
    void NotifyBanned(string channel, TimeSpan duration);
}
