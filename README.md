# 🎯 Interview Coach  
*A Blazor Server app for tracking and improving interview preparation — built as my MSSA‑CAD Week 8/17 mini‑project.*

---

## 🚀 What It Does  
Interview prep is easy to misjudge — you “feel” ready until you freeze or ramble in the moment. This app gives structure, repetition, and visibility into your progress.

### Current Features  
- 🧠 **Behavioral Question Practice** — write responses, score yourself across **Situation / Task / Action / Result**, and compare against a rubric  
- 📊 **Kanban Dashboard** — move responses through **Todo → WIP → Solid → Mastered** as your scores improve  
- 💾 **Session Export/Import** — everything lives in a JSON file you own; no accounts, no cloud dependency  
- 🎯 **Smart Recommendations** — surfaces your lowest‑scoring, most urgent, and most practice‑ready responses  

---

## 🛠️ Tech Stack  
- ⚙️ **.NET 10 Blazor Server**  
- 💻 **C#** throughout  
- 📦 **System.Text.Json** with polymorphic type discriminators  
- 📁 **JSON export/import** for fully user‑owned data  

---

## 📈 Status  
Actively in development.

### Coming Soon  
- ✍️ **Narrative & Technical lanes**  
- 🧩 **LeetCode‑style coding prep** (whiteboard + AI prompt handoff)  
- ⏱️ **Challenge Mode** (timed practice sessions)  
- 🔁 **Spaced repetition**  
- 🤖 **AI coaching prompts** using a **BYO‑AI** pattern  

---

## 🧩 Why Blazor Server  
- MSSA is Microsoft‑focused, so .NET was a natural fit  
- Great environment for practicing **routing**, **component design**, and **clean OOP** in a real application  

---

## 🔐 Why In‑Memory State (No Database)  
- **User‑owned data** — everything stays local  
- **Zero accounts, zero cloud storage, zero risk**  
- JSON export/import keeps the app simple, portable, and privacy‑first  

---

## 🤝 Why the BYO‑AI Pattern  
Instead of locking into a single AI provider, the app generates **coaching prompts** you can paste into whatever AI tool you prefer — Copilot, ChatGPT, Claude, Gemini, etc.

- You get feedback in your AI chat  
- You bring insights back into the app  
- Future me may add optional RAG‑powered evaluation  

---

