import typescriptEslintParser from '@typescript-eslint/parser';
import typescriptEslintPlugin from '@typescript-eslint/eslint-plugin';
import vuePlugin from 'eslint-plugin-vue';
import prettierPlugin from 'eslint-plugin-prettier';
import vueEslintParser from 'vue-eslint-parser';
import unusedImports from 'eslint-plugin-unused-imports';

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
      prettier: prettierPlugin,
      'unused-imports': unusedImports,
    },
    rules: {
      '@typescript-eslint/explicit-function-return-type': ['error'],
      '@typescript-eslint/explicit-module-boundary-types': ['error'],
      '@typescript-eslint/no-var-requires': 'off',
      '@typescript-eslint/no-unused-vars': 'warn',
      'prefer-promise-reject-errors': 'off',
      'no-unused-vars': 'warn',
      'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
      'vue/script-setup-uses-vars': 'error',
      curly: ['warn', 'all'],
      quotes: ['warn', 'single', { avoidEscape: true }],
      'prettier/prettier': 'warn',
      'unused-imports/no-unused-imports': 'error',
    },
  },
];
