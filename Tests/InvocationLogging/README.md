Run from the repository root:

```powershell
dotnet run --project .\Tests\InvocationLogging\InvocationLogging.Tests.csproj --configuration Release
```

This assertion-based regression suite runs the existing controllers on a loopback-only Kestrel server, uses an isolated EF InMemory database and an HTTP mock for cancellation, and never loads local credentials or connects to SQL Server/MSB. It checks authentication, acknowledgments, business storage, log lifecycle, recursive masking, request metadata, validation, and resilience to persistence and custom logger failures. A failed assertion exits with a nonzero status.
