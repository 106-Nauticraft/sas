using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using NFluent;
using NFluent.Kernel;

namespace sas.nfluent.tests;

public class HttpResponseMessageCheckShould
{
    [Fact]
    public void Validate_Ok_HttpStatus_Code_when_response_is_200()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        Check.ThatCode(() => Check.That(response).IsOk()).DoesNotThrow();
    }
        
    // ReSharper disable once UnusedMember.Local
    private enum ContentType { Type1, Type2 }

    private record Content(int Id, string Name, decimal[] Values, ContentType Type)
    {
        public static Content Create() => new(12, "Name", new[] {1.0m, 0.9m, 2.0m}, ContentType.Type2);
    }
        
    [Fact]
    public void Validate_Ok_Response_With_a_payload_serialized_With_SystemTextJson()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Content.Create())
        };

        Check.That(response).IsOk<Content>()
            .WhichPayload(content =>
            {
                Check.That(content).IsNotNull();
                Check.That(content!.Id).IsEqualTo(12);
                Check.That(content.Name).IsEqualTo("Name");
                Check.That(content.Values).ContainsExactly(1.0m, 0.9m, 2.0m);
                Check.That(content.Type).IsEqualTo(ContentType.Type2);
            });
    }

    [Fact]
    public void Validate_Ok_Response_With_a_payload_serialized_with_NewtonSoft()
    {
        var serialized = JsonConvert.SerializeObject(Content.Create());
            
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(serialized, Encoding.UTF8, "application/json")
        };

        Check.ThatCode(() => 
            Check.That(response).IsOk<Content>()
                .WhichPayload(content =>
                {
                    Check.That(content).IsNotNull();
                    Check.That(content!.Id).IsEqualTo(12);
                    Check.That(content.Name).IsEqualTo("Name");
                    Check.That(content.Values).ContainsExactly(1.0m, 0.9m, 2.0m);
                    Check.That(content.Type).IsEqualTo(ContentType.Type2);
                })
        ).DoesNotThrow();
    }
        
    [Fact]
    public void Validate_BadRequest_HttpStatus_Code_when_response_is_400()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        Check.ThatCode(() => Check.That(response).IsBadRequest()).DoesNotThrow();
    }
        
    [Fact]
    public void Validate_NotFound_HttpStatus_Code_when_response_is_404()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        Check.ThatCode(() => Check.That(response).IsNotFound()).DoesNotThrow();
    }
        
    [Fact]
    public void Validate_UnAuthorized_HttpStatus_Code_when_response_is_401()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        Check.ThatCode(() => Check.That(response).IsUnauthorized()).DoesNotThrow();
    }
        
        
    [Fact]
    public void Fail_when_statusCode_does_not_match()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        Check.ThatCode(() => Check.That(response).HasStatus(HttpStatusCode.Conflict))
            .Throws<FluentCheckException>()
            .WithMessage("The Http StatusCode is invalid. Expected [Conflict]. Actual: [Unauthorized] ");
    }
        
    [Fact]
    public void Fail_with_message_including_error_payload_when_statusCode_does_not_match()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = JsonContent.Create(new
            {
                type = "ERROR",
                message = "Something bad happened"
            })
        };

        Check.ThatCode(() => Check.That(response).HasStatus(HttpStatusCode.Conflict))
            .Throws<FluentCheckException>()
            .WithMessage("The Http StatusCode is invalid. Expected [Conflict]. Actual: [Unauthorized] : {\"type\":\"ERROR\",\"message\":\"Something bad happened\"}");
    }
}