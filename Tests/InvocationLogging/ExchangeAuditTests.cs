using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Callback;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

internal static class ExchangeAuditTests
{
    public static async Task RunAsync(IServiceProvider provider, IConfiguration configuration)
    {
        var options = Options.Create(new MsbSettings
        {
            ApiKeyHeaderName = "msb-identifier", ApiKey = "TEST-API-KEY",
            RuleId = "MISCONFIGURED-RULE", PoaInquiryRuleId = "MISCONFIGURED-RULE"
        });
        using var scope = provider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<IMsbInvocationLogger>();
        var fake = new ContractHandler();
        using var client = new HttpClient(fake);
        var factory = new AuditFactory(client);
        var accessor = new HttpContextAccessor();
        var document = new DocumentVerificationService(factory, options, logger,
            NullLogger<DocumentVerificationService>.Instance, configuration, accessor);
        var poa = new PoaInquiryService(factory, options, logger,
            NullLogger<PoaInquiryService>.Instance, configuration, accessor);
        var map = new MapApprovalService(factory, options, logger,
            NullLogger<MapApprovalService>.Instance, configuration, accessor);
        var cancellation = new Made14CancellationService(factory, options, logger,
            NullLogger<Made14CancellationService>.Instance, configuration, accessor);

        // Official table fields, not a list inferred from implementation reflection.
        foreach (var field in new[] { "RequesterName", "RequesterFamily", "RequesterNationalCode",
            "RequesterOfficProvinceName", "RequesterOfficeCode", "RequesterOfficNumber",
            "RequestUniqueId", "ElectronicEstateNoteNo", "NationalityCode" })
        {
            foreach (var invalid in new string?[] { null, "", "   " })
            {
                var request = Document();
                typeof(G2GInquiryRequest).GetProperty(field)!.SetValue(request, invalid);
                await Rejected(() => document.G2GInquiryAsync(request), fake);
            }
        }
        await Rejected(() => document.G2GInquiryAsync(null!), fake);
        var wrongDocument = Document(); wrongDocument.RuleId = "INVALID-RULE";
        await Rejected(() => document.G2GInquiryAsync(wrongDocument), fake);

        foreach (var field in new[] { "NationalRegisterNo", "SecretNo", "RequesterName", "RequesterFamily",
            "RequesterNationalCode", "RequesterOfficProvinceName", "RequesterOfficeCode", "RequesterOfficNumber", "RequestUniqueId" })
        {
            foreach (var invalid in new string?[] { null, "", "   " })
            {
                var request = Poa();
                typeof(PoaInquiryRequest).GetProperty(field)!.SetValue(request, invalid);
                await Rejected(() => poa.SendAsync(request), fake);
            }
        }
        await Rejected(() => poa.SendAsync(null!), fake);
        var wrongPoa = Poa(); wrongPoa.RuleId = "INVALID-RULE";
        await Rejected(() => poa.SendAsync(wrongPoa), fake);

        foreach (var field in new[] { "OrganId", "RequestUniqueId", "IssueNo", "IssueDate", "IssueTime",
            "ActionReferenceNo", "EstateTypeCode", "Province", "Unit", "Section", "SubSection", "Basic", "Secondary", "JamCode", "Address" })
        {
            var request = Map();
            typeof(MapApprovalRequest).GetProperty(field)!.SetValue(request, " ");
            await Rejected(() => map.SendAsync(request), fake);
        }
        foreach (var field in new[] { "Area", "TotalBlockNo" })
        {
            var request = Map();
            typeof(MapApprovalRequest).GetProperty(field)!.SetValue(request, null);
            await Rejected(() => map.SendAsync(request), fake);
        }
        foreach (var change in new Action<MapApprovalRequest>[]
        {
            r => r.Elzam14 = false, r => r.ActionReferenceType = 7, r => r.IssueTime = "12:00:00",
            r => r.IssueNo = new string('1', 11), r => r.Blocks[0].BlockNo = null,
            r => r.Blocks[0].StructureType = 4, r => r.Blocks[0].Limitation = "",
            r => r.Blocks[0].Classes[0].ClassNo = null, r => r.Blocks[0].Classes[0].ClassUnitNo = null,
            r => r.Blocks[0].Classes[0].ClassArea = null,
            r => r.Blocks[0].Classes[0].EstateUnits[0].Usage = "",
            r => r.Blocks[0].Classes[0].EstateUnits[0].Limitation = "",
            r => r.Blocks[0].Classes[0].Joints[0].Area = null,
            r => r.Blocks[0].Classes[0].Joints[0].Code = "",
            r => r.Blocks[0].Classes[0].Joints[0].Sector = "",
            r => r.Blocks[0].Classes[0].Joints[0].Usage = "",
            r => r.Blocks[0].Classes[0].Joints[0].Limitation = "",
            r => r.OwnersInfo[0].NationalNo = "invalid", r => r.OwnersInfo[0].ContactNo = "invalid",
            r => r.OwnersInfo[0].Name = "", r => r.OwnersInfo[0].Type = "",
            r => r.OwnersInfo[0].OwnersAddress = "", r => r.OwnersInfo[0].Family = null,
            r => r.OwnersInfo[0].BirthDate = null, r => r.OwnersInfo[0].FatherName = null,
            r => r.MainPlan!.Type = "application/pdf", r => r.UnitPlans[0].Type = "application/zip",
            r => r.OwnersInfo.Add(null!), r => r.Blocks.Add(null!)
        })
        {
            var request = Map(); change(request);
            await Rejected(() => map.SendAsync(request), fake);
        }
        await Rejected(() => map.SendAsync(null!), fake);
        var falseToken = Cancel(); falseToken.NoToken = false;
        await Rejected(() => cancellation.CancelMade14Async(falseToken), fake);
        var invalidRule = Cancel(); invalidRule.RuleId = "INVALID-RULE-ID";
        await Rejected(() => cancellation.CancelMade14Async(invalidRule), fake);
        foreach (var field in new[] { "OrganId", "OwTrackingCode", "RuleId" })
        {
            var request = Cancel();
            typeof(Made14CancellationRequest).GetProperty(field)!.SetValue(request, " ");
            await Rejected(() => cancellation.CancelMade14Async(request), fake);
        }
        foreach (var change in new Action<Made14CancellationRequest>[]
        {
            r => r.Data = null!, r => r.Data.CancelReason = " ", r => r.Data.ActionId = " "
        })
        {
            var request = Cancel(); change(request);
            await Rejected(() => cancellation.CancelMade14Async(request), fake);
        }
        await Rejected(() => cancellation.CancelMade14Async(null!), fake);
        Check(fake.Calls == 0, "All validation failures reject before any outbound send");

        var registered = Document();
        fake.Path = "/interagency/document-verification-inquiry/G2GInquery";
        var documentResponse = await document.G2GInquiryAsync(registered);
        Check(registered.RuleId == "mhrne7iv" && documentResponse.Code == 200 && documentResponse.Msg == "OK" &&
            documentResponse.OwTrakingCode == "TEST-OUTBOUND", "Document official rule and response");
        await VerifyLog(provider, "DocumentVerification", "mhrne7iv");
        Check(fake.Body!.Contains("TEST-REQUESTER") && fake.Body.Contains("0000000000"), "Wire request retains document data");

        var poaRequest = Poa();
        fake.Path = "/interagency/poa/G2GInquery";
        var poaResponse = await poa.SendAsync(poaRequest);
        Check(poaRequest.RuleId == "mhlu20po" && poaResponse.Code == 200 && poaResponse.Msg == "OK" &&
            poaResponse.OwTrakingCode == "TEST-OUTBOUND", "POA official rule and response");
        await VerifyLog(provider, "PoaInquiry", "mhlu20po");
        Check(fake.Body!.Contains("TEST-POA-SECRET"), "Wire request retains secretNo");

        fake.Path = "/made14/map-approval/G2GInquery";
        var mapResponse = await map.SendAsync(Map());
        Check(mapResponse.GetProperty("custom").GetInt32() == 7, "Map preserves unspecified raw JSON response schema");
        await VerifyLog(provider, "MapApproval", null);
        Check(fake.Body!.Contains("TEST-FILE-DATA") && fake.Body.Contains("TEST-OWNER") &&
            fake.Body.Contains("EPSG:4326"), "Wire map data unchanged by log sanitizer");
        // Zero is a present numeric value; no undocumented positive-only restriction is added.
        var zero = Map(); zero.Area = 0; zero.TotalBlockNo = 0;
        zero.Blocks[0].Classes[0].ClassNo = 0;
        await map.SendAsync(zero);
        var legalOwner = Map(); legalOwner.OwnersInfo[0].NationalNo = "14000000000";
        legalOwner.OwnersInfo[0].Family = legalOwner.OwnersInfo[0].BirthDate = legalOwner.OwnersInfo[0].FatherName = null;
        await map.SendAsync(legalOwner);

        fake.Path = "/made14/ebtal/v1/response";
        foreach (var rule in new[] { "mgrjz6mo", "mgw2bnhz" })
        {
            var request = Cancel(); request.RuleId = rule;
            var response = await cancellation.CancelMade14Async(request);
            Check(response.Code == 200 && response.Msg == "OK", "Cancellation official response");
            await VerifyLog(provider, "Made14Cancellation", rule);
        }

        // Failures must finish the same sanitized log and preserve the original exception type.
        foreach (var service in new[] { "DocumentVerification", "PoaInquiry", "MapApproval" })
        {
            foreach (var mode in new[] { "http", "network", "json" })
            {
                fake.Mode = mode;
                try
                {
                    if (service == "DocumentVerification") { fake.Path = "/interagency/document-verification-inquiry/G2GInquery"; await document.G2GInquiryAsync(Document()); }
                    if (service == "PoaInquiry") { fake.Path = "/interagency/poa/G2GInquery"; await poa.SendAsync(Poa()); }
                    if (service == "MapApproval") { fake.Path = "/made14/map-approval/G2GInquery"; await map.SendAsync(Map()); }
                    throw new Exception("Expected outbound failure");
                }
                catch (Exception ex) when (ex is HttpRequestException or JsonException) { }
                var log = await Latest(provider, service);
                Check(!log.IsSuccess && log.CompletedAt.HasValue && log.ErrorType is not null, "Failure completion persisted");
                Check(!JsonSerializer.Serialize(log).Contains("TEST-RESPONSE-SECRET"), "Failure payload omitted or masked");
            }
        }
        fake.Mode = "ok";

        var throwing = new ThrowingLogger();
        var tolerant = new PoaInquiryService(factory, options, throwing,
            NullLogger<PoaInquiryService>.Instance, configuration, accessor);
        fake.Path = "/interagency/poa/G2GInquery";
        Check((await tolerant.SendAsync(Poa())).Code == 200, "New outbound logging tolerates begin failure");
        throwing.BeginFails = false;
        Check((await tolerant.SendAsync(Poa())).Code == 200, "New outbound logging tolerates completion failure");

        Sanitization(configuration);
        var pdfPoa = """{"data":{"DocType_code":"DOC","DocImage_Base64":"TEST-IMAGE","ADVOCACYENDDATE":"1405/01/01","lstFindPersonInQuery":[{"PersonType_code":"PERSON","Person_RoleType_code":"ROLE"}]}}""";
        var parsed = JsonSerializer.Deserialize<PoaEvaluationResponseRequest>(pdfPoa,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        Check(parsed.Data!.CodeDocType == "DOC" && parsed.Data.DocImageBase64 == "TEST-IMAGE" &&
            parsed.Data.AdvocacyEndDate == "1405/01/01" && parsed.Data.Persons![0].PersonTypeCode == "PERSON" &&
            parsed.Data.Persons[0].CodeRoleTypePerson == "ROLE", "PDF POA names deserialize without invented aliases");
    }

    private static void Sanitization(IConfiguration configuration)
    {
        foreach (var key in new[] { "ApiKey", "Authorization", "token", "password", "secret", "SeCrEt_No",
            "Cookie", "File", "DocImage_Base64", "64Base_DocImage", "NationalNo", "ContactNo" })
        {
            var obj = new JsonObject { [key] = "TEST-PRIVATE-VALUE", ["echo"] = "TEST-PRIVATE-VALUE", ["safe"] = 7 };
            var result = new InvocationLogSanitizer(configuration).Body(obj.ToJsonString())!;
            Check(!result.Contains("TEST-PRIVATE-VALUE") && result.Contains("\"safe\":7"), "Private key and cross-field echo masked");
        }
        var nested = """{"OwnersInfo":[{"Name":"TEST-OWNER","ContactNo":"09123456789","extra":{"File":"TEST-FILE-DATA"}}],"echo":"TEST-OWNER TEST-FILE-DATA"}""";
        var masked = new InvocationLogSanitizer(configuration).Body(nested)!;
        Check(!masked.Contains("TEST-OWNER") && !masked.Contains("09123456789") && !masked.Contains("TEST-FILE-DATA"), "Owner subtree and echoes omitted");
        var sharedName = """{"OwnersInfo":[{"Name":"TEST"}],"organId":"TEST-ORGAN","message":"Owner TEST responded"}""";
        var retained = new InvocationLogSanitizer(configuration).Body(sharedName)!;
        Check(retained.Contains("TEST-ORGAN") && !retained.Contains("Owner TEST responded"),
            "Personal-name echo masked without corrupting operational IDs");
        var large = "{\"File\":\"" + new string('A', InvocationLogSanitizer.BodyLimit) + "\"}";
        Check(new InvocationLogSanitizer(configuration).Body(large)!.StartsWith("[OMITTED"), "Large binary payload omitted");
    }

    private static async Task Rejected(Func<Task> action, ContractHandler handler)
    {
        var calls = handler.Calls;
        var rejected = false;
        try { await action(); } catch (ArgumentException) { rejected = true; }
        Check(rejected && handler.Calls == calls, "Invalid request rejected before handler");
    }

    private static async Task<MsbInvocationLog> Latest(IServiceProvider provider, string service)
    {
        using var scope = provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().MsbInvocationLogs.AsNoTracking()
            .Where(x => x.ServiceName == service && x.Direction == "Outbound").OrderByDescending(x => x.CreatedAt).FirstAsync();
    }

    private static async Task VerifyLog(IServiceProvider provider, string service, string? rule)
    {
        var log = await Latest(provider, service);
        Check(log.HttpMethod == "POST" && log.ContentType == "application/json" && log.HttpStatusCode == 200 &&
            log.IsSuccess && log.CompletedAt.HasValue && log.RuleId == rule && log.ResponseCode == "200" && log.ResponseMessage == "OK", "Outbound log contract");
        if (service != "Made14Cancellation") Check(log.RequestId == "TEST-REQUEST-ID", "Top-level requestUniqueId captured");
        if (service is "MapApproval" or "Made14Cancellation") Check(log.OrganId == "14002861227", "Outbound organId captured");
        foreach (var secret in new[] { "TEST-API-KEY", "TEST-POA-SECRET", "TEST-REQUESTER", "0000000000",
            "TEST-OWNER", "TEST-FILE-DATA", "09123456789", "TEST-RESPONSE-SECRET" })
            Check(!JsonSerializer.Serialize(log).Contains(secret), "Complete database log excludes private data");
    }

    private static G2GInquiryRequest Document() => new()
    {
        RequesterName = "TEST-REQUESTER", RequesterFamily = "TEST-FAMILY", RequesterNationalCode = "0000000000",
        RequesterOfficProvinceName = "TEST-PROVINCE", RequesterOfficeCode = "TEST-OFFICE", RequesterOfficNumber = "TEST-NUMBER",
        RequestUniqueId = "TEST-REQUEST-ID", ElectronicEstateNoteNo = "TEST-DOCUMENT", NationalityCode = "0000000000"
    };

    private static PoaInquiryRequest Poa() => new()
    {
        NationalRegisterNo = "TEST-REGISTER", SecretNo = "TEST-POA-SECRET", RequesterName = "TEST-REQUESTER",
        RequesterFamily = "TEST-FAMILY", RequesterNationalCode = "0000000000", RequesterOfficProvinceName = "TEST-PROVINCE",
        RequesterOfficeCode = "TEST-OFFICE", RequesterOfficNumber = "TEST-NUMBER", RequestUniqueId = "TEST-REQUEST-ID"
    };

    private static Made14CancellationRequest Cancel() => new()
    {
        OrganId = "14002861227", OwTrackingCode = "TEST-CANCEL", RuleId = "mgrjz6mo",
        Data = new() { CancelReason = "TEST LOCAL ONLY", ActionId = "TEST-ACTION" }
    };

    private static MapApprovalRequest Map() => new()
    {
        OrganId = "14002861227", RequestUniqueId = "TEST-REQUEST-ID", IssueNo = "TEST-ISSUE", IssueDate = "1405/01/01",
        IssueTime = "12:00", ActionReferenceType = 1, ActionReferenceNo = "14000000000", EstateTypeCode = "01",
        Province = "01", Unit = "01", Section = "01", SubSection = "01", Basic = "1", Secondary = "2", JamCode = "TEST-JAM",
        Address = "TEST-ADDRESS", Area = 25, TotalBlockNo = 1,
        Blocks = [new() { BlockNo = 1, StructureType = 1, Limitation = "center", Classes = [new()
        {
            ClassNo = 0, ClassUnitNo = 1, ClassArea = 25,
            EstateUnits = [new() { EstateUnitNo = 1, UnitArea = 20, Usage = "1", Limitation = "center" }],
            Joints = [new() { Code = "1", Area = 5, Usage = "1", Sector = "1", Limitation = "center" }]
        }] }],
        OwnersInfo = [new() { NationalNo = "0000000000", Type = "1", Name = "TEST-OWNER", Family = "TEST-FAMILY",
            BirthDate = "1400/01/01", FatherName = "TEST-FATHER", OwnersAddress = "TEST-ADDRESS", ContactNo = "09123456789" }],
        MainPlan = new() { Type = "image/png", File = "TEST-FILE-DATA", EstateMap = JsonSerializer.SerializeToElement(new
            { Type = "FeatureCollection", CoordinateReferenceSystem = new { Properties = new { Name = "EPSG:4326" } }, Features = Array.Empty<object>() }) },
        UnitPlans = [new() { BlockNo = 1, ClassNo = 0, EstateUnitNo = 1, Type = "application/pdf", File = "TEST-FILE-DATA" }]
    };

    private static void Check(bool condition, string description)
    {
        TestState.Assertions++;
        if (!condition) throw new Exception("Assertion failed: " + description);
    }

    private sealed class AuditFactory(HttpClient client) : IHttpClientFactory
    { public HttpClient CreateClient(string name) => client; }

    private sealed class ThrowingLogger : IMsbInvocationLogger
    {
        public bool BeginFails = true;
        public Task<Guid> BeginAsync(MsbInvocationLog log) => BeginFails ? throw new InvalidOperationException("TEST-PRIVATE-VALUE") : Task.FromResult(Guid.NewGuid());
        public Task CompleteAsync(Guid id, MsbInvocationCompletion completion) => throw new InvalidOperationException("TEST-PRIVATE-VALUE");
    }

    private sealed class ContractHandler : HttpMessageHandler
    {
        public int Calls;
        public string Path = "";
        public string Mode = "ok";
        public string? Body;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Calls++;
            Check(request.Method == HttpMethod.Post && request.RequestUri!.AbsoluteUri == "http://pmsbgw.shahr-bank.ir" + Path,
                "Official POST URL captured by fake only");
            Check(request.Headers.GetValues("msb-identifier").Single() == "TEST-API-KEY" &&
                request.Content!.Headers.ContentType!.MediaType == "application/json", "Header and media type");
            Body = await request.Content!.ReadAsStringAsync(token);
            using var body = JsonDocument.Parse(Body);
            if (Path.Contains("document-verification")) Check(body.RootElement.GetProperty("ruleId").GetString() == "mhrne7iv", "Official document rule on wire");
            if (Path.Contains("/poa/")) Check(body.RootElement.GetProperty("ruleId").GetString() == "mhlu20po", "Official POA rule on wire");
            if (Path.Contains("map-approval")) Check(body.RootElement.GetProperty("elzam14").GetBoolean(), "Official map fixed value on wire");
            if (Path.Contains("ebtal")) Check(body.RootElement.GetProperty("noToken").GetBoolean() &&
                body.RootElement.GetProperty("ruleId").GetString() is "mgrjz6mo" or "mgw2bnhz", "Official cancellation fixed values on wire");
            if (Mode == "network") throw new HttpRequestException("TEST-RESPONSE-SECRET");
            return new HttpResponseMessage(Mode == "http" ? HttpStatusCode.BadGateway : HttpStatusCode.OK)
            {
                Content = new StringContent(Mode == "json" ? "malformed TEST-RESPONSE-SECRET" : Mode == "http" ?
                    "{\"secretNo\":\"TEST-RESPONSE-SECRET\",\"msg\":\"TEST-RESPONSE-SECRET\"}" :
                    "{\"code\":200,\"msg\":\"OK\",\"owTrakingCode\":\"TEST-OUTBOUND\",\"custom\":7}", Encoding.UTF8, "application/json")
            };
        }
    }
}
