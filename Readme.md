# MeiHasCrashed.DSharpPlus.Components

This library provides a simple component system for DSharpPlus, 
inspired by Discord.Net's Interaction implementation.

> [!CAUTION]
> Currently this is primarily intended for me to use, but if anyone else finds it useful, feel free to use it.\
> However I make no guarantees about stability or support. Things might get removed or changed without warning.


## Features
- Declare handlers for Buttons, Modals and most components using attributes similar to how you can declare commands.
- Automatic registration of component handlers.
- Handlers can take parameters from the component's custom ID using wildcards.

## Installation
You can install the library via NuGet:
```
Install-Package MeiHasCrashed.DSharpPlus.Components
```
Or via the .NET CLI:
```
dotnet package add MeiHasCrashed.DSharpPlus.Components
```

> [!IMPORTANT]
> You will need at least DSharpPlus version 5.0.0 or higher to use this library (currently DSharpPlus Nightly).

## Usage

> [!TIP]
> You can find a full example project in `src/DSharpPlus.Components.Example`

You first need to register the component system in your service collection and configure it:

```csharp
services.AddComponentsExtension(components =>
    {
        components.AddInteractions(Assembly.GetAssembly(typeof(Program))!);
    })
```
You can either use `AddInteractions` to scan an assembly for all component handlers (both ComponentInteraction and ModalInteraction),
or you can use `AddComponents` or `AddModals` to only register one type.

Then, you can declare your component handlers using attributes:

> [!NOTE]
> Component Handlers have no specific requirements for their containing class.\
> Additionally, they can either be in normal classes or static classes.\
> Normal classes will get resolved from the service provider the extension is registered on.

### Message Components
```csharp
public class MyButtonHandler {
    [ComponentInteraction("button-test-*")]
    public async Task HandleTestButtonAsync(ComponentContext ctx, string id){
        var builder = new DiscordInteractionResponseBuilder()
            .WithContent("Button clicked! Wildcard: " + id)
            .AsEphemeral();
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
}
```

### Modals
```csharp
public class MyModalHandler {
    [ModalInteraction("modal-test-*")]
    public async Task ModalTestHandler(ModalContext ctx, string wildcard)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Thank you!").AsEphemeral());
    }
}
```

## CustomIds, Path Matching and Wildcards
Currently, the customId you specify will be used as paths using `-` as a separator, in a similar fashion to how http routing works.
Wildcards (`*`) will be sent to the function as string arguments.

> [!CAUTION]
> You **must** have a matching number of wildcards and string parameters in your handler method.

If you have multiple wildcards, they will be passed in the order they appear in the customId.

### Routing
The routing system will always attempt to match to the most specific route first. 
In case this fails, it will try to match with wildcards.\
In the case that a specific match fails, but a wildcard match is found earlier in the customId, 
the rest of the id will be treated as a single wildcard and returned in joined format.

### Examples
**Basic Examples**:
- `button-test-123` matches `button-test-*` with wildcard `123`
- `button-test-123-456` matches `button-test-*-*` with wildcards `123` and `456`
- `button-test-123-456` matches `button-test-*` with wildcard `123-456`
- `button-foo-123` does not match `button-test-*`

**Priority Examples**:
- `button-test-123-456` with handlers for `button-test-*-*` and `button-test-123-456` will match `button-test-123-456` first.
- `button-test-123-456` with handlers for `button-test-123-*` and `button-test-*` will match `button-test-123-*` first.

## License
This project is licensed under the terms of the MIT License.
See the [LICENSE](LICENSE.md) file for details.