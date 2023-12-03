# ConfigBlocks

## Description

ConfigBlocks is a Unity package that helps you make your code configurable by augmenting your key-value-store Remote Config provider (such as Firebase Remote Config or similar).

You define C# classes that represent configurations for various parts of your app. ConfigBlocks will use the name of the type as the key to look up a json string value from your Remote Config provider.

You'll always get a valid instance of the type you requested. If no json string value is found, ConfigBlocks will default-construct the type. This makes it easy to add new configurations to your app without having to add control flow behavior depending if the Remote Config provider has json for a given key or not.

## Usage

### 1. Connect your Remote Config solution

Example using Firebase Remote Config:
```csharp
class FirebaseRemoteConfig : ConfigBlocks.IRemoteConfig
{
    public bool TryGetValue(string key, out string value)
    {
        // You would add code here to check if the key exists
        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).StringValue;
    }
}

var configBlockProvider = new ConfigBlockProvider(new FirebaseRemoteConfig());
```

### 2. Create a ConfigBlock

```csharp
class StartPageConfigBlock
{
    public string MessageOfTheDay;
    public string BackgroundImageUrl;
}
```

### 3. Get the ConfigBlock

```csharp
var configBlock = configBlockProvider.Get<StartPageConfigBlock>();
ShowMessage(configBlock.MessageOfTheDay);
```

What actually happens there 👆 is that ConfigBlocks will look up the json string value for the key `"StartPageConfigBlock"` from your Remote Config provider. If it exists, it will deserialize the json value into a `StartPageConfigBlock` instance using Unity's built-in JsonUtility. If it doesn't exist, it will default-construct a `StartPageConfigBlock` instance.