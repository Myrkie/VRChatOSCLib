using System.Net;
using VRChatOSCLib;

internal class Program
{
    static VRChatOSC osc = new VRChatOSC();

    static async Task Main(string[] args)
    {
        // Connect. Port number is optional
        osc.Connect(9000);

        // Asyncn't
        // Send to a specific address
        osc.SendTo("/test/lib/float", 0.5f);


        // Send to an avatar parameter
        // This will automatically send it to the proper path
        osc.SendParameter("GlassesToggle", true); // Boolean sent as "/avatar/parameters/GlassesToggle"
        osc.SendParameter("GlassesColor", 0.5f); // Float sent as "/avatar/parameters/GlassesColor"
        osc.SendParameter("UltimateAnswer", 42); // Integer sent as "/avatar/parameters/UltimateAnswer"


        // Send supported inputs like buttons and Axes to VRChat
        osc.SendInput(VRCButton.Jump, true); // Jump sent as "/input/Jump/" 1
        osc.SendInput(VRCButton.Jump, false); // Jump sent as "/input/Jump/" 0

        osc.SendInput(VRCAxes.Vertical, 0.42f); // Vertical Axes sent as "/input/Vertical/" 0.42f


        // Send Chatbox Messages
        osc.SendChatbox("Hello World 1"); // Sends the string to the Keyboard
        osc.SendChatbox("Hello World 2", true); // Bypass keyboard and sends string to Chatbox
        osc.SendChatbox("Hello World 3", true, true); // Sends a string to the Chatbox and plays message SFX on VRC

        osc.SendChatbox(true); // Set typing indicator ON
        osc.SendChatbox(false); // Set typing indicator OFF


        // Async aka Awaitable methods
        await osc.SendToAsync("/test/lib/async", true);
        await osc.SendParameterAsync("AsyncRocks", true);
        await osc.SendInputAsync(VRCButton.Run, true);
        await osc.SendChatboxAsync("Hello World", true);


        // Listen for incoming messages
        osc.Listen(); // Listen on default port 9001 and local host
        osc.Listen(null, 9042); // Listen on custom port
        osc.Listen(null, 9001, 1024); // Use custom port and custom buffer length
        
        
        osc.Listen(IPAddress.Parse("127.0.0.1")); // Listen on default port 9001 and on user defined ipaddress
        osc.Listen(IPAddress.Parse("127.0.0.1"), 9042); // Listen on port a custom port and on user defined ipaddress

        // Subscribe to incoming messages
        osc.OnMessage += OnMessageReceived;
        Console.WriteLine($"OSC Initialized:\n                         Remote: {osc.RemoteEndPoint}\n                         Local: {osc.LocalEndPoint}");

        
        // bulk add methods on parmeter recieved
        osc.TryAddMethod("helloworld", HelloWorldParameter);
        
        Console.ReadKey(true); // :P
    }

    static void OnMessageReceived(object? source, VRCMessage message)
    {
        // Full address from message, example "/avatar/parameters/CatToggle"
        string address = message.Address;
        // Shortened path from the address, example "/avatar/parameters/"
        string path = message.Path;

        // True if this message is an avatar parameter of any kind
        bool isParameter = message.IsParameter;

        // Message type (Unknown|DefaultParameter|AvatarParameter|AvatarChange)
        VRCMessage.MessageType messageType = message.Type;
        
        switch (messageType)
        {
            // shortened address to just be the parameter, example "MuteSelf" 
            case VRCMessage.MessageType.AvatarParameter:
                switch (message.AvatarParameter)
                {
                    case "myboolean":
                        if (message.GetValue() is bool boolean)
                        {
                            if (boolean)
                            {
                                Console.WriteLine($"boolean state is {boolean}.");
                                
                                // invert state after 5 seconds
                                Thread.Sleep(5000);
                                osc.SendInput(VRCButton.Voice, !boolean);
                                Console.WriteLine($"boolean state is now inverted.");
                            }
                        }
                        break;
                    
                    default: 
                        message.Print(); 
                        break;
                }
                break;
            
            case VRCMessage.MessageType.AvatarChange:
                if (message.GetValue() is string s) Console.WriteLine($"Avatar Changed to {s}");
                break;
        }

        // Retrieve the first value object contained in this message
        object? value = message.GetValue(); // Can return default
        float val = message.GetValue<float>(); // Alternative with generic type

        val = message.GetValueAt<float>(0); // Get argument value by index
        value = message[0]; // Same with indexer

        // Print this message with fancy colors for debugging
        message.Print();
    }
    
    static void HelloWorldParameter(VRCMessage msg)
    {
        try
        {
            if (msg.GetValue() is bool boolean)
            {
                if (boolean)
                {
                    Console.WriteLine($"helloworld boolean state is {boolean}.");
                                
                    // invert state after 5 seconds
                    Thread.Sleep(5000);
                    osc.SendInput(VRCButton.Voice, !boolean);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occured");
        }
    }
}