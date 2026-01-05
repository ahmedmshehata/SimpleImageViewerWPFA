Contributing to SimpleImageViewer

Thanks for your interest in contributing! This file describes the recommended workflow and guidelines to help your contribution be accepted quickly.

Prerequisites

- .NET 10 SDK
- Visual Studio 2026 or a compatible IDE

Getting started

1. Fork the repository (if applicable) and create a feature branch from `main`:

   - Branch name format: `feature/<short-description>` or `fix/<short-description>`

2. Build and run locally:

   ```bash
   dotnet build
   dotnet run --project SimpleImageViewer
   ```

Code style

- Keep code simple and readable.
- Follow existing project conventions and C# idioms used in the repository.
- Prefer small, focused commits with clear messages.
- Use PascalCase for types and methods; camelCase for local variables and parameters.
- If adding public APIs, include XML documentation comments.

Testing

- Add tests where applicable. If the repo doesn't yet contain a test project, open an issue before adding one.

Pull request (PR) guidelines

- Open a PR against `main` with a clear title and description of the change.
- Reference related issues by number (e.g., `Fixes #12`).
- Keep PRs focused and small when possible.
- Ensure the solution builds and run locally before submitting.

Issues

- Before opening a new issue, search existing issues to avoid duplicates.
- Provide a clear description, steps to reproduce, and environment details (OS, .NET SDK version).

Commit message format

- Use concise messages. Optionally follow Conventional Commits, e.g.:
  - `feat: add theme switcher`
  - `fix: prevent crash on empty directory`
  - `docs: update README`

Licensing and CLA

- By contributing, you agree that your contributions will be licensed under the project's license (add `LICENSE` file to the repo).

Code of conduct

- Be respectful and collaborative. Treat contributors with civility.

Need help?

- Open an issue or submit a draft PR and tag maintainers.

Thank you for helping improve SimpleImageViewer.