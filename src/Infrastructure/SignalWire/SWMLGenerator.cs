using Application.Interfaces;
using Domain.ValueObjects;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace Infrastructure.SignalWire;

/// <summary>
/// Generates SWML (SignalWire Markup Language) using Twilio's TwiML library
/// SignalWire is Twilio-compatible, so we use the Twilio SDK
/// </summary>
public class SWMLGenerator : ISWMLGenerator
{
    public string GenerateMenuResponse(MenuState menuState, string actionUrl)
    {
        if (menuState == null)
            throw new ArgumentNullException(nameof(menuState));

        if (string.IsNullOrWhiteSpace(actionUrl))
            throw new ArgumentException("Action URL cannot be empty", nameof(actionUrl));

        var response = new VoiceResponse();

        // Use Gather to collect DTMF input
        var gather = new Gather(
            input: new List<Gather.InputEnum> { Gather.InputEnum.Dtmf },
            action: new Uri(actionUrl, UriKind.Relative),
            method: Twilio.Http.HttpMethod.Post,
            numDigits: 1,
            timeout: 5
        );

        // Add the menu message as text-to-speech
        gather.Say(menuState.Message, voice: Say.VoiceEnum.PollyJoanna, language: Say.LanguageEnum.EnUs);

        response.Append(gather);

        // If no input is received, repeat the menu
        response.Say("We didn't receive any input. Please try again.",
            voice: Say.VoiceEnum.PollyJoanna,
            language: Say.LanguageEnum.EnUs);
        response.Redirect(new Uri(actionUrl, UriKind.Relative));

        return response.ToString();
    }

    public string GenerateErrorResponse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = "An error occurred. Please try again later.";

        var response = new VoiceResponse();
        response.Say(message, voice: Say.VoiceEnum.PollyJoanna, language: Say.LanguageEnum.EnUs);
        response.Hangup();

        return response.ToString();
    }

    public string GenerateHangupResponse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = "Thank you for calling. Goodbye.";

        var response = new VoiceResponse();
        response.Say(message, voice: Say.VoiceEnum.PollyJoanna, language: Say.LanguageEnum.EnUs);
        response.Hangup();

        return response.ToString();
    }
}
