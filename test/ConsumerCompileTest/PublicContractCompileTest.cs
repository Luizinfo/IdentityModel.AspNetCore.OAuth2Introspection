using System;
using System.Threading.Tasks;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;

namespace ConsumerCompileTest;

public static class PublicContractCompileTest
{
    public static void Configure(
        OAuth2IntrospectionOptions options,
        DiscoveryPolicy discoveryPolicy,
        ClientCredentialStyle clientCredentialStyle,
        BasicAuthenticationHeaderStyle authorizationHeaderStyle)
    {
        options.DiscoveryPolicy = discoveryPolicy;
        options.ClientCredentialStyle = clientCredentialStyle;
        options.AuthorizationHeaderStyle = authorizationHeaderStyle;

        options.Events.OnSendingRequest = context =>
        {
            TokenIntrospectionRequest request = context.TokenIntrospectionRequest;
            request.TokenTypeHint = options.TokenTypeHint;
            return Task.CompletedTask;
        };

        options.Events.OnUpdateClientAssertion = context =>
        {
            ClientAssertion assertion = context.ClientAssertion;
            context.ClientAssertion = assertion;
            context.ClientAssertionExpirationTime = DateTime.UtcNow.AddMinutes(1);
            return Task.CompletedTask;
        };
    }
}
