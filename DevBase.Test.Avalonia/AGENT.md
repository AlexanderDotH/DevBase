# DevBase.Test.Avalonia Agent Guide

## Purpose

`DevBase.Test.Avalonia` is a small Avalonia desktop harness for manually exercising DevBase Avalonia color extraction packages. It loads cached PNG files, runs the LAB cluster color calculator, and visualizes RGB channel panels plus the selected color.

## Important Files

- `DevBase.Test.Avalonia.csproj`: `WinExe` test harness targeting `net9.0` with Avalonia desktop dependencies.
- `Program.cs`: Avalonia application bootstrap.
- `App.axaml` / `App.axaml.cs`: application shell.
- `MainWindow.axaml`: compact manual UI with a load button, color panels, and image preview.
- `MainWindow.axaml.cs`: loads a random PNG from `%APPDATA%/DevBase/Cache`, calculates color, and updates UI panels.

## Commands

```bash
dotnet restore DevBase.Test.Avalonia/DevBase.Test.Avalonia.csproj
dotnet build DevBase.Test.Avalonia/DevBase.Test.Avalonia.csproj --no-restore -v:minimal
dotnet run --project DevBase.Test.Avalonia/DevBase.Test.Avalonia.csproj
```

## Contributor Notes

- Treat this as a manual visual harness, not an automated test suite.
- Keep UI changes small and focused on exercising `DevBase.Avalonia` and `DevBase.Avalonia.Extension`.
- Avoid hardcoding machine-specific image paths; the current cache path is user-profile based.
- Keep `Tmds.DBus.Protocol` pinned unless the transitive Avalonia dependency no longer needs the vulnerability override.
- Do not add provider/network behavior here; use the integration test projects for that.
- When changing color calculator calls, verify the window still runs with real cached PNG files.
