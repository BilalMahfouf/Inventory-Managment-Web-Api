# Frontend Project Summary

This is a React-based inventory management frontend that covers products, product categories, units of measure, inventory, stock locations, stock transfers, customers, customer categories, sales orders, and dashboard reporting. It replaces manual operational tracking with localized summary cards, search-driven tables, tabbed CRUD workflows, and realtime low-stock notifications. My role was to build the application shell, route structure, feature pages, reusable UI components, server-driven data tables, forms and dialogs, API integration, localization, and the order and inventory workflow logic.

# Tech Stack

- React 19 and JSX: the core UI framework used to build the full single-page application.
- Vite with React SWC: the build and dev toolchain used for fast startup, hot reload, and production bundling.
- JavaScript ES modules: the codebase is implemented in JavaScript rather than TypeScript, so the frontend is lightweight and direct.
- React Router DOM: used for public routes, protected routes, nested layouts, redirects, dynamic detail pages, and not-found handling.
- TanStack Query: used for server state, request caching, query invalidation, loading states, and mutation workflows across every feature area.
- TanStack Table: used for both server-side and client-side data tables with sorting, filtering, pagination, and custom row actions.
- Axios: used as the centralized HTTP client for REST API calls, auth headers, token refresh, and error extraction.
- i18next and react-i18next: used for English, French, and Arabic translations, language switching, and locale-aware UI text.
- Tailwind CSS 4: used for the design system, responsive layout, utility styling, and local theme variables.
- shadcn/ui-style primitives and Radix Dialog: used for dialog and modal infrastructure, plus local component primitives such as input and dialog wrappers.
- react-tabs: used for tabbed page layouts and multi-step or segmented forms.
- lucide-react: used for the full icon set across navigation, cards, tables, forms, and status badges.
- @microsoft/signalr: used for realtime low-stock notification delivery from the backend hub.
- clsx and tailwind-merge: used to compose conditional Tailwind class names safely.
- tailwindcss-animate and tw-animate-css: used for toast and overlay motion.
- Note: React Hook Form, Zod, Zustand, framer-motion, and charting libraries are not present in this frontend.

# Architecture

- The app entry point is split cleanly: main.jsx boots i18n before rendering, and App.jsx wraps the router with React Query, toast, and auth providers.
- Routing is centralized in router.jsx. Public routes cover login, forgot password, and 404. Protected routes are nested under AuthGuard and MainLayout, then branch into dashboard, products, inventory, sales-orders, customers, and settings.
- The codebase is organized by feature under src/features. Each feature groups pages, components, services, hooks, utilities, and implementation notes, which keeps product, inventory, customer, sales, and auth logic isolated.
- Shared cross-cutting code lives in src/shared. That layer contains the Axios client, query client, query keys, i18n bootstrap, reusable hooks, SignalR wiring, date and currency formatters, toast context, and small shared utilities.
- Reusable app shell components live in src/components. That layer contains the responsive layout, top nav, sidebar, page header, info cards, button, input, dialog, confirmation dialog, tables, notification bell, language switcher, logo, and toast container.
- State management is mostly local state plus React Query. Forms and dialogs use component state for inputs and validation, while server data, caches, and mutations are handled with React Query.
- Authentication is handled through a token manager in the Axios layer, an AuthProvider that hydrates session state and listens for unauthorized events, and an AuthGuard that blocks protected routes until auth is resolved.
- The API layer is centralized and consistent. The axios instance injects bearer tokens, refreshes expired sessions with a single in-flight refresh promise, and extracts error messages from a variety of backend payload shapes.
- Forms are implemented with manual validation rather than a form library. Most create and edit flows maintain explicit errors state, set field-specific messages, and drive save buttons and toasts from local state.
- Loading and error handling are explicit throughout the UI. Tables use loading flags, forms disable actions while mutations are pending, skeletons are shown on detail pages, and destructive flows use confirmation dialogs.
- Responsive behavior is built into the shell and feature pages. The sidebar collapses into a drawer on smaller screens, the top nav adapts to mobile and desktop, and tables and modals use scrollable containers and max-width constraints.
- Localization is wired into the architecture, not bolted on. The app persists the selected language, translates all major feature labels, and formats numbers, dates, and currency with locale-aware utilities. Arabic resources exist, but the current document direction is still forced to LTR in the i18n bootstrap.
- Theme support is based on CSS variables and Tailwind tokens. The project has a light application aesthetic with color tokens for primary, success, warning, error, and info states.
- Code organization is reinforced by path aliases in Vite and jsconfig, which keep feature and shared imports readable.

# Features I Built

## Global App Shell and Navigation

The application shell provides the structural foundation for all protected pages.
- MainLayout and Layout wrap the routed content with the fixed top bar, collapsible sidebar, toast portal, and responsive main content padding.
- The sidebar exposes the main application destinations: dashboard, products, inventory, sales-orders, customers, and settings.
- The top bar includes search, language switching, the notification bell, and the user avatar placeholder.
- The active nav state is handled with NavLink-based sidebar items so the current section is always visible.
- The shell is responsive, with a mobile menu button and animated sidebar drawer behavior.

## Authentication and Session Management

The auth flow covers sign-in, session bootstrap, protected routing, and unauthorized recovery.
- The login page includes email and password fields, password visibility toggling, a remember-me style sign-in flow through React Query, and error feedback from the API.
- The forgot-password route exists as a routed placeholder with a back-to-login action, so the navigation path is wired even though recovery itself is not implemented.
- AuthProvider initializes the session from the access token manager, attempts refresh-token bootstrap when needed, and updates auth state when tokens change.
- AuthGuard blocks protected routes while auth is loading and redirects unauthenticated users back to the login screen.
- The Axios client injects bearer tokens automatically, refreshes access tokens after 401 responses, and redirects on unrecoverable auth failures.
- Logout is handled through the auth service, which calls the backend logout endpoint, clears local auth state, and returns the user to the login route.

## Dashboard

The dashboard is a management overview page built around KPI cards, realtime operational panels, and fast entry actions.
- The page renders summary cards for total products, total customers, low stock items, total sales orders, total revenues, pending orders, fulfilled orders, and active suppliers.
- Revenue numbers and performance values are formatted with the local currency helper so the numbers remain locale-aware and consistent.
- The Today Performance panel shows today’s sales, new orders, new customers, and products sold, with loading states and a fallback data set if the summary request fails.
- The Top Selling Products widget uses a ranked API response and displays product name, units sold, and revenue.
- The Alerts widget consumes inventory alert data and renders the first few alert cards for low-stock items.
- Quick Actions provides shortcut tiles, including an action that opens the AddProduct modal directly from the dashboard.
- The dashboard page also includes a generate report button and integrates with localized titles, descriptions, and card labels.

## Products Management

The products area is the largest feature slice and combines catalog management, category management, units of measure, stock movement history, and product summary metrics.
- The Products page shows summary cards for total products, inventory value, low stock items, and profit potential, then segments the page into tabs for products, stock movements, units of measure, product categories, and product images.
- ProductDataTable is the main catalog table. It supports server-side search, sorting, and pagination, and exposes view, edit, and delete actions.
- AddProduct is a multi-tab create and edit modal that handles basic information, pricing, inventory, and details. It loads categories, units of measure, and locations, calculates profit margin, markup, and per-unit profit, and invalidates both product and inventory caches after success.
- ProductViewDialog is a read-only modal with tabbed sections for basic information, pricing, and details, plus audit data and derived profit metrics.
- StockMovementHistoryTable shows historical stock movements with date, product, type, quantity, reason, and processed-by columns, using badge styles to distinguish in, out, adjustment, and transfer movements.
- Product categories are fully managed through ProductCategoryDataTable, AddProductCategory, and ProductCategoryView. The workflow supports main and sub categories, parent selection, hierarchy viewing, and category audit details.
- Units of measure are managed through UnitOfMeasureTable, AddUnitOfMeasure, and UnitOfMeasureView, including create, edit, delete, and read-only detail views with audit metadata.
- The products area also includes demo and example artifacts such as AddProductDemo and DeleteDialogExample, which document integration patterns for the modal and confirmation flow.

## Inventory Management

The inventory module tracks quantity-on-hand, reorder thresholds, storage locations, and potential profit.
- The Inventory page exposes summary cards for total inventory items, total potential profit, low stock items, and out of stock items, then switches between inventory, stock movements, stock transfers, stock alerts, and locations.
- InventoryDataTable is a server-side table with product, location, quantity, reorder, max, status, and potential profit columns. It uses status badges for in-stock, low-stock, and out-of-stock records.
- AddUpdateInventory is a multi-step modal for creating and editing inventory records. In create mode it searches for products, selects a location, and sets stock thresholds; in edit mode it locks product and location and edits stock levels only.
- The inventory form validates that a product and location are selected, blocks negative stock thresholds, and ensures reorder level does not exceed max level.
- ViewInventoryDialog provides a tabbed detail modal for product and location info, stock levels, and audit metadata, including stock percentage and derived stock status.
- The location section of inventory is connected to location lookups and summary invalidation so the inventory and product views stay in sync after mutations.
- Stock-related values are displayed with locale-aware currency and date formatting helpers.

## Stock Locations

Location management is implemented as a dedicated CRUD slice inside inventory.
- LocationDataTable renders the location list with name, address, location type, created date, and created-by columns.
- AddUpdateLocation creates and edits locations with name, address, location type, and active state, and validates required fields before save.
- ViewLocation shows read-only location details, status, location type, audit metadata, and deleted-state handling.
- Location type options are loaded from the location type API so the form can support multiple warehouse or stock-site types.
- Location mutations invalidate both the location list and inventory summary so dashboard and inventory metrics stay current.

## Stock Transfers

Stock transfers are implemented as a separate operational workflow for moving stock between locations.
- StockTransferDataTable shows product, from location, to location, quantity, status, user, and created date, with a read-only detail dialog for each transfer.
- AddStockTransfer is a tabbed modal with product selection, transfer route details, and history. It supports search by product ID, selection of valid source and destination locations, quantity entry, and transfer notes.
- The transfer form validates that a product is selected, both locations are chosen, locations are different, and quantity is greater than zero.
- ProductInfoTab and TransferDetailsTab are reusable subcomponents that split product lookup from route configuration and summary calculation.
- HistoryTab renders transfer status, creation time, created-by information, and a status timeline view.
- ViewStockTransfer is a detail dialog with transfer details, items, and history tabs, plus localized status badges and route information.
- Transfer creation invalidates both inventory and product caches because a transfer changes quantities and availability.

## Customers Management

The customers area covers customer records, customer categories, and read-only customer detail views.
- The Customers page renders summary cards for total customers, total revenue, new customers, and a stock-related placeholder card, then switches between customer catalog, customer categories, and customer contacts.
- CustomerDataTable is a server-side table with customer name and email, phone, category, total orders, total spent, active status, and created date columns.
- AddUpdateCustomer is a tabbed modal with basic information and summary tabs. It handles name, email, phone, customer category, and full address fields, and it validates required values plus email format.
- ViewCustomer renders a structured read-only profile with general information, address, and system metadata, including customer code and status badge.
- Customer categories are handled by CustomerCategoryDataTable, AddUpdateCustomerCategory, and ViewCustomerCategory.
- Customer category management supports individual versus business classification, freeform description, category lookup, detail viewing, and delete confirmation.
- The customer data layer is wired into the sales order flow so the order creation screen can fetch customers for the dropdown selector.

## Sales Orders

The sales module is a full order-management workflow with listing, creation, detail viewing, editing, payment updates, and status transitions.
- OrdersPage and SalesPage both present sales summaries and route into the order table and create-order flow. The orders view focuses on the list, while the sales page acts as an overview entry point with recent orders.
- OrdersDataTable is a server-side table with filters for status, customer, and date range. It supports navigation to detail pages, edit access for pending orders, and a row action menu for transitions and payment updates.
- The order action model is status-driven. Available actions are derived from a status transition matrix, and the action configuration determines button labels, variants, and whether confirmation or tracking input is required.
- StatusBadge and PaymentStatusBadge provide consistent visual treatment for order state and payment state across list and detail screens.
- UpdatePaymentModal lets the user change payment status and amount, including the partial-payment case where the amount must be strictly between zero and the order total.
- OrderActionBar renders the status transition buttons on the detail page and in compact mode on the edit page.
- CreateOrderPage supports walk-in sales and customer-linked orders, lets the user pick a payment status, builds order items with the OrderItemsForm component, and submits notes with the order.
- EditOrderPage is restricted to pending orders only, preloads the current line items, allows editing payment status, items, notes, and shipping address, and redirects back to the detail page after save.
- OrderDetailPage is a read-only view with customer, date, payment status, notes, shipping address, tracking number, line items, order total, and the full action bar.
- The sales hooks useOrders, useOrder, useCreateOrder, useUpdateOrder, useOrderTransition, and useUpdateOrderPayment encapsulate query and mutation logic for all order flows.
- The sales order service normalizes backend payloads, maps order items and totals, and exposes create, update, payment, and transition endpoints.

## Sales Order Item Builder

OrderItemsForm is one of the most technically involved UI pieces in the frontend.
- It is a controlled dynamic line-item editor that supports adding and removing order rows, per-row product lookup, location selection, quantity, and unit price editing.
- Product search can be done by ID or SKU. SKU search is debounced, and ID search runs immediately against the product detail endpoint.
- Each selected product triggers inventory lookup so the location picker can be limited to stock-bearing locations only.
- The form prevents duplicate product and location combinations and warns when a row conflicts with another row.
- Quantity is capped against available stock, and the component reflects the available stock directly in the UI.
- Unit price, subtotal, and total are all formatted with the currency helper, and the currency suffix adapts to the active locale.
- The empty state, loading states, retry affordance, and mobile table layout make this a polished reusable order-entry component.

## Realtime Notifications

The top nav includes a realtime notification center for low-stock alerts.
- NotificationBell connects to the SignalR hub, listens for the low-stock alert method, and stores incoming messages in local state.
- The bell shows an unread count badge, renders a dropdown list of notifications, and supports marking all as read or removing individual notifications.
- The dropdown closes when the user clicks outside of it, which keeps the interaction predictable on desktop.
- useSignalR manages the SignalR subscription lifecycle, and signalRService creates and reuses the hub connection.
- The notification experience is currently scoped to low-stock alerts, which makes the feature tightly aligned with the inventory domain.

## Localization and Formatting

Localization is a real product feature rather than a translation layer bolted on later.
- The app supports English, French, and Arabic translations through i18next resources.
- LanguageSwitcher lets the user change languages from the top navigation, and the chosen language is persisted in localStorage.
- Translation keys are centralized in keyContainer.js so feature components can stay consistent and avoid hardcoded strings.
- Date formatting is handled by a cached Intl-based utility that normalizes the locale and supports date-only or date-time output.
- Currency formatting is handled by a separate DZD formatter that adds the proper suffix for Arabic versus Latin locales.
- Dashboard, products, customers, inventory, locations, stock transfers, and sales all consume localized labels, statuses, buttons, and empty-state text.
- Arabic resources exist, but the i18n bootstrap currently forces document direction to LTR, so RTL support is not yet fully enabled.

## Error, Loading, and Empty States

I built a consistent set of feedback states across the frontend.
- Tables show loading flags, search placeholders, and empty rows or no-data prompts when APIs return nothing.
- Detail pages use skeleton loaders or centered loading states while query data resolves.
- Toast notifications surface success, warning, info, and error outcomes for create, update, delete, and transition operations.
- Confirmation dialogs are used for destructive actions such as deletes and status transitions that need user confirmation.
- Many async components provide retry or recovery affordances when lookups fail, especially in order-item, inventory, and transfer workflows.
- The not-found page provides a clear 404 experience with go-back and return-home actions.

## Settings and Placeholder Routes

The settings route exists, but the frontend implementation is still a placeholder.
- SettingsPage is routed and rendered, but it currently contains a stub instead of a finished settings interface.
- The forgot-password route is also a placeholder, which means the navigation and UI shell are ready even though the backend or flow is not implemented.

# UI Components

- Button: used throughout the app for form submission, navigation, dashboard actions, and dialog confirmation buttons.
- Input: used in create/edit forms and several search fields as the shared text and number input primitive.
- Dialog primitives: used by product, category, unit, inventory, location, transfer, and customer detail modals.
- ConfirmationDialog: used for delete flows, destructive transitions, and other confirmation-required actions.
- DataTable: the shared server-side table component used by products, customers, inventory, stock transfers, and sales orders.
- SimpleDataTable: the shared client-side table component used by product categories, units of measure, locations, and customer categories.
- PageHeader: used on dashboard, products, customers, inventory, and sales pages for the title and description block.
- InfoCard: used for dashboard and feature summary metrics across products, customers, inventory, and sales.
- Sidebar, MainLayout, Layout, TopNav, SideBarLink, and Logo: together form the application shell and navigation framework.
- LanguageSwitcher: used in the top nav to switch the active locale.
- NotificationBell: used in the top nav to surface realtime low-stock alerts.
- ToastContainer and Toast: used as the global feedback system for success, error, warning, and info messages.
- useToast and ToastContext: shared notification state and helper API for feature components.
- ProductViewDialog, ProductCategoryView, UnitOfMeasureView, ViewInventoryDialog, ViewLocation, ViewStockTransfer, ViewCustomer, and ViewCustomerCategory: read-only detail views for the main domain entities.
- AddProduct, AddProductCategory, AddUnitOfMeasure, AddUpdateInventory, AddUpdateLocation, AddStockTransfer, AddUpdateCustomer, and AddUpdateCustomerCategory: the main create and edit modal components.
- ProductDataTable, ProductCategoryDataTable, UnitOfMeasureTable, InventoryDataTable, LocationDataTable, StockTransferDataTable, CustomerDataTable, CustomerCategoryDataTable, and OrdersDataTable: the main list components.
- OrderItemsForm, OrderActionBar, StatusBadge, PaymentStatusBadge, and UpdatePaymentModal: reusable sales workflow components.
- ProductInfoTab, TransferDetailsTab, and HistoryTab: reusable stock transfer modal sections.
- PerformanceCard, TodaysPerformance, TopProductItem, QuickActions, Alerts, and Alert: dashboard subcomponents.
- AddProductDemo and DeleteDialogExample: example components that document how to integrate the product modal and confirmation dialog.

# Forms

- Login form: purpose is authentication; fields are email and password; validation is manual and the API error state clears the password on failure; it uses custom input components and a password visibility toggle.
- AddProduct: purpose is product creation and editing; fields include product name, SKU, category, description, status, cost price, selling price, current stock, minimum stock, maximum stock, unit of measure, and storage location; validation is manual and the form also calculates profit margin and markup.
- AddProductCategory: purpose is product taxonomy management; fields include name, description, category type, and parent category; validation requires a name and parent selection for subcategories.
- AddUnitOfMeasure: purpose is unit definition management; fields include name and description; validation requires a unit name.
- AddUpdateCustomer: purpose is customer creation and editing; fields include name, email, phone, customer category, street, city, state, and zip code; validation checks required fields and email format.
- AddUpdateCustomerCategory: purpose is customer classification management; fields include name, individual-versus-business flag, and description; validation checks required name and length limits.
- AddUpdateInventory: purpose is inventory record creation and editing; fields include product search, location selection, quantity on hand, reorder level, and max level; validation checks required selections and stock threshold rules.
- AddUpdateLocation: purpose is warehouse or location management; fields include name, address, location type, and active state; validation checks required text fields and type selection.
- AddStockTransfer: purpose is stock movement between locations; fields include product, from location, to location, quantity, and notes; validation checks product presence, two distinct locations, and positive quantity.
- CreateOrderPage: purpose is sales order creation; fields include walk-in toggle, customer selector, payment status, order items, and notes; validation requires at least one valid line item.
- EditOrderPage: purpose is pending-order updates; fields include payment status, order items, notes, and shipping address; validation mirrors the create page but keeps the customer read-only.
- UpdatePaymentModal: purpose is payment tracking; fields include payment status and amount; validation enforces the unpaid, partially paid, and paid rules relative to the order total.
- All forms use local component state, manual validation, inline field errors, and mutation-driven success or error feedback. React Hook Form and Zod are not used.

# Tables

- ProductDataTable: columns are SKU, product and category, stock, price, cost, active status, and created date; it supports server-side pagination, sorting, search, view, edit, and delete actions.
- StockMovementHistoryTable: columns are date, product and SKU, movement type, quantity, reason, and processed by; it supports server-side pagination, sorting, and search.
- ProductCategoryDataTable: columns are ID, name, parent category, created date, and created by; it supports client-side search, sorting, view, edit, and delete.
- UnitOfMeasureTable: columns are ID, name, created date, and created by; it supports client-side search, sorting, view, edit, delete, and add-new actions.
- CustomerDataTable: columns are customer name and email, phone, category, total orders, total spent, active status, and created date; it supports server-side pagination, sorting, search, view, edit, and delete.
- CustomerCategoryDataTable: columns are name, type, description, and created date; it supports client-side search, view, edit, and delete.
- InventoryDataTable: columns are product and SKU, location, quantity, reorder, max, status, and potential profit; it supports server-side pagination, sorting, search, view, edit, and delete.
- LocationDataTable: columns are ID, name, address, location type, created date, and created by; it supports client-side search, view, edit, delete, and add-new actions.
- StockTransferDataTable: columns are product, from location, to location, quantity, status, user, and created date; it supports server-side pagination, sorting, search, and a read-only view action.
- OrdersDataTable: columns are order number, customer, order date, item count, total, order status, and payment status; it supports server-side pagination, status and customer filters, date range filters, row navigation, edit access for pending orders, payment update, and status transition actions.
- DataTable is the server-side table primitive that powers the more complex domain tables, while SimpleDataTable is the client-side primitive used for smaller catalog lists.
- Both table primitives support action menus, search debouncing, pagination controls, sorting indicators, and mobile-aware row behavior.

# API Integration

- Dashboard endpoints consumed by the frontend are /dashboard/summary, /dashboard/today-performance, /dashboard/top-selling-products, and /dashboard/inventory-alerts.
- Product endpoints consumed by the frontend are /products, /products/summary, /products/:id, /products/stock-movements-history, /products/:id/inventory, and the create, update, and delete routes.
- Product category endpoints consumed by the frontend are /product-categories, /product-categories/names, /product-categories/tree, /product-categories/main-categories, and the detail, create, update, and delete routes.
- Unit of measure endpoints consumed by the frontend are /unit-of-measures, /unit-of-measures/names, and the detail, create, update, and delete routes.
- Customer endpoints consumed by the frontend are /customers, /customers/summary, /customers/:id, and the create, update, and delete routes.
- Customer category endpoints consumed by the frontend are /customer-categories, /customer-categories/names, and the detail, create, update, and delete routes.
- Inventory endpoints consumed by the frontend are /inventory, /inventory/summary, /inventory/:id, and the create, update, and delete routes.
- Location endpoints consumed by the frontend are /locations, /locations/names, and the detail, create, update, and delete routes.
- Location type endpoints consumed by the frontend are /location-type and /location-type/:id, plus create for new location types.
- Stock transfer endpoints consumed by the frontend are /stock-transfers, /stock-transfers/:id, /stock-transfers/:id/status, and create.
- Sales order endpoints consumed by the frontend are /sales-orders, /sales-orders/summary, /sales-orders/:id, /sales-orders/:id/payment, and the confirm, transit, ship, complete, cancel, and return action routes.
- Auth endpoints consumed by the frontend are /auth/login, /auth/refresh-token, and /auth/logout.
- Realtime notifications are consumed from the SignalR hub at /hubs/notification.
- Query keys are centralized so each feature can invalidate exactly the relevant cache set after a mutation.
- The React Query configuration keeps recent results warm for 30 seconds, retains cache for 5 minutes, avoids mutation retries, and preserves previous table data during pagination changes.
- Error handling is centralized through the Axios client’s error extraction helper, and individual service methods normalize the different backend response shapes into success/error objects.
- Optimistic updates are not broadly used. The normal pattern is to perform the mutation, show a toast, and invalidate the affected queries so the UI refetches authoritative server state.

# Performance Optimizations

- React Query caching reduces repeat requests and keeps summaries, detail views, and table pages responsive.
- keepPreviousData is used for server-side tables so pagination and search changes do not flash empty states unnecessarily.
- Table column definitions are memoized so expensive column reconstruction is avoided on every render.
- The date formatter caches Intl.DateTimeFormat instances by locale and date mode.
- The DZD currency formatter caches Intl.NumberFormat instances by locale.
- Order item SKU search is debounced so typing does not trigger a request on every keystroke.
- Search and list queries are paginated on the server, which reduces payload size and keeps the data grid scalable.
- Mutation flows generally invalidate precise query keys instead of refetching the entire application.
- Dropdown placement for row action menus is measured against the viewport so the menu opens in a usable position even near the bottom of the page.
- No virtualization, route-level code splitting, or lazy-loaded feature modules were found in the current frontend.

