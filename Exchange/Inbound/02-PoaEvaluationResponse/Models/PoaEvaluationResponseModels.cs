using System.Text.Json;
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Callback;

public class PoaEvaluationResponseRequest
{
    [JsonPropertyName("organId")]
    public string? OrganId { get; set; }

    [JsonPropertyName("owTrakingCode")]
    public string? OwTrakingCode { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("error")]
    public PoaEvaluationError? Error { get; set; }

    [JsonPropertyName("data")]
    public PoaEvaluationData? Data { get; set; }
}

public class PoaEvaluationError
{
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("errorCode")]
    public int? ErrorCode { get; set; }
}

public class PoaEvaluationData
{
    [JsonPropertyName("RegCases")]
    public List<JsonElement>? RegCases { get; set; }

    [JsonPropertyName("BaseDocuments")]
    public List<JsonElement>? BaseDocuments { get; set; }

    [JsonPropertyName("FollowerDocuments")]
    public List<JsonElement>? FollowerDocuments { get; set; }

    [JsonPropertyName("succseed")]
    public bool? Succseed { get; set; }

    [JsonPropertyName("NationalRegisterNo")]
    public string? NationalRegisterNo { get; set; }

    [JsonPropertyName("DocType")]
    public string? DocType { get; set; }

    [JsonPropertyName("DocType_code")]
    public string? CodeDocType { get; set; }

    [JsonPropertyName("HasPermission")]
    public bool? HasPermission { get; set; }

    [JsonPropertyName("ExistDoc")]
    public bool? ExistDoc { get; set; }

    [JsonPropertyName("Desc")]
    public string? Desc { get; set; }

    [JsonPropertyName("ScriptoriumName")]
    public string? ScriptoriumName { get; set; }

    [JsonPropertyName("SignGetterTitle")]
    public string? SignGetterTitle { get; set; }

    [JsonPropertyName("SignSubject")]
    public string? SignSubject { get; set; }

    [JsonPropertyName("DocDate")]
    public string? DocDate { get; set; }

    [JsonPropertyName("CaseClasifyNo")]
    public string? CaseClasifyNo { get; set; }

    [JsonPropertyName("ImpotrtantAnnexText")]
    public string? ImpotrtantAnnexText { get; set; }

    [JsonPropertyName("DocImage")]
    public string? DocImage { get; set; }

    [JsonPropertyName("DocImage_Base64")]
    public string? DocImageBase64 { get; set; }

    [JsonPropertyName("advocacyEndDate")]
    public string? AdvocacyEndDate { get; set; }

    [JsonPropertyName("lstFindPersonInQuery")]
    public List<PoaEvaluationPerson>? Persons { get; set; }
}

public class PoaEvaluationPerson
{
    [JsonPropertyName("NationalNo")]
    public string? NationalNo { get; set; }

    [JsonPropertyName("Birthdate")]
    public string? Birthdate { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("Family")]
    public string? Family { get; set; }

    [JsonPropertyName("AgentType")]
    public string? AgentType { get; set; }

    [JsonPropertyName("PersonType")]
    public string? PersonType { get; set; }

    [JsonPropertyName("PersonType_code")]
    public string? PersonTypeCode { get; set; }

    [JsonPropertyName("NationalNoMovakel")]
    public string? NationalNoMovakel { get; set; }

    [JsonPropertyName("NameMovakel")]
    public string? NameMovakel { get; set; }

    [JsonPropertyName("FamilyMovakel")]
    public string? FamilyMovakel { get; set; }

    [JsonPropertyName("txtRelation")]
    public string? TxtRelation { get; set; }

    [JsonPropertyName("RoleType")]
    public string? RoleType { get; set; }

    [JsonPropertyName("Person_RoleType_code")]
    public string? CodeRoleTypePerson { get; set; }
}
