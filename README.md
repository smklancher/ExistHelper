# ExistHelper

Shows number of days since last usage of attributes in the [Exist](https://exist.io) API.  
Partly created as a "vibe coding" experiment to get used to using GitHub Copilot Edits via chat.

---

## About This Project

This project was created primarily using GitHub Copilot Edits, with iterative requests and responses guiding the development process.

### Copilot Edit Thread Summary

- Created a Blazor WebAssembly project to interact with the Exist API.
- Implemented a service class to call a web service API authorized by a Bearer token.
- Added a page for users to provide and save their token.
- Added a page to display web service results in a table.
- Registered services for dependency injection.
- Added an "Options" page and navigation entry.
- Allowed users to input username and password to fetch a token from the Exist API.
- Saved and loaded the token from local storage.
- Modified API service to accept and encode query parameters.
- Changed API result deserialization to return raw JSON or strongly-typed objects as needed.
- Added models for Exist API responses.
- Created a high-level service to fetch and filter attribute data.
- Added a page to display attribute values with filtering and sorting.
- Added a page to show attribute streaks, including days since last usage.
- Added a static logger for browser console output.
- Improved navigation and removed unused pages.
- Ensured all string fields are initialized to avoid nullability warnings.
- Added a GitHub Actions workflow to deploy the site to GitHub Pages.
- Provided clear instructions and warnings for token usage, including PowerShell and curl examples.
- Added an About link to the navigation pointing to the [GitHub repository](https://github.com/smklancher/ExistHelper).

---