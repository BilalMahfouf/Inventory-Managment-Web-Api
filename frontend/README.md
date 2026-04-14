# Frontend

React + Vite frontend for the Inventory Management System.

## Local Development

1. Install dependencies:

	npm install

2. Run the app:

	npm run dev

## Production Build

Build the app:

npm run build

Preview the production build locally:

npm run preview

## Vercel Deployment

This project is configured for Vercel via `vercel.json`.

Required environment variable in Vercel Project Settings:

- `VITE_API_ORIGIN`: Public API origin (example: `https://api.example.com`)

After setting env vars, deploy normally with Vercel.
