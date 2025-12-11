import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/testing/vitest-setup.ts'],
    include: ['src/**/*.spec.ts'],
    exclude: ['node_modules', 'dist'],
    coverage: {
      provider: 'v8',
      reporter: ['html', 'text', 'lcov', 'json-summary'],
      reportsDirectory: './coverage/lightnap-ng',
      exclude: [
        'node_modules/',
        'dist/',
        'src/testing/**',
        '**/*.spec.ts',
        'src/environments/**',
      ],
      thresholds: {
        lines: 42,
        functions: 40,
        branches: 25,
        statements: 45,
      },
    },
  },
  resolve: {
    alias: {
      '@core': new URL('./src/app/core', import.meta.url).pathname,
      '@testing': new URL('./src/testing', import.meta.url).pathname,
    },
  },
});
