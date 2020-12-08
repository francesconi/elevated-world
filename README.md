# Elevated World

This sample is inspired from [Reinventing the Reader monad](https://fsharpforfunandprofit.com/posts/elevated-world-6) and a similar MSSQL implementation written by @toburger.

## Using The SQL Analyzer (VS Code)

### 1. Configure the connection string to your development database
[Docs](https://github.com/Zaid-Ajaj/Npgsql.FSharp.Analyzer/#1---configure-the-connection-string-to-your-development-database-1)
### 2. Install the analyzer using nuget
```
nuget install NpgsqlFSharpAnalyzer -OutputDirectory packages/analyzers
```
### 3. Enable analyzers in Ionide
Open _settings.json_ by using `Ctrl+Shift+P` and typing "Open Workspace Settings (JSON)".
```json
{
    "FSharp.enableAnalyzers": true,
}
```
Then restart VS Code.