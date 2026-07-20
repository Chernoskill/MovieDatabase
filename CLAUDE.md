# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

MovieDatabase is a .NET MAUI cross-platform app targeting Android and Windows (net10.0). The single `.csproj` at `MovieDatabase/MovieDatabase.csproj` covers all platforms. Currently at early scaffold stage.
MovieDatabase lets users view movie information in a list.
The main view is a list of movie information. Each movie information contains the name, tag, years of release, personal favorite status, and storage identification string of an owned movie.
The Storage Identification string is a string typed by the user. It lets the user know where the physical copy of the movie is stored.
Tags are used for movie genres, example: Action, Comedy.
In the list, each movie area on has an edit button that opens the editing view for this movie inforamtion. The user can edit all properties like name and year.
The main view has a filter button. When the filter button is clicked, an overlay in the upper part of the screen appears. In the overlay, the user can select a range between two years, a name or name part, tags, and favorite status. Applying these settings filters the list view.
The mobile app has another tab for a configuration view. In the configuration view, the user can export and import the movie inforamtion collection as a file. The user can also add, rename or delete tags.


## Build & Run

Build for Windows (primary dev target):
```
dotnet build MovieDatabase/MovieDatabase.csproj -f net10.0-windows10.0.19041
```

Run on Windows (unpackaged — no MSIX required):
```
dotnet run --project MovieDatabase/MovieDatabase.csproj -f net10.0-windows10.0.19041
```

Run on Android emulator:
```
dotnet run --project MovieDatabase/MovieDatabase.csproj -f net10.0-android
```

`WindowsPackageType` is set to `None`, so Windows builds run unpackaged without needing the MSIX/Store pipeline.

## Architecture

- **Entry point**: `MauiProgram.cs` → `App.xaml.cs` → `AppShell.xaml`
- **Navigation**: Shell-based (`AppShell.xaml`). Add new pages as `ShellContent` entries there and register routes for code-behind navigation.
- **Pages**: Each page is a XAML + code-behind pair (e.g., `MainPage.xaml` / `MainPage.xaml.cs`). The `GlobalXmlns.cs` registers the `MovieDatabase` and `MovieDatabase.Pages` namespaces under the global MAUI XAML URI so pages can be referenced in XAML without per-file `xmlns` declarations.
- **Resources**: Shared colors in `Resources/Styles/Colors.xaml`, styles in `Resources/Styles/Styles.xaml`. Fonts, images, raw assets, and the app icon are declared in the `.csproj` as `MauiFont`, `MauiImage`, and `MauiAsset` items.
- **Platform overrides**: Per-platform code lives under `Platforms/{Android,iOS,MacCatalyst,Windows,Tizen}/`. Prefer MAUI abstractions over platform-specific code; only drop into `Platforms/` when a MAUI API doesn't cover the need.
