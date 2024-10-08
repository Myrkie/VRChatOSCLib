# VRChatOSC Library for C#
## A simple, straightforward OSC library for VRChat created in .NET Core 6, CSharp

## Usage
##### These examples are for usage demonstrations... do not just copy-paste.
###### Or do. I am a programmer, not a cop.

### Namespace
```cs
using VRChatOSCLib;
```
### Initializing
#### You can use any of these methods to initialize the class.
```cs
VRChatOSC vrcOsc = new VRChatOSC(); // Doesn't connect, use .Connect() after
            .... = new VRChatOSC(true);// Connect and use default remote port 9000
            .... = new VRChatOSC(8901); // Connect and use custom remote port
            .... = new VRChatOSC("192.168.1.X", 9000); // Connect and Use custom IP from string and custom port
            .... = new VRChatOSC(IPAddress ipAddress, 9000); // Connect and Use custom IP from IPAddress class

// If no parameters are passed to the constructor, you will need to connect the client manually. You can do this using the .Connect() method.
vrcOsc.Connect(); // Uses default remote port 9000
vrcOsc.Connect(8901); // Use custom remote port
vrcOsc.Connect("192.168.1.X", 9000); // Use custom IP from string and custom port
vrcOsc.Connect(IPAddress ipAddress, 9000); // Use custom IP from IPAddress class
```
## Examples
```cs
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
        osc.Listen(IPAddress.Parse("127.0.0.1")); // Listen on default port 9001 and local host
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

        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occured");
        }
    }
}
```
  
## Credits

- [OscCore](https://github.com/tilde-love/osc-core)
- There's probably many libraries out there for VRC, also give them a try. This was originally a personal project for personal use, but I decided to share it.