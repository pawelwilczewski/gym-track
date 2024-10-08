import typescriptEslintParser from '@typescript-eslint/parser'
import typescriptEslintPlugin from '@typescript-eslint/eslint-plugin'
import prettierConfig from 'eslint-config-prettier'
import vuePlugin from 'eslint-plugin-vue'
import vueEslintParser from 'vue-eslint-parser'

export default [
  {
    files: ['**/*.ts', '**/*.tsx', '**/*.vue'],
    languageOptions: {
      parser: vueEslintParser,
      parserOptions: {
        parser: typescriptEslintParser,
        extraFileExtensions: ['.vue'],
      },
    },
    plugins: {
      '@typescript-eslint': typescriptEslintPlugin,
      vue: vuePlugin,
    },
    rules: {
      curly: ['warn', 'all'],
      'prefer-promise-reject-errors': 'off',
      quotes: ['warn', 'single', { avoidEscape: true }],
      '@typescript-eslint/explicit-function-return-type': 'off',
      '@typescript-eslint/no-var-requires': 'off',
      'no-unused-vars': 'warn',
      '@typescript-eslint/no-unused-vars': 'warn',
      'vue/script-setup-uses-vars': 'error',
      'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    },
  },
]
