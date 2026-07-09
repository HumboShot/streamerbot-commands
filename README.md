# Streamer.Bot Commands

Custom C# sub-actions for [Streamer.bot](https://streamer.bot), plus supporting data files.

This repo was split out of the original `twitch-webpages` monorepo, keeping full file history.

## Structure

- `Commands/` — .NET (net481) WPF class library with Streamer.bot C# sub-actions:
  - `NowPlayingCommand.cs`
  - `SetLightCommand.cs`
  - `WeatherCommand.cs`
- `Text Files/` — text content used by commands (e.g. `Ads started.txt` for random ad-break messages)

## Setup

1. Set `StreamerBotPath` in `Commands/Commands.csproj` to your local Streamer.bot install directory.
2. Build `Commands.csproj`.
3. Import the compiled sub-actions into Streamer.bot as a C# execute sub-action.
