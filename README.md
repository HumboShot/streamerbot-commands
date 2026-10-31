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
2. Copy `secrets.example.json` to `secrets.json` and fill in your values. `secrets.json` is git-ignored. Keep it private.
3. Build `Commands.csproj`. The build writes a copy of each command to `dist/`, with the `{{KEY}}` placeholders replaced by the values from `secrets.json`. The build fails if a placeholder has no value.
4. Paste each file from `dist/` into Streamer.bot as a C# execute sub-action.
5. `WeatherCommand` also reports the Stue temperature from Home Assistant. In Streamer.bot's **Variables** tab, add a persisted Global variable named `haToken` set to your Home Assistant long-lived access token (Home Assistant → your profile → Security → Long-Lived Access Tokens). Without this token, the Stue temperature is silently skipped and the rest of the weather report still works.
   - Optional: override `haStueEntityId` (default `sensor.temperature_and_humidity_sensor_t1_temperature`) as an action argument if your setup differs.
6. `WeatherCommand` and `SetLightCommand` both call Home Assistant at `http://192.168.1.234:7777`. To change this address in one place, add a persisted Global variable named `haBaseUrl`. A `haBaseUrl` action argument on the weather action overrides it.
7. `NowPlayingCommand` reads the song from your private nowplaying.site widget URL. Set it as `NOW_PLAYING_URL` in `secrets.json`.
8. `SetLightCommand` refunds the channel point redeem when the color is not valid, or when Home Assistant does not respond.

## License

[MIT](LICENSE)
