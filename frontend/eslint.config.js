import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import { defineConfig, globalIgnores } from 'eslint/config'

export default defineConfig([
    globalIgnores(['dist']),

    // React/browser files
    {
        files: ['**/*.{js,jsx}'],
        extends: [
            js.configs.recommended,
            reactHooks.configs['recommended-latest'],
            reactRefresh.configs.vite,
        ],
        languageOptions: {
            ecmaVersion: 'latest',
            globals: globals.browser,
            parserOptions: {
                ecmaFeatures: { jsx: true },
                sourceType: 'module', // ✅ browser code is ESM
            },
        },
        rules: {
            'no-unused-vars': ['error', { varsIgnorePattern: '^[A-Z_]' }],
        },
    },

    // Node/CommonJS config files
    {
        files: ['**/{tailwind,postcss}.config.js'],
        languageOptions: {
            ecmaVersion: 'latest',
            sourceType: 'commonjs',
            globals: globals.node,
        },
    },

    // ESM config files (vite, eslint)
    {
        files: ['**/{vite,eslint}.config.js'],
        languageOptions: {
            ecmaVersion: 'latest',
            sourceType: 'module', // ✅ treat them as ESM
            globals: globals.node,
        },
    },
])
