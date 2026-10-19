using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

/*----- Class name should match FileName -----*/
#if EXTERNAL_EDITOR
public class SetLightCommand : CPHInlineBase
#else
public class CPHInline
#endif
/*--------------------------------------------*/
{
    private static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };

    public bool Execute()
    {
        var rawInput = args.ContainsKey("rawInput") ? args["rawInput"].ToString().Trim().ToLower() : "";

        var finalColor = ValidateColor(rawInput);

        string userName = "";
        CPH.TryGetArg("userName", out userName);

        if (string.IsNullOrEmpty(finalColor))
        {
            bool refundSuccess = Refund();
            CPH.LogWarn($"[Hue Lights] Invalid color input from viewer: '{rawInput}'. Refund attempted: {refundSuccess}.");
            CPH.SendMessage(
                $"@{userName} '{rawInput}' is not a valid color. You can find the list here: https://link.humboshot.com/colors"
            );

            return true;
        }

        var payload = new { color_name = finalColor };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        CPH.LogInfo($"[Hue Lights] Sending color change request to Home Assistant: {json}");

        // The webhook only accepts calls from the local network.
        string haBaseUrl = CPH.GetGlobalVar<string>("haBaseUrl", true) ?? "http://192.168.1.234:7777";
        var webhookUrl = haBaseUrl.TrimEnd('/') + "/api/webhook/twitch_hue_color";

        string error = null;
        try
        {
            HttpResponseMessage response = client.PostAsync(webhookUrl, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                error = $"status {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }

        if (error != null)
        {
            bool refundSuccess = Refund();
            CPH.LogWarn($"[Hue Lights] HA webhook failed: {error}. Refund attempted: {refundSuccess}.");
            CPH.SendMessage(refundSuccess
                ? $"@{userName} the lights did not respond, so your points were refunded."
                : $"@{userName} the lights did not respond. Sorry about that!");
        }

        return true;
    }

    private bool Refund()
    {
        if (CPH.TryGetArg("rewardId", out string rewardId) && CPH.TryGetArg("redemptionId", out string redemptionId))
        {
            return CPH.TwitchRedemptionCancel(rewardId, redemptionId);
        }

        return false;
    }

    private string? ValidateColor(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        // The list of colors have been taken from the HA code https://github.com/home-assistant/core/blob/e2fdc6a98bdd22187688e70701fc3617423a714b/homeassistant/util/color.py#L19
        var allowedColors = new List<string>
        {
            "aliceblue",
            "antiquewhite",
            "aqua",
            "aquamarine",
            "azure",
            "beige",
            "bisque",
            "black",
            "blanchedalmond",
            "blue",
            "blueviolet",
            "brown",
            "burlywood",
            "cadetblue",
            "chartreuse",
            "chocolate",
            "coral",
            "cornflowerblue",
            "cornsilk",
            "crimson",
            "cyan",
            "darkblue",
            "darkcyan",
            "darkgoldenrod",
            "darkgray",
            "darkgreen",
            "darkgrey",
            "darkkhaki",
            "darkmagenta",
            "darkolivegreen",
            "darkorange",
            "darkorchid",
            "darkred",
            "darksalmon",
            "darkseagreen",
            "darkslateblue",
            "darkslategray",
            "darkslategrey",
            "darkturquoise",
            "darkviolet",
            "deeppink",
            "deepskyblue",
            "dimgray",
            "dimgrey",
            "dodgerblue",
            "firebrick",
            "floralwhite",
            "forestgreen",
            "fuchsia",
            "gainsboro",
            "ghostwhite",
            "gold",
            "goldenrod",
            "gray",
            "green",
            "greenyellow",
            "grey",
            "honeydew",
            "hotpink",
            "indianred",
            "indigo",
            "ivory",
            "khaki",
            "lavender",
            "lavenderblush",
            "lawngreen",
            "lemonchiffon",
            "lightblue",
            "lightcoral",
            "lightcyan",
            "lightgoldenrodyellow",
            "lightgray",
            "lightgreen",
            "lightgrey",
            "lightpink",
            "lightsalmon",
            "lightseagreen",
            "lightskyblue",
            "lightslategray",
            "lightslategrey",
            "lightsteelblue",
            "lightyellow",
            "lime",
            "limegreen",
            "linen",
            "magenta",
            "maroon",
            "mediumaquamarine",
            "mediumblue",
            "mediumorchid",
            "mediumpurple",
            "mediumseagreen",
            "mediumslateblue",
            "mediumspringgreen",
            "mediumturquoise",
            "mediumvioletred",
            "midnightblue",
            "mintcream",
            "mistyrose",
            "moccasin",
            "navajowhite",
            "navy",
            "navyblue",
            "oldlace",
            "olive",
            "olivedrab",
            "orange",
            "orangered",
            "orchid",
            "palegoldenrod",
            "palegreen",
            "paleturquoise",
            "palevioletred",
            "papayawhip",
            "peachpuff",
            "peru",
            "pink",
            "plum",
            "powderblue",
            "purple",
            "red",
            "rosybrown",
            "royalblue",
            "saddlebrown",
            "salmon",
            "sandybrown",
            "seagreen",
            "seashell",
            "sienna",
            "silver",
            "skyblue",
            "slateblue",
            "slategray",
            "slategrey",
            "snow",
            "springgreen",
            "steelblue",
            "tan",
            "teal",
            "thistle",
            "tomato",
            "turquoise",
            "violet",
            "wheat",
            "white",
            "whitesmoke",
            "yellow",
            "yellowgreen",
            // And...
            "homeassistant"
        };

        if (allowedColors.Contains(input))
        {
            return input;
        }

        return null;
    }
}