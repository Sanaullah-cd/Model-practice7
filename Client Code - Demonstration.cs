class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🚀 CHAT SYSTEM DEMONSTRATION - MEDIATOR PATTERN\n");

        // Create mediator
        IMediator mediator = new ChatMediator();

        // Create users
        var alice = new User("Alice", mediator);
        var bob = new User("Bob", mediator);
        var charlie = new User("Charlie", mediator);
        var admin = new User("Admin", mediator, true);

        Console.WriteLine("=== SETUP: Creating channels and adding users ===\n");

        // Create channels
        mediator.CreateChannel("general");
        mediator.CreateChannel("games");
        mediator.CreateChannel("work");

        // Add users to channels
        mediator.AddUser(alice, "general");
        mediator.AddUser(bob, "general");
        mediator.AddUser(charlie, "general");
        mediator.AddUser(admin, "general");

        mediator.AddUser(alice, "games");
        mediator.AddUser(bob, "games");

        mediator.AddUser(alice, "work");
        mediator.AddUser(admin, "work");

        Console.WriteLine("\n=== TEST 1: Normal messaging ===\n");
        alice.SendMessage("Hello everyone!", "general");
        bob.SendMessage("Hi Alice! How are you?", "general");
        charlie.SendMessage("I'm here too!", "general");

        Console.WriteLine("\n=== TEST 2: Private messages ===\n");
        alice.SendPrivateMessage("This is a secret message for you!", bob);
        bob.SendPrivateMessage("Got it, thanks!", alice);

        Console.WriteLine("\n=== TEST 3: Cross-channel messaging ===\n");
        alice.SendCrossChannelMessage("Hello work people from general chat!", "general", "work");

        Console.WriteLine("\n=== TEST 4: User join/leave notifications ===\n");
        var david = new User("David", mediator);
        mediator.AddUser(david, "general");
        mediator.RemoveUser(charlie, "general");

        Console.WriteLine("\n=== TEST 5: Admin functionality - Ban user ===\n");
        admin.SendMessage("I'm the admin here", "general");
        admin.BanUser(admin, bob, "general", TimeSpan.FromSeconds(10)); // 10 sec ban for demo

        Console.WriteLine("\n=== TEST 6: Error handling - Banned user tries to message ===\n");
        bob.SendMessage("This should fail - I'm banned!", "general");

        Console.WriteLine("\n=== TEST 7: Error handling - Non-existent channel ===\n");
        alice.SendMessage("Test message", "nonexistent-channel");

        Console.WriteLine("\n=== TEST 8: Error handling - User not in channel ===\n");
        charlie.SendMessage("I'm not in this channel", "work");

        Console.WriteLine("\n=== Available Channels ===");
        foreach (var channel in mediator.GetChannels())
        {
            Console.WriteLine($"📁 {channel}");
        }

        Console.WriteLine("\n⏳ Waiting for ban to expire...");
        System.Threading.Thread.Sleep(11000); // Wait for ban to expire

        Console.WriteLine("\n=== TEST 9: After ban expires ===\n");
        bob.SendMessage("I'm back! The ban expired.", "general");

        Console.WriteLine("\n🎉 Demonstration completed!");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
