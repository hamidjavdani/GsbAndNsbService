using GSB.Test.Api.Models.Requests;

namespace GSB.Test.Api.Services;

// Enforces unambiguous PDF requirements only; geometry and inconsistent wire names are preserved.
internal static class MapApprovalValidation
{
    public static void Validate(MapApprovalRequest request)
    {
        if (!request.Elzam14) throw new ArgumentException("elzam14 must be true.", nameof(request));
        MsbRequestValidation.Required(request.OrganId, "organId");
        MsbRequestValidation.Required(request.RequestUniqueId, "requestUniqueId");
        MsbRequestValidation.Required(request.IssueNo, "IssueNo", 10);
        MsbRequestValidation.Required(request.IssueDate, "IssueDate", 10);
        MsbRequestValidation.Required(request.IssueTime, "IssueTime", 5);
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.IssueTime, "^[0-9]{2}:[0-9]{2}$"))
            throw new ArgumentException("IssueTime must use xx:xx format.", nameof(request));
        if (request.ActionReferenceType is < 1 or > 6)
            throw new ArgumentException("ActionReferenceType is not in the documented enumeration.", nameof(request));
        MsbRequestValidation.Required(request.ActionReferenceNo, "ActionReferenceNo", 11);
        MsbRequestValidation.Required(request.EstateTypeCode, "EstateTypeCode", 2);
        MsbRequestValidation.Required(request.Province, "Province", 32);
        MsbRequestValidation.Required(request.Unit, "Unit", 32);
        MsbRequestValidation.Required(request.Section, "Section", 32);
        MsbRequestValidation.Required(request.SubSection, "SubSection", 32);
        MsbRequestValidation.Required(request.Basic, "Basic", 77);
        MsbRequestValidation.Required(request.Secondary, "Secondary", 77);
        MsbRequestValidation.Required(request.JamCode, "JamCode", 23);
        MsbRequestValidation.Required(request.Address, "Address", 2000);
        MsbRequestValidation.RequiredNumber(request.Area, "Area");
        MsbRequestValidation.RequiredNumber(request.TotalBlockNo, "TotalBlockNo");
        OptionalLength(request.PostalCode, "PostalCode", 10);

        // The PDF does not declare minimum array sizes; empty optional arrays remain valid.
        foreach (var block in request.Blocks ?? [])
        {
            RequireItem(block, "Block");
            MsbRequestValidation.RequiredNumber(block.BlockNo, "Block.BlockNo");
            if (block.StructureType is < 1 or > 3)
                throw new ArgumentException("StructureType is not in the documented enumeration.", nameof(request));
            MsbRequestValidation.Required(block.Limitation, "Block.Limitation", 2000);
            OptionalLength(block.BlockName, "Block.BlockName", 500);
            foreach (var level in block.Classes ?? [])
            {
                RequireItem(level, "Block.Class");
                MsbRequestValidation.RequiredNumber(level.ClassNo, "Class.ClassNo");
                MsbRequestValidation.RequiredNumber(level.ClassUnitNo, "Class.ClassUnitNo");
                MsbRequestValidation.RequiredNumber(level.ClassArea, "Class.ClassArea");
                foreach (var unit in level.EstateUnits ?? [])
                {
                    RequireItem(unit, "EstateUnits");
                    MsbRequestValidation.Required(unit.Usage, "EstateUnits.Usage", 3);
                    MsbRequestValidation.Required(unit.Limitation, "EstateUnits.Limitation", 2000);
                }
                foreach (var joint in level.Joints ?? [])
                {
                    RequireItem(joint, "Joint");
                    MsbRequestValidation.Required(joint.Code, "Joint.Code", 3);
                    MsbRequestValidation.RequiredNumber(joint.Area, "Joint.Area");
                    MsbRequestValidation.Required(joint.Usage, "Joint.Usage");
                    MsbRequestValidation.Required(joint.Sector, "Joint.Sector", 50);
                    MsbRequestValidation.Required(joint.Limitation, "Joint.Limitation", 2000);
                }
            }
        }
        foreach (var owner in request.OwnersInfo ?? [])
        {
            RequireItem(owner, "OwnersInfo");
            MsbRequestValidation.Required(owner.NationalNo, "OwnersInfo.NationalNo", 11);
            if (owner.NationalNo.Length is not (10 or 11) || !owner.NationalNo.All(c => c is >= '0' and <= '9'))
                throw new ArgumentException("NationalNo must contain 10 or 11 digits.", nameof(request));
            MsbRequestValidation.Required(owner.Type, "OwnersInfo.Type", 12);
            MsbRequestValidation.Required(owner.Name, "OwnersInfo.Name", 200);
            MsbRequestValidation.Required(owner.OwnersAddress, "OwnersInfo.OwnersAddress", 2000);
            MsbRequestValidation.Required(owner.ContactNo, "OwnersInfo.ContactNo", 11);
            if (!System.Text.RegularExpressions.Regex.IsMatch(owner.ContactNo, "^09[0-9]{9}$"))
                throw new ArgumentException("ContactNo must use the documented mobile number format.", nameof(request));
            if (owner.NationalNo.Length == 10)
            {
                MsbRequestValidation.Required(owner.Family, "OwnersInfo.Family", 200);
                MsbRequestValidation.Required(owner.BirthDate, "OwnersInfo.BirthDate", 10);
                MsbRequestValidation.Required(owner.FatherName, "OwnersInfo.FatherName", 200);
            }
            else
            {
                OptionalLength(owner.Family, "OwnersInfo.Family", 200);
                OptionalLength(owner.BirthDate, "OwnersInfo.BirthDate", 10);
                OptionalLength(owner.FatherName, "OwnersInfo.FatherName", 200);
            }
            OptionalLength(owner.ElectronicEstateNoteNo, "OwnersInfo.ElectronicEstateNoteNo", 19);
        }
        if (request.MainPlan is { } plan)
        {
            PlanType(plan.Type, false);
            OptionalLength(plan.FileName, "MainPlan.FileName", 50);
        }
        foreach (var unitPlan in request.UnitPlans ?? [])
        {
            RequireItem(unitPlan, "UnitPlans");
            PlanType(unitPlan.Type, true);
            // UnitPlan identifier types and geometry differ between PDF table and examples.
            // Do not introduce aliases, coerce geometry or infer undocumented numeric ranges.
        }
    }

    private static void RequireItem(object? item, string field)
    {
        if (item is null) throw new ArgumentException($"{field} cannot contain null entries.", "request");
    }

    private static void OptionalLength(string? value, string field, int length)
    {
        if (value?.Length > length) throw new ArgumentException($"{field} exceeds the documented maximum length.", "request");
    }

    private static void PlanType(string? type, bool unitPlan)
    {
        if (string.IsNullOrEmpty(type)) return; // File metadata is optional in the PDF.
        if (type is "image/png" or "image/jpeg" || (unitPlan && type == "application/pdf")) return;
        throw new ArgumentException("Plan Type is not a documented media type.", "request");
    }
}
