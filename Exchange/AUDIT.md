# Exchange integration audit

Scope: each service's adjacent official PDF, README and current implementation. Local verification uses fake outbound handlers, loopback inbound HTTP and EF InMemory; it does not prove external MSB acceptance. No route requiring MSB confirmation, geometry contract, production database or deployment is changed.

| Service | Direction | Endpoint | Request/response contract | Validation | Invocation logging | Persistence | Final status |
|---|---|---|---|---|---|---|---|
| DocumentOwnershipVerification | Inbound | Intentional coordinated `/v1/create` difference | PDF envelopes and typed results retained | Existing required/conditional checks tested | Sanitized middleware | SanadCallbacks retained | PASS |
| PoaEvaluationResponse | Inbound | Additional `/v1` still needs MSB approval | Two definite JSON names fixed; expiry capitalization preserved | Existing checks retained | Sanitized middleware | PoaEvaluationCallbacks retained | MSB CONFIRMATION REQUIRED |
| RegistrationResponse | Inbound | PDF endpoint/service ID empty; implementation route retained | PDF request and acknowledgment | Six required fields tested | Sanitized middleware | SanadCallbacks retained | MSB CONFIRMATION REQUIRED |
| UniqueIdentifierResponse | Inbound | Exact official route | Required arrays/workflow/user fields; PDF inconsistencies recorded | Existing required checks tested; business lookups absent | Sanitized middleware; owner/identity data masked | UniqueIdentifierCallbacks retained | BUSINESS RULE REQUIRED |
| DocumentVerification | Outbound | Exact default official route | PDF request and response retained | Required strings and official rule enforced | FIXED: secure shared outbound capture | Sanitized invocation records only | FIXED |
| PoaInquiry | Outbound | Exact default official route | PDF request and response retained | Required strings and official rule enforced | FIXED: secretNo/requester identity masked | Sanitized invocation records only | FIXED |
| MapApproval | Outbound | Exact default official route | Ambiguous names/geometry unchanged; JsonElement response retained | Definite fields/nested requirements fixed | FIXED: owner/file/base64/geometry omitted | Sanitized invocation records only | MSB CONFIRMATION REQUIRED |
| Made14Cancellation | Outbound | Exact default official route | Official request/response retained | FIXED: official rules and noToken=true enforced | Secure outbound capture; response details omitted from HTTP exceptions | Sanitized invocation records only | FIXED |

## Confirmations and business rules still required

- All PDFs name `X-MSB-Api-Key`; the application uses the configured header, locally `msb-identifier`. The audit preserves that setting. Deployment configuration/header coordination must be confirmed separately; PASS/FIXED above describes the local audit scope, not certification of external acceptance.
- DocumentOwnershipVerification: official `/document-ownership-verification/create` versus coordinated runtime `/document-ownership-verification/v1/create`; no route change. PDF polygon table/sample representation differs, and `JsonElement` preserves it.
- POA: official `/interagency-inquiry/poa-evaluation-response/create` versus runtime `/interagency-inquiry/poa-evaluation-response/v1/create`. Table and sample agree on `DocType_code`, `Person_RoleType_code`, `DocImage_Base64`, `PersonType_code`; only the first two previously differed from the model and were corrected. `ADVOCACYENDDATE`/`advocacyEndDate` remains the document's capitalization difference, accepted case-insensitively. No invented aliases are added.
- Registration: official endpoint and service ID are **Not specified in official document**; `/made-14/registration-response/v1/create` remains an implementation route. Validation of organId, owTrakingCode, status, result, result.code and result.msg was already correct and is unchanged.
- Unique identifier: codes `101`, `102`, `105`, `106` require authoritative tracking lookup, duplicate, mismatch and registration-state rules, not just response constants. None are invented. The controller explicitly contains `100`, `103`, `104`, `107`; ASP.NET's automatic model-state handling can return a 400 problem response before the explicit `103` branch. PDF user identity requirements conflict with its one-identity-per-user examples; current either-identity logic is preserved. Attachment-name casing and sample-only per-land tracking fields remain documented.
- Map: `ActionRefrenceNo`/`ActionReferenceNo`, `Limitaion`/`Limitation`, string-versus-number UnitPlans identifiers and FeatureCollection-versus-Polygon geometry require MSB confirmation. External CRS is `EPSG:4326`; no inferred conversion or new geometry representation is introduced. The PDF specifies no response schema, so the raw JsonElement response is retained. Only unambiguous numeric required fields become nullable internally to detect omission; zero remains valid where the PDF states no range, and optional arrays have no invented minimum size.
- Cancellation: the PDF header table says application/json, its curl says text/plain. The code follows the table; the curl also contains malformed punctuation. Tests now exercise the registered service with both official rules and reject noToken=false. HTTP-success logging is not a separate check of business response success.
- Document/POA inquiries: the error example spells `msg ` with trailing whitespace, unlike the table/model `msg`. No response-field alias is invented.

## Security and database boundaries

Invocation logs receive only sanitized bodies. Credential fields (including secretNo), cookies/authorization, files/base64 and unnecessary personal fields are masked recursively; malformed/oversized bodies are omitted. Personal owner/user subtrees and map geometry are omitted. Known sensitive values echoed in text are masked without corrupting operational IDs that merely share a short name fragment. Headers containing actual key values are not captured. Logger diagnostics persist exception type only, not exception text or objects. Logging failures remain isolated from business processing.

The logger's separate DbContext scopes, SQL timeout and schema are unchanged; no migration or database update is performed. Inbound business RawJson persistence is separate from invocation logging and remains unchanged, including the unique-identifier workflow token; the sanitizer does not rewrite business records. POA inquiry secretNo and outbound map owners/files have no business persistence path.

## Verification

The regression suite checks all four inbound callbacks and all four outbound services using fake handlers, plus validation-before-send, official fixed values, safe persisted logs, response deserialization and error handling. GitHub Actions runs this suite before the existing loopback registration smoke test. Local keys are not loaded by the suite. appsettings.json is excluded from the audit commit.
