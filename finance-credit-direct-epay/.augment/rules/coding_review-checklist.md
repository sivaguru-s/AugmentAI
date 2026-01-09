type: "always_apply"
description: "Generic code and documentation review checklist for any repository."
---

# Code Review Checklist

Use this checklist when reviewing changes in any repository.

> Goal: Ensure changes are **correct, safe, maintainable, consistent with standards, and well-tested**.

---

## 1. Scope & Intent
- [ ] Is the **problem / feature** clearly stated (issue, ticket, or PR description)?
- [ ] Does the change set stay within the intended **scope** (no hidden features or refactors)?
- [ ] Are there any **surprising side‑effects** (APIs, data models, behavior) that should be called out?

---

## 2. Architecture & Design
- [ ] Does the change follow existing **architecture patterns** (layers, boundaries, domain separation)?
- [ ] Are responsibilities aligned with **SOLID principles** (no god classes/functions)?
- [ ] Is new business logic placed in the **correct layer** (e.g., service vs controller vs UI)?
- [ ] Are cross‑cutting concerns (logging, validation, auth, error handling) handled in the **standard way**?
- [ ] If introducing or changing an API, is there **OpenAPI or equivalent documentation** updated?

---

## 3. Standards & Conventions
- [ ] Does the code follow the **language / framework style guide** (formatting, naming, imports)?
- [ ] Are there any **duplicated patterns** that should be refactored to a shared helper or module?
- [ ] Are **magic numbers / strings** replaced with named constants or configuration where appropriate?
- [ ] For front‑end/UI work, does it align with the project's **design system / UI guidelines** (no ad‑hoc styles without justification)?

---

## 4. Correctness & Robustness
- [ ] Are all **happy‑path flows** clearly implemented and easy to follow?
- [ ] Are **edge cases** handled (null/empty, large input, timeouts, missing data, race conditions)?
- [ ] Is **input validation** present at the appropriate boundaries (API, UI, services)?
- [ ] Are **error conditions** handled with meaningful messages and not silently swallowed?
- [ ] Are there any **obvious race conditions** or concurrency issues (async, multi‑threading, shared resources)?

---

## 5. Security & Privacy
- [ ] Are all **inputs validated and sanitized** (especially for APIs, forms, and external data)?
- [ ] Are **secrets, keys, and credentials** kept out of source control and config checked into git?
- [ ] Are **authorization checks** enforced for protected operations (role‑based access where applicable)?
- [ ] Are there any potential **injection, XSS, CSRF, or deserialization** risks?
- [ ] Is sensitive data **redacted from logs** and error messages?

---

## 6. Performance & Scalability
- [ ] Are there any **obvious performance issues** (N+1 queries, unnecessary loops, large in‑memory structures)?
- [ ] Are database queries **indexed appropriately** and using efficient access patterns?
- [ ] For new features, is the **expected load** considered (throughput, latency, limits)?
- [ ] Are there long‑running operations that should be **async, queued, or batched** instead?

---

## 7. Testing (TDD‑Aligned)
- [ ] Were tests **added or updated first**, before the implementation where feasible (TDD mindset)?
- [ ] Are there **unit tests** covering core logic and edge cases?
- [ ] Are there **integration tests** where multiple components interact (DB, services, APIs)?
- [ ] For critical flows, are there **end‑to‑end tests** or at least a clear plan to add them?
- [ ] Do tests run **quickly and deterministically** (no flaky or timing‑dependent tests)?
- [ ] Do tests assert on **meaningful behavior**, not just implementation details?

---

## 8. Tooling & Automation
- [ ] Do all **linters, formatters, and static analyzers** pass (no new warnings/errors)?
- [ ] Does the project **build/compile** cleanly in CI and locally?
- [ ] Are any new **scripts or tooling changes** documented and easy to run?

---

## 9. Documentation & Clarity
- [ ] Is there an appropriate **README / docs update** for new features or major changes?
- [ ] Are complex sections of code **commented where necessary**, without restating the obvious?
- [ ] Are **function, class, and module names** descriptive and consistent with existing terminology?
- [ ] If behavior is non‑obvious, is there a short **rationale** in comments or docs explaining why?

---

## 10. Dependencies & Integrations
- [ ] Are any **new dependencies** truly necessary, well-maintained, and appropriately versioned/pinned?
- [ ] Are **external services or APIs** handled robustly (timeouts, retries, circuit breaking, clear errors)?
- [ ] Are **configuration and environment changes** (env vars, feature flags, secrets) documented and safe across environments (dev/stage/prod)?

---

## 11. Changelog & Versioning
- [ ] Is **Changelog.md** (or equivalent change log) updated with date, files changed, and a brief description?
- [ ] If this is a notable change, is the **version bumped** appropriately (e.g., minor vs patch vs major)?
- [ ] Are commit messages **clear and consistent**, following this repository's conventions (e.g., conventional commits, or prefixes like `augment_ai:` for AI‑generated commits)?

---

## 12. Overall Review Decision
- [ ] Do you understand the change well enough to **confidently approve** it?
- [ ] Is the change **small and focused enough**? If not, should it be split into smaller PRs?
- [ ] Would you be comfortable **owning this code** in production?

If any important box is unchecked, leave a **clear review comment** explaining:
- What is missing or concerning
- Why it matters (risk, maintainability, standards)
- A concrete suggestion or example of how to improve it

