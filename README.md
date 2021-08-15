# EmployeeRegisterAPI

-install-package NLog.Extensions.Logging -version 1.6.5
-Add-Migration
-Remove-Migration.
-Update-Database

//indicating versioning through a Query string
https://localhost:5001/api/companies?api-version=2.0

// With Caching
-Cache-Control: max-age=180 --indicates that the response should be cached for 180 seconds

// Validation before returning cached responses
-Marvin.Cache.Headers