# UX Improvements

- Loading skeletons are used on order detail and edit pages, and smaller loading indicators appear in tables, forms, and the auth flow.
- Toast notifications give immediate success and failure feedback after CRUD operations, order transitions, payment updates, and stock workflows.
- Confirmation dialogs protect destructive actions such as deletes and state transitions.
- The sidebar collapses into a mobile drawer, and the top nav compresses gracefully for small screens.
- Empty states are explicit in order items, alerts, tables, and placeholder routes so the UI never feels broken or ambiguous.
- The notification dropdown closes on outside click, and the table action menus close on outside click, Escape, pagination changes, and viewport changes.
- The language switcher is always reachable from the top nav, which makes the app easy to use across supported locales.
- Accessibility is considered in several places: toast containers use live regions, the language switcher has an accessible label, and notification actions include ARIA labels.
- The order form uses product and inventory lookups to prevent invalid line items before submission, which reduces backend rejection and user frustration.
- The transfer and inventory forms guide the user through required selections step by step rather than exposing every field at once.

# Technical Challenges Solved

- I implemented a single-flight token refresh flow so multiple expired requests do not trigger redundant refresh calls.
- I wired a status-transition matrix for sales orders so buttons, confirmation dialogs, row actions, and allowed workflow states stay in sync.
- I normalized backend order payloads and line items so list, detail, create, update, and payment screens can share a consistent frontend model.
- I kept product, inventory, and location caches synchronized after create, update, and delete operations so cross-feature counts stay accurate.
- I built a dynamic order item editor that searches by SKU or ID, loads product inventory per item, prevents duplicate product-location pairs, and clamps quantity to available stock.
- I added localized DZD formatting and cached date formatting so revenue, totals, and timestamps stay readable in all supported locales.
- I integrated SignalR low-stock alerts into the top navigation without blocking the rest of the app.
- I separated the main UI shell, shared primitives, feature modules, and API layer so the project stays maintainable as more modules are added.

# Deliverables

- Authentication UI with login, forgot-password placeholder, protected routing, and logout handling.
- Responsive application shell with sidebar navigation, top nav, language switching, search field, and notification bell.
- Dashboard overview with summary cards, today performance, top selling products, quick actions, and inventory alerts.
- Product catalog CRUD with detail dialog, category support, units of measure, stock movement history, and summary metrics.
- Inventory CRUD with stock levels, stock status, potential profit, and detail views.
- Stock location CRUD with location types and audit metadata.
- Stock transfer workflow with product lookup, route selection, history, and validation.
- Customer CRUD with address data, category assignment, summary views, and customer category management.
- Sales order listing, order creation, order editing, order detail viewing, payment updates, and status transitions.
- Reusable data table primitives for server-side and client-side lists.
- Reusable modal and dialog components for create, edit, view, and confirmation flows.
- Global toast notification system.
- Localized UI with English, French, and Arabic resources.
- Locale-aware date and DZD currency formatting.
- Realtime low-stock notifications via SignalR.
- Placeholder settings route and 404 page.

## Resume Keywords

React, JavaScript, JSX, Vite, React Router DOM, TanStack Query, TanStack Table, Axios, i18next, react-i18next, Tailwind CSS, Radix Dialog, shadcn/ui-style primitives, lucide-react, react-tabs, SignalR, CRUD, REST APIs, authentication, authorization, protected routes, session refresh, token management, server-side pagination, filtering, search, sorting, caching, query invalidation, manual validation, forms, modals, dialogs, tables, responsive design, reusable components, component library, toast notifications, loading states, skeleton loaders, empty states, error handling, localization, internationalization, currency formatting, date formatting, DZD formatting, language switching, Arabic, French, English, inventory management, product catalog, customer management, sales orders, stock transfers, stock locations, dashboard development, realtime notifications, performance optimization, accessibility, status workflows, action menus, detail views, summary cards, frontend architecture, layout system, UI shell, debounced search, dynamic line items, inventory status badges, payment updates, low-stock alerts.