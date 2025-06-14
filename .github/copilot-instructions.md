# Copilot Instructions

## General
Always use braces around if statements, even for single-line statements.
Always initialize string properties, fields, and locals to avoid nullability warnings.

## Blazor WebAssembly Specific
Because the base href will be set on build, don't prefix links throughout the app with a forward slash. Either avoid the use of a path segment separator or use dot-slash (./) relative path notation.