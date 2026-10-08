Run from the repository root:

```powershell
dotnet run --project .\Tests\InvocationLogging\InvocationLogging.Tests.csproj --configuration Release
```

This assertion-based regression suite runs the existing controllers on a loopback-only Kestrel server, uses an isolated EF InMemory database and an HTTP mock for cancellation, and never loads local credentials or connects to SQL Server/MSB. It checks authentication, acknowledgments, business storage, log lifecycle, recursive masking, request metadata, validation, and resilience to persistence and custom logger failures. A failed assertion exits with a nonzero status.

`ExchangeAuditTests` also covers all four registered outbound services with fake handlers: PDF required fields, official rule IDs, fixed booleans, unambiguous nested map/owner validation, exact outbound URLs, unchanged wire payloads, sanitized database logs, HTTP/network/JSON failures and logger failures. POA callback JSON mappings and registration/unique-identifier required fields are checked. No undocumented business lookup or CRS conversion is simulated as implemented behavior.
