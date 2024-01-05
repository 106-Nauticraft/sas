using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NFluent.Extensibility;
using NFluent.Kernel;

// ReSharper disable once CheckNamespace
namespace NFluent;

public static class HttpResponseMessageNFluentExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = {new JsonStringEnumConverter()}
    };

    public static ICheckLinkWhich<ICheck<HttpResponseMessage>, ICheck<TContent?>> IsOk<TContent>(
        this ICheck<HttpResponseMessage> check, JsonSerializerOptions? options = null) =>
        check.HasStatus<TContent>(HttpStatusCode.OK, options);

    public static ICheckLink<ICheck<HttpResponseMessage>> IsOk(this ICheck<HttpResponseMessage> check) =>
        check.HasStatus(HttpStatusCode.OK);

    public static ICheckLinkWhich<ICheck<HttpResponseMessage>, ICheck<TContent?>> IsBadRequest<TContent>(
        this ICheck<HttpResponseMessage> check, JsonSerializerOptions? options = null) =>
        check.HasStatus<TContent>(HttpStatusCode.BadRequest, options);

    public static ICheckLink<ICheck<HttpResponseMessage>> IsNotFound(this ICheck<HttpResponseMessage> check) =>
        check.HasStatus(HttpStatusCode.NotFound);
        
    public static ICheckLink<ICheck<HttpResponseMessage>> IsBadRequest(this ICheck<HttpResponseMessage> check) =>
        check.HasStatus(HttpStatusCode.BadRequest);

    public static ICheckLink<ICheck<HttpResponseMessage>> IsUnauthorized(this ICheck<HttpResponseMessage> check) =>
        check.HasStatus(HttpStatusCode.Unauthorized);

    public static ICheckLink<ICheck<HttpResponseMessage>> HasStatus(
        this ICheck<HttpResponseMessage> check,
        HttpStatusCode status)
    {
        var checker = ExtensibilityHelper.ExtractChecker(check);

        var httpResponseMessage = checker.Value;

        return checker.ExecuteCheck(() =>
        {
            if (httpResponseMessage.StatusCode == status) return;


            var contentStream = httpResponseMessage.Content.ReadAsStream();

            var errorPayload = "";

            try
            {
                var payloadAsJson = JsonDocument.Parse(contentStream);
                errorPayload = $": {payloadAsJson.RootElement.ToString()}";
            }
            catch
            {
                // ignored
            }

            throw new FluentCheckException(
                $"The Http StatusCode is invalid. Expected [{status}]. Actual: [{httpResponseMessage.StatusCode}] {errorPayload}");
        }, $"The Http StatusCode is {status} but it shouldn't.");
    }
        
    public static ICheckLinkWhich<ICheck<HttpResponseMessage>, ICheck<TContent?>> HasStatus<TContent>(
        this ICheck<HttpResponseMessage> check,
        HttpStatusCode status,
        JsonSerializerOptions? options = null)
    {
        check.HasStatus(status);

        var checker = ExtensibilityHelper.ExtractChecker(check);
        var response = checker.Value;

        var content = response.Content.ReadFromJsonAsync<TContent>(options ?? DefaultJsonSerializerOptions).GetAwaiter().GetResult();

        return ExtensibilityHelper.BuildCheckLinkWhich(check, content, "HttpResponse Payload");
    }

    public static ICheckLinkWhich<ICheck<HttpResponseMessage>, ICheck<TContent?>> WhichPayload<TContent>(
        this ICheckLinkWhich<ICheck<HttpResponseMessage>, ICheck<TContent?>> checkLink,
        Action<TContent?> checkThatPayload)
    {
        var checkerWhich = ExtensibilityHelper.ExtractChecker(checkLink.Which);
        checkThatPayload(checkerWhich.Value);
        return checkLink;
    }
}