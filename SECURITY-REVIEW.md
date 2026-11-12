# Security Review — Admin Console (`feature/admin-dashboard`)

Review date: 2026-11-12
Scope: diff between `main` and `feature/admin-dashboard`
(`WuphfWeb/Controllers/AdminController.cs`, `WuphfWeb/Pages/Admin/Login.cshtml(.cs)`,
`WuphfWeb/Pages/Admin/Dashboard.cshtml(.cs)`, `WuphfWeb/Pages/Shared/_Layout.cshtml`)

## Summary

| # | Severity | File | Lines | Vulnerability | Confidence |
|---|----------|------|-------|---------------|------------|
| 1 | CRITICAL | WuphfWeb/Controllers/AdminController.cs | 28-30 | SQL injection via string-interpolated `FromSqlRaw` in `SearchUsers` | 10/10 |
| 2 | HIGH | WuphfWeb/Pages/Admin/Login.cshtml.cs | 11-12 | Weak/broken crypto — unsalted MD5 password hash | 9/10 |
| 3 | HIGH | WuphfWeb/Pages/Admin/Login.cshtml.cs | 11-12 | Hard-coded admin credentials in source | 8/10 |
| 4 | HIGH | WuphfWeb/Controllers/AdminController.cs | 34-45 | Missing authorization check on `DeleteWuphf` (unauthenticated delete) | 9/10 |
| 5 | MEDIUM | AdminController.cs / Dashboard.cshtml | 17-45 / 90-99 | CSRF — no antiforgery token on state-changing admin POST | 8/10 |
| 6 | MEDIUM | WuphfWeb/Pages/Admin/Login.cshtml.cs | 33-37 | Insecure auth cookie (missing HttpOnly/Secure/SameSite) | 8/10 |
| 7 | MEDIUM | WuphfWeb/Pages/Admin/Dashboard.cshtml | 88-93 | DOM XSS via unescaped `innerHTML` in search results rendering | 8/10 |

## Details

### 1. SQL Injection — CRITICAL
`SearchUsers` builds a raw SQL query by directly interpolating the user-supplied
`query` parameter and executing it with `FromSqlRaw`, with no parameterization
or escaping. An attacker can break out of the `LIKE` clause to run arbitrary SQL.
**Fix:** use `FromSqlInterpolated` or a parameterized LINQ `Where(u => u.Username.Contains(query))`.

### 2. Weak/Broken Crypto (MD5) — HIGH
Admin login hashes the submitted password with MD5 and compares it to a fixed
digest. MD5 is cryptographically broken, unsalted, and trivially brute-forced
offline if the hash is ever extracted.
**Fix:** use a modern adaptive hash (bcrypt/Argon2id) with a per-credential salt.

### 3. Hard-coded Admin Credentials — HIGH
The admin username and password hash are embedded directly in source as
constants, making them recoverable from the compiled binary or repo.
**Fix:** move credentials to a secrets manager / identity provider.

### 4. Missing Authorization on Delete — HIGH
`DeleteWuphf` performs no `IsAdmin` cookie (or any) check before deleting a
record, unlike `SearchUsers`, which does check it. Any unauthenticated caller
can delete arbitrary WUPHFs.
**Fix:** add the same authorization check used elsewhere, ideally via shared
middleware/filter rather than per-action checks.

### 5. CSRF on State-changing Admin Actions — MEDIUM
Admin authorization relies solely on a cookie with no antiforgery token
validation, so a logged-in admin could be tricked into triggering destructive
actions (e.g., deletion) via a cross-site request.
**Fix:** add ASP.NET Core antiforgery tokens to state-changing admin POSTs.

### 6. Insecure Auth Cookie Flags — MEDIUM
The `IsAdmin` cookie is set without `HttpOnly`, `Secure`, or `SameSite`,
increasing exposure to XSS-based theft, plaintext transmission, and CSRF.
**Fix:** set `HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict`.

### 7. DOM XSS in Search Results — MEDIUM
Search results are injected into the DOM via `innerHTML` without escaping
`username`/`email`, allowing stored/reflected script execution if those fields
contain HTML/script markup.
**Fix:** use `textContent` or HTML-encode values before injecting into the DOM.

## Status

Findings documented only; no fixes applied in this commit, per request.
