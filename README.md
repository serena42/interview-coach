# 🎯 Interview Coach
*A Blazor Server app for structured, trackable interview preparation — built as my MSSA-CAD Week 8/17 mini-project.*

---

## What It Does

Interview prep is easy to misjudge — you "feel" ready until you freeze or ramble in the moment. This app gives structure, repetition, and visibility into your actual progress across behavioral, narrative, and technical question types.

---

## Features

### 🧠 Behavioral & Narrative Practice
- Write and score responses using the **STAR framework** (Situation / Task / Action / Result)
- Score each section 1–10 against a rubric; overall score is computed automatically
- Narrative questions use a single freeform response with a score slider
- Practice mode includes a **Toastmasters-style timer** with color-coded zones (grey / green / yellow / red) based on target time ranges per question type
- Save practice attempts with duration and notes

### 📊 Kanban Dashboard
- Responses move through **Todo → Wip → Solid → Mastered** automatically based on score thresholds
- Wip → Solid at score ≥ 7, Solid → Mastered at score ≥ 9
- Manual demotion available
- Smart recommendation panel surfaces: most urgent (by deadline), needs most work (lowest score), and ready to practice (highest scoring Wip)

### 🔁 Spaced Repetition — Technical & Coding Review
- Simplified spaced repetition algorithm inspired by SM-2
- Cards rated 1–5; interval doubles or triples on high ratings, resets to 1–3 days on low ratings
- Difficulty gating: all Basic cards must be seen before Intermediate unlocks, Intermediate before Advanced
- Session composition: all due cards (no cap) + configurable new cards (default 5, capped at 20 total)
- Progress saved on every rating — bail out mid-session without losing work

### 💾 Session Export / Import
- All data serialized to a single portable JSON file
- Polymorphic serialization via `[JsonDerivedType]` handles `StarResponse` and `FreeformResponse` subtypes correctly on round-trip
- **Auto-save** to temp folder on every response save
- Restore prompt on app load if an unsaved session is detected
- Last exported timestamp shown in nav; "Not yet exported" warning in amber

---

## Tech Stack

| Layer | Choice |
|---|---|
| Framework | .NET 10 Blazor Server |
| Language | C# |
| Serialization | System.Text.Json 
| State | In-memory singleton services (`InterviewService`, `SessionService`, `RubricService`) |
| Seed data | JSON files loaded at startup (`Behavioral.json`, `CSharp_technical.json`, `CSharp_coding.json`) |

---

## Architecture Notes

- **`InterviewResponse`** is the polymorphic base class; `StarResponse` and `FreeformResponse` extend it. `[JsonDerivedType]` attributes on the base class enable correct object creation without a custom converter. (Ploymorphic serialization)
- **`InterviewService`** is a singleton holding all questions and responses in memory. Status promotion logic lives here — `UpdateStatus()` handles all response types including a fallback for future types.
- **`SessionService`** handles import/export and auto-save. Depends on `InterviewService` via constructor injection; avoids circular dependency by calling `AutoSave()` directly from pages rather than through a callback.
- **`BuildReviewSession()`** builds the spaced repetition queue — due cards shuffled within difficulty groups, new cards appended at the unlocked difficulty tier.
- Blazor's `System.Threading.Timer` is used for the practice timer. `InvokeAsync(StateHasChanged)` marshals UI updates back to the render thread safely.

---

## Why No Database

- User-owned data — everything stays local as a JSON file
- Zero accounts, zero cloud storage, zero vendor dependency
- Simple to reason about and demo; import/export makes sessions fully portable

---

## Project Status

Actively in development as part of MSSA-CAD cohort Week 8/17 mini-project.

### Completed
- Behavioral and Narrative practice with STAR wizard and scoring
- Kanban dashboard with smart recommendations
- Technical and Coding spaced repetition review
- Session export/import with auto-save and restore

### Planned
- `Coding.razor` — syntax and implementation practice page
- `Interview.razor` — full mock interview simulator
- Challenge mode — timed mixed practice sessions
