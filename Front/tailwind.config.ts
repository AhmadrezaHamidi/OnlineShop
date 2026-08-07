import type { Config } from 'tailwindcss'

export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        pet: {
          50:  '#FFF8F0', 100: '#FFF3E4', 200: '#FDE8D4',
          300: '#F9C99E', 400: '#F0A86A', 500: '#E07B39',
          600: '#C4622A', 700: '#A34E20',
        },
        forest: {
          50: '#E6F7EE', 100: '#C3EACF',
          400: '#4CAF7D', 500: '#3A9668', 600: '#2D7A54',
        },
        honey: {
          50: '#FEF3DC', 400: '#F5C842', 500: '#F5A623', 600: '#D4891A',
        },
        ink: { 900: '#1A1A2E', 700: '#2D2D4A', 500: '#4A4A6A', 300: '#8888AA', 100: '#DDDDEF' },
        teal: {
          50: '#E1F5EE', 100: '#B3E5D3', 200: '#80D3B5',
          300: '#4DC197', 400: '#26B37E', 500: '#0F6E56',
          600: '#085041', 700: '#063D32',
        },
        amber: { 50: '#FFF8E7', 500: '#BA7517' },
        coral: { 50: '#FDEEE9', 500: '#993C1D' },
        kheir: {
          50: '#EAF3DE', 100: '#C0DD97', 200: '#97C459',
          400: '#639922', 500: '#3B6D11', 600: '#27500A', 700: '#173404',
        },
        gold: {
          50: '#FAEEDA', 200: '#EF9F27', 400: '#BA7517', 500: '#633806',
        },
      },
      fontFamily: { sans: ['Vazirmatn', 'system-ui', 'sans-serif'] },
      boxShadow: {
        pet:    '0 4px 20px rgba(224,123,57,.15)',
        'pet-lg': '0 8px 32px rgba(224,123,57,.22)',
      },
    },
  },
  plugins: [],
} satisfies Config
