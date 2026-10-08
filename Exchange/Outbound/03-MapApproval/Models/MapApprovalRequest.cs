using System.Text.Json;
using System.Text.Json.Serialization;

namespace GSB.Test.Api.Models.Requests;

public class MapApprovalRequest
{
    [JsonPropertyName("organId")] public string OrganId { get; set; } = string.Empty;
    [JsonPropertyName("requestUniqueId")] public string RequestUniqueId { get; set; } = string.Empty;
    [JsonPropertyName("elzam14")] public bool Elzam14 { get; set; } = true;
    [JsonPropertyName("IssueNo")] public string IssueNo { get; set; } = string.Empty;
    [JsonPropertyName("IssueDate")] public string IssueDate { get; set; } = string.Empty;
    [JsonPropertyName("IssueTime")] public string IssueTime { get; set; } = string.Empty;
    [JsonPropertyName("ActionReferenceType")] public int ActionReferenceType { get; set; }
    [JsonPropertyName("ActionReferenceNo")] public string ActionReferenceNo { get; set; } = string.Empty;
    [JsonPropertyName("EstateTypeCode")] public string EstateTypeCode { get; set; } = string.Empty;
    [JsonPropertyName("Province")] public string Province { get; set; } = string.Empty;
    [JsonPropertyName("Unit")] public string Unit { get; set; } = string.Empty;
    [JsonPropertyName("Section")] public string Section { get; set; } = string.Empty;
    [JsonPropertyName("SubSection")] public string SubSection { get; set; } = string.Empty;
    [JsonPropertyName("Basic")] public string Basic { get; set; } = string.Empty;
    [JsonPropertyName("Secondary")] public string Secondary { get; set; } = string.Empty;
    [JsonPropertyName("JamCode")] public string JamCode { get; set; } = string.Empty;
    [JsonPropertyName("Address")] public string Address { get; set; } = string.Empty;
    [JsonPropertyName("Area")] public decimal? Area { get; set; }
    [JsonPropertyName("PostalCode")] public string? PostalCode { get; set; }
    [JsonPropertyName("TotalBlockNo")] public int? TotalBlockNo { get; set; }
    [JsonPropertyName("Block")] public List<MapApprovalBlock> Blocks { get; set; } = new();
    [JsonPropertyName("OwnersInfo")] public List<MapApprovalOwner> OwnersInfo { get; set; } = new();
    [JsonPropertyName("MainPlan")] public MapApprovalPlan? MainPlan { get; set; }
    [JsonPropertyName("UnitPlans")] public List<MapApprovalUnitPlan> UnitPlans { get; set; } = new();
}

public class MapApprovalBlock
{
    [JsonPropertyName("BlockNo")] public int? BlockNo { get; set; }
    [JsonPropertyName("BlockName")] public string? BlockName { get; set; }
    [JsonPropertyName("StructureType")] public int StructureType { get; set; }
    [JsonPropertyName("Limitation")] public string Limitation { get; set; } = string.Empty;
    [JsonPropertyName("Class")] public List<MapApprovalClass> Classes { get; set; } = new();
}

public class MapApprovalClass
{
    [JsonPropertyName("ClassNo")] public int? ClassNo { get; set; }
    [JsonPropertyName("ClassUnitNo")] public int? ClassUnitNo { get; set; }
    [JsonPropertyName("ClassArea")] public decimal? ClassArea { get; set; }
    [JsonPropertyName("EstateUnits")] public List<MapApprovalEstateUnit> EstateUnits { get; set; } = new();
    [JsonPropertyName("Joint")] public List<MapApprovalJoint> Joints { get; set; } = new();
}

public class MapApprovalEstateUnit
{
    [JsonPropertyName("EstateUnitNo")] public int EstateUnitNo { get; set; }
    [JsonPropertyName("UnitArea")] public decimal UnitArea { get; set; }
    [JsonPropertyName("Usage")] public string Usage { get; set; } = string.Empty;
    [JsonPropertyName("Limitation")] public string Limitation { get; set; } = string.Empty;
}

public class MapApprovalJoint
{
    [JsonPropertyName("BlockNo")] public int? BlockNo { get; set; }
    [JsonPropertyName("Code")] public string Code { get; set; } = string.Empty;
    [JsonPropertyName("Area")] public decimal? Area { get; set; }
    [JsonPropertyName("Usage")] public string Usage { get; set; } = string.Empty;
    [JsonPropertyName("Sector")] public string Sector { get; set; } = string.Empty;
    [JsonPropertyName("Limitation")] public string Limitation { get; set; } = string.Empty;
}

public class MapApprovalOwner
{
    [JsonPropertyName("NationalNo")] public string NationalNo { get; set; } = string.Empty;
    [JsonPropertyName("Type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("Name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("Family")] public string? Family { get; set; }
    [JsonPropertyName("BirthDate")] public string? BirthDate { get; set; }
    [JsonPropertyName("FatherName")] public string? FatherName { get; set; }
    [JsonPropertyName("OwnersAddress")] public string OwnersAddress { get; set; } = string.Empty;
    [JsonPropertyName("ElectronicEstateNoteNo")] public string? ElectronicEstateNoteNo { get; set; }
    [JsonPropertyName("ContactNo")] public string ContactNo { get; set; } = string.Empty;
}

public class MapApprovalPlan
{
    [JsonPropertyName("Type")] public string? Type { get; set; }
    [JsonPropertyName("FileName")] public string? FileName { get; set; }
    [JsonPropertyName("File")] public string? File { get; set; }
    [JsonPropertyName("EstateMap")] public JsonElement? EstateMap { get; set; }
}

public class MapApprovalUnitPlan
{
    [JsonPropertyName("BlockNo")] public int BlockNo { get; set; }
    [JsonPropertyName("ClassNo")] public int ClassNo { get; set; }
    [JsonPropertyName("EstateUnitNo")] public int EstateUnitNo { get; set; }
    [JsonPropertyName("Type")] public string? Type { get; set; }
    [JsonPropertyName("FileName")] public string? FileName { get; set; }
    [JsonPropertyName("File")] public string? File { get; set; }
    [JsonPropertyName("UnitMap")] public JsonElement? UnitMap { get; set; }
}
