import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5500,
    proxy: {
      '/api': { target: 'http://localhost:8080', changeOrigin: true },
      '/kheirkhabar-api': { target: 'http://localhost:5141', changeOrigin: true, rewrite: p => p.replace(/^\/kheirkhabar-api/, '/api') },
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/tests/setup.ts',
    include: ['src/tests/**/*.test.{ts,tsx}'],
  },
})
