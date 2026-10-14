# Inventory Management System

## Project Overview

A full-stack web application for managing inventory and sales in one place. It lets a small business track products, stock levels across multiple locations, customers, and sales orders from a single dashboard, with live low-stock alerts. It replaces manual spreadsheets and disconnected stock tracking with a centralized system that keeps inventory counts accurate as orders are created, shipped, cancelled, or returned.

## Problem and Value

- **Problem:** Small shops and warehouses often track stock manually (spreadsheets, paper, guesswork), which leads to overselling, stockouts, lost orders, and no clear picture of revenue or best-selling products.
- **Intended users:** Small retail businesses, stores, and warehouses that hold stock in one or more locations and sell to customers (including walk-in sales).
- **Why it is useful:** One dashboard shows what is in stock, what is running low, who bought what, and how the business performed today. Stock counts update automatically when orders change state, so staff do not have to reconcile inventory by hand. Realtime low-stock alerts, printable order PDFs, and English/French/Arabic UI make it practical for day-to-day use in a multilingual environment.

## Key Features

- **Product catalog:** Products with SKU, category, pricing, and status; hierarchical categories (main + sub-categories with tree view); units of measure (piece, kg, box); product images with a primary-image flag; supplier links per product with lead time and minimum order quantity.
- **Inventory tracking per location:** Stock quantities tracked per product + location combination, with reorder and max levels, automatic In Stock / Low Stock / Out of Stock status, and rules that prevent negative stock or exceeding max.
- **Stock movements ledger:** Every stock change (purchase, sale, transfer, adjustment, initial stock) is recorded with reason and who processed it; per-product movement history and inventory valuation reports.
- **Stock transfers:** Move stock between locations; transfer history is viewable.
- **Locations:** Warehouses, stores, and shelves with address, type, and active/inactive state; view all stock held in a location.
- **Sales orders:** Create orders for a customer or walk-in sales; line items with product, location, quantity, and price; full lifecycle Pending → Confirmed → In Transit → Shipped → Completed; cancel restores stock automatically; returns supported; edit while pending; payment tracking (Unpaid / Partially Paid / Paid); tracking number, notes, and shipping address; filtering by status, customer, and date range.
- **Customers:** Contact info and address, individual vs. business categories, multiple contacts per customer, credit status, and per-customer order totals and spending.
- **Dashboard:** KPI cards (products, low-stock items, customers, orders, revenue, pending/completed orders, suppliers), today's performance panel, top-selling products by revenue, inventory alerts panel, and quick actions (e.g., add product directly from the dashboard).
- **Realtime low-stock alerts:** Push notifications appear in the UI (notification bell with unread count) as soon as stock drops below the reorder level.
- **Documents and email:** Itemized sales-order PDFs; password-reset and email-confirmation emails.
- **Multi-language UI:** English, French, and Arabic with a language switcher and locale-aware date/currency (DZD) formatting.
- **Authentication and users:** Email/password login with hashed passwords, expiring access tokens with silent refresh, password reset, email confirmation, user management (create, edit, activate/deactivate, soft-delete), and five roles (Admin, Manager, Sales, Warehouse, Viewer).

## Technical Highlights

- **REST API with real business rules:** Around 109 endpoints across 17 route groups (auth, users, products, inventory, sales, customers, dashboard, and more). Core rules live server-side: selling price cannot be below cost, stock can never go negative, cancelling an order restores stock, and order status follows a controlled lifecycle.
- **Relational database design:** PostgreSQL database covering products, inventory per location, movements, transfers, customers, and orders; automatic migrations on startup plus seed/demo data for development.
- **Authentication and authorization:** JWT access tokens plus rotating refresh tokens in secure HTTP-only cookies, Argon2 password hashing, and role-based access on endpoints.
- **Realtime plus reliable background work:** Low-stock events flow through an outbox table and a scheduled background job, then push to the browser over SignalR — so alerts are not lost if part of the system is briefly busy.
- **PDF, email, images, validation:** Order PDFs rendered with a headless browser from templates, SMTP emails for account flows, image uploads with storage management, and server-side input validation on requests.
- **Modern React frontend:** Single-page app with protected routes, server-driven tables (search, sort, pagination), cached data fetching with automatic refresh after changes, modals and confirmation dialogs, toasts, and loading/empty/error states throughout.
- **Testing and CI:** Unit plus integration test suites covering most application services plus PDF generation; a CI workflow builds the solution and runs the test suites on pull requests to main.

## Technology Stack

