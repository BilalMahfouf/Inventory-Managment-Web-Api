# Inventory Management System — Project Overview

A full-stack inventory and sales management web application. It lets a business manage its
**products, stock, locations, customers, and sales orders** from a single dashboard, with
realtime low-stock alerts and multi-language support.

This document explains what the software does and how it is built. It is written for anyone
(developers, managers, new team members) to understand the project.

---

## 1. What the software offers (features)

### 1.1 Authentication & Users
- Login with email/password (passwords stored hashed, never in plain text).
- Session handling with access tokens that expire and can be silently refreshed.
- Forgot / reset password (reset link is emailed, token expires in 15 minutes).
- Email confirmation (one-time token, expires in 24 hours).
- User management: create, edit, activate / deactivate, soft-delete, change own password.
- User roles (Admin, Manager, Sales, Warehouse, Viewer).
- A default admin account is seeded for first use.

### 1.2 Dashboard
- KPI summary cards: total products, low-stock items, active customers, total sales orders,
  total revenue, pending orders, completed orders, active suppliers.
- "Today's performance" panel: today's orders, new customers, products sold, revenue.
- Top selling products (ranked by revenue from completed orders).
- Inventory alerts panel (low-stock and out-of-stock items).
- Quick actions (e.g. open the "add product" form directly from the dashboard).

### 1.3 Products
- Product catalog with SKU, name, description, category, pricing, and status.
- Product categories with **hierarchy** (main categories and sub-categories, tree view).
- Units of measure (kg, piece, box, etc.).
- Product images (upload, list, delete, mark one as the primary image).
- Link suppliers to products (supplier product code, lead time, minimum order quantity).
- Pricing rules: selling price can never be less than cost.
- Low-stock detection and per-product inventory overview across locations.
- Stock movement history per product (full audit trail of every stock change).

### 1.4 Inventory & Stock
- Stock levels per **product + location** combination.
- Thresholds per record: reorder level and max level.
- Automatic stock status: In Stock / Low Stock / Out of Stock.
- Stock can never go negative and never exceed the max level.
- Inventory cost and total valuation reports.
- **Stock transfers**: move stock from one location to another.
- Stock movements ledger: every stock change (purchase, sale, transfer, adjustment,
  initial stock, etc.) is recorded with reason and processed-by.

### 1.5 Locations
- Physical storage locations (warehouses, stores, shelves) with address and location type.
- Activate / deactivate a location.
- See all stock stored in a location.

### 1.6 Sales Orders
- Create orders linked to a customer, or **walk-in sales** (no customer, completed immediately).
- Orders contain line items (product, location, quantity, price).
- Full order lifecycle with status transitions:
  Pending → Confirmed → In Transit → Shipped → Completed.
- Cancel an order (stock is automatically **restored** to inventory).
- Return a completed order.
- Edit orders while they are still pending.
- Payment tracking: Unpaid / Partially Paid / Paid, with partial-amount rules.
- Shipping tracking number, notes, and shipping address.
- Order listing with filters (status, customer, date range) and summary metrics.

### 1.7 Customers
- Customer records with contact info and full address.
- Customer categories (individual vs. business).
- Customer contacts (multiple contacts per customer).
- Credit status (active, on hold, suspended, closed).
- Per-customer totals: number of orders and total amount spent.

### 1.8 Realtime Notifications
- Low-stock alerts pushed live to the UI as soon as stock drops below the reorder level
  (via SignalR, shown in a notification bell with unread count).

### 1.9 Documents & Emails
- **PDF generation** for sales orders (itemized list and totals) rendered by a headless browser.
- Emails for password reset and email confirmation.

### 1.10 Multi-language UI
- English, French, and Arabic translations with a language switcher.
- Locale-aware date and currency (DZD) formatting.

---

## 2. How it is built

### 2.1 Backend (API)
- **.NET 10 (C#)** REST API with a layered architecture:
  - **Domain** — the core business rules and entities (orders, inventory, products, etc.).
  - **Application** — the use cases/services and validation (all business logic lives here).
  - **Infrastructure** — external concerns: database access, email, file storage, PDF,
    notifications, security, background jobs.
  - **Presentation** — the HTTP API endpoints.
- **PostgreSQL** relational database.
- **JWT** access tokens + rotating refresh tokens (kept in secure HTTP-only cookies).
- **Argon2** password hashing.
- **SignalR** for realtime notifications (low-stock alerts).
- **Background job** processes domain events reliably (outbox pattern) — e.g. the low-stock
  event that triggers the realtime alert.
- **FluentValidation** for input validation.
- Automatic database migrations on startup.
- Seed/demo data (products, customers, locations, orders, etc.) for development.
- OpenAPI docs with a browsable API reference (Scalar) in development.

### 2.2 Frontend (Web App)
- **React 19** single-page application, built with **Vite**.
- **React Router** for navigation (public routes: login, forgot password, 404; protected
  routes behind an auth guard).
- **TanStack Query** for server data fetching, caching, and mutation handling.
- **TanStack Table** for data tables with search, sorting, pagination, and row actions.
- **Axios** HTTP client with automatic token refresh.
- **Tailwind CSS 4** with a shadcn/ui-style component set for the design system.
- **i18next** for English / French / Arabic localization.
- **SignalR client** for realtime notifications.
- Modals, confirmation dialogs, toasts, loading/empty/error states throughout.

### 2.3 Deployment
- Frontend is configured to deploy on **Vercel**.
- Backend runs with environment-variable configuration (database, JWT secrets, SMTP email).

---

## 3. Modules in the data model (not yet exposed)

These exist as domain entities and are used internally, but are **not yet available as API
features** — good candidates for future work:
- **Purchase orders** (buying stock from suppliers) — data model exists, no API.
- **Suppliers** — used for product-supplier links and dashboard counts, but no full
  management screens.
- **Stock transfer lifecycle** (approve / reject / in-transit / complete) — the logic exists
  in the domain, but the API only supports creating and viewing transfers.

---

## 4. Known gaps & things to add next

### Functionality
- **Settings page** is a placeholder.
- **Forgot-password** flow is a placeholder in the UI (backend supports it, frontend does not).
- **Supplier management** and **purchase order workflow** are missing.
- Full **stock transfer approval workflow** is not exposed.
- A logout endpoint on the backend.
- Real **RTL layout** for Arabic (translations exist, direction is still forced left-to-right).
- A dark-mode toggle (styles are scaffolded, no switch yet).

### Quality / bugs to fix
- A few API routes are missing authentication (`/api/location-type`,
  `/api/stock-movement-types`, `GET /api/users`) — should be protected.
- Known frontend bug in the notification bell (crashes when the first alert arrives).

---

## 5. Tech stack summary

| Layer    | Technology                                    |
|----------|-----------------------------------------------|
| Backend  | .NET 10 (C#), PostgreSQL, JWT, SignalR, Argon2 |
| Frontend | React 19, Vite, Tailwind CSS, TanStack Query/Table, Axios, i18next |
| Realtime | SignalR (low-stock notifications)              |
| Extras   | PDF generation, email sending, image storage, background jobs |
| Deploy   | Frontend on Vercel, backend as a containerized API |
