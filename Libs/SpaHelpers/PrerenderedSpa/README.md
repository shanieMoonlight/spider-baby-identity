# PrerenderedSpa

PrerenderedSpa is a .NET middleware library for serving prerendered Single Page Application (SPA) files (such as Angular, React, or Vue) with advanced fallback and routing support. It is designed for scenarios where your SPA is prerendered to static HTML files for each route, enabling fast, SEO-friendly, and robust static hosting in ASP.NET Core.

## Features

- **Serve Prerendered HTML:** Automatically serves the correct `index.html` (or custom file) for each route, supporting directory-based routing.
- **Fallback Logic:** Configurable fallback to a global or nearest parent `index.html` if a specific route file does not exist.
- **Trailing Slash Redirects:** Optionally redirects requests without a trailing slash to the canonical version, preventing broken relative links.
- **Static File Support:** Serves all static assets (JS, CSS, images, etc.) from your SPA's output directory.
- **Flexible Configuration:** Easily configure source paths, fallback behavior, and file serving options.

## Getting Started

### 1. Install the NuGet Package

```sh
dotnet add package PrerenderedSpa
```

### 2. Configure in `Program.cs`

```csharp
using PrerenderedSpa.PrerenderedBuilder;

var builder = WebApplication.CreateBuilder(args);
// ... other service registrations ...

// In production, the SPA files will be served from this directory
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

var app = builder.Build();

// ... other middleware ...

app.UsePrerenderedSpa(new PrerenderedSpaOptions
{
    SourcePath = "ClientApp/dist",
    FallbackToNearestParentDirectory = true // Optional: fallback to nearest parent index.html
});

// ... other middleware ...
```

### 3. Example Usage

See `Apps/MyIdDemo/Program.cs` for a real-world example.

## API Overview

### `PrerenderedSpaOptions`

- `SourcePath`: Directory containing your SPA's static files (default: `wwwroot`).
- `DefaultFileName`: The default file to serve for each route (default: `index.html`).
- `FallbackFileName`: The file to serve if the default file is not found (default: `index.html`).
- `FallbackDirectoryRelativePath`: Directory to look for the fallback file (default: root of `SourcePath`).
- `FallbackToNearestParentDirectory`: If true, will search parent directories for a fallback file.
- `ServeUnknownFileTypes`: If true, serves files with unknown MIME types.

### `UsePrerenderedSpa` Extension

- Adds the middleware to your ASP.NET Core pipeline.
- Handles static file serving, default file resolution, and fallback logic.

## How It Works

- For each incoming request:
  - If the request is for a static file (e.g., `.js`, `.css`), it is served directly.
  - If the request is for a route (e.g., `/about`), the middleware checks for `/about/index.html`.
  - If not found, it falls back to a parent or global `index.html` as configured.
  - If the request is missing a trailing slash, it redirects to the canonical path to ensure relative links work.

## Advanced

- **Custom Fallbacks:** Configure fallback file and directory for advanced routing scenarios.
- **Trailing Slash Redirect:** Ensures browser resolves relative links correctly (important for SEO and resource loading).

## License

MIT

---

**Note:** For best results, prerender your SPA with a tool that outputs a static HTML file for each route (e.g., Angular Universal, React Static Export, Scully, etc.).

---

If you need more details or want to see the full API, check the source code or the included unit tests for usage patterns.