- **Languages:** C# (.NET 10), JavaScript (React frontend, no TypeScript).
- **Backend:** ASP.NET Core REST API (Carter-style minimal endpoints), Entity Framework Core, FluentValidation, JWT Bearer auth, SignalR hubs, Quartz background jobs, PuppeteerSharp + Handlebars (PDF), MailKit (SMTP email).
- **Frontend:** React 19, Vite, React Router, TanStack Query, TanStack Table, Axios, Tailwind CSS 4 with Radix Dialog primitives, i18next + react-i18next, SignalR client, lucide-react icons.
- **Databases and storage:** PostgreSQL (primary database), local/server file storage for product images and generated PDFs.
- **Infrastructure and deployment:** Docker support for the backend API, Vercel configuration for the frontend, environment-variable configuration (database, JWT secrets, SMTP, API origin), Scalar/OpenAPI docs in development.
- **Testing and tooling:** xUnit-style unit tests and integration tests (backend), ESLint + Prettier (frontend), GitHub Actions CI (restore → build → unit tests → integration tests).

## Implementation Status

- **Implemented and verified (code + tests + docs agree):** Auth and users with roles; product catalog with categories, units, and images; per-location inventory with thresholds and status rules; movement ledger and transfers (create/view); locations; customers; full sales-order lifecycle with payments, cancel-restore, returns, and walk-in sales; dashboard KPIs and reports; SignalR low-stock alerts via outbox job; order PDFs; confirmation/reset emails; EN/FR/AR translations; seed data; CI pipeline.
- **Partially implemented:** Stock-transfer approval workflow (domain logic exists, API only supports create/view); supplier and purchase-order areas (data model exists, no management screens/API); forgot-password backend works but frontend page is a placeholder; settings page is a placeholder.
- **Known issues (documented, not fixed):** Notification bell crashes when the first alert arrives; a few routes lack authentication (`/api/location-type`, `/api/stock-movement-types`, `GET /api/users`) and should be protected; Arabic has translations but layout is still forced left-to-right; dark-mode styles scaffolded but no toggle; one documented test failure around duplicate-username handling.
- **Planned / not built:** Full purchase-order workflow, supplier management screens, transfer approval screens, real RTL layout, backend logout endpoint polish.

## My Contribution

- Solo-built end-to-end as a personal portfolio project: the entire .NET backend and the entire React frontend.
- Sole committer across roughly 290 commits (backend, frontend, tests, deployment setup, bug fixes).
- Personally implemented: database design and API endpoints, order/inventory business logic, JWT auth with roles, outbox + realtime low-stock notifications, PDF/email/image services, all frontend feature pages (dashboard, products, inventory, sales, customers, auth shell), trilingual UI, seed data, Docker/Vercel deployment setup, and the unit + integration test suites.

## Results and Evidence

- **Scale of what was built:** ~109 API route registrations in 17 endpoint groups; 7 frontend feature areas plus shared app shell; 3-language UI (EN/FR/AR); 50+ backend test files covering services, domain events, and PDF generation; CI runs build + unit + integration tests on every pull request to main.
- **Deployment/usage:** Local/demo only — runs locally with seed data; frontend prepared for Vercel and backend containerized, but there is no live production deployment and no real business users yet.
- **Measured impact:** None available — no production users, revenue, time-saved, or performance measurements exist, and none are claimed.

## CV Bullet Points

- Built a full-stack inventory and sales management app solo with a .NET REST API and React dashboard, covering products, multi-location stock, customers, and orders.
- Implemented a sales-order lifecycle (pending to completed) with automatic stock deduction on confirmation and stock restoration on cancellation, plus partial-payment tracking and walk-in sales.
- Added realtime low-stock alerts delivered to the browser via background job processing and SignalR push notifications.
- Secured the API with JWT access/refresh tokens, Argon2 password hashing, email confirmation and password reset, and five role levels.
- Shipped a trilingual (English/French/Arabic) responsive frontend with searchable, sortable, paginated tables and printable order PDFs.
- Backed the backend with unit and integration test suites and a CI pipeline that builds and tests every pull request.

## Additional Information Needed

- A live demo URL or short demo video link (would strengthen the CV once available).
- A rough line/count of test cases passing in CI (e.g., "200+ tests green") once confirmed by a full test run.
- Any future pilot usage (shop name, number of products/orders managed) if the app gets trialed in a real business.
- Confirmation of the duplicate-username fix and notification-bell crash fix once resolved (both are currently documented as open issues).

## Notes for CV Generation

- **Lead with:** solo full-stack ownership + real business workflow (stock can never go negative, cancel restores stock, payments, PDFs, realtime alerts). These are concrete and recruiter-friendly.
- **Prioritize:** .NET + PostgreSQL + React, JWT auth with roles, order/inventory logic, SignalR realtime, testing + CI, trilingual UI.
- **Do not claim:** production deployment, real users, revenue, time saved, performance gains, or team leadership — none are supported by evidence.
- **Keep separate:** supplier/purchase-order screens and transfer approvals are model-only or partial; settings and forgot-password UI are placeholders. Mention scope honestly if asked.
- **Tone:** simple, direct verbs (built, implemented, added, secured, shipped, backed). Avoid architecture jargon, pattern names, and class/file lists.
