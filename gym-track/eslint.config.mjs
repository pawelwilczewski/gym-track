import typescriptEslintParser from '@typescript-eslint/parser';
import typescriptEslintPlugin from '@typescript-eslint/eslint-plugin';
import vuePlugin from 'eslint-plugin-vue';
import prettierPlugin from 'eslint-plugin-prettier';
import vueEslintParser from 'vue-eslint-parser';
import unusedImports from 'eslint-plugin-unused-imports';
import typescriptEslint from 'typescript-eslint';
import unicorn from 'eslint-plugin-unicorn';
import eslint from '@eslint/js';

export default [
  eslint.configs.recommended,
  ...typescriptEslint.configs.recommended,
  ...vuePlugin.configs['flat/recommended'],
  unicorn.configs['flat/recommended'],
  {
    files: ['**/*.ts', '**/*.tsx', '**/*.vue'],
    languageOptions: {
      parser: vueEslintParser,
      parserOptions: {
        parser: typescriptEslintParser,
        extraFileExtensions: ['.vue'],
        project: [
          './tsconfig.json',
          './tsconfig.app.json',
          './tsconfig.node.json',
        ],
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
      '@typescript-eslint/switch-exhaustiveness-check': ['error'],
      '@typescript-eslint/no-var-requires': 'off',
      '@typescript-eslint/no-unused-vars': 'warn',
      'prefer-promise-reject-errors': 'off',
      'no-unused-vars': 'off',
      // eslint-disable-next-line no-undef
      'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
      'vue/script-setup-uses-vars': 'error',
      curly: ['warn', 'all'],
      quotes: ['warn', 'single', { avoidEscape: true }],
      'prettier/prettier': 'warn',
      'unused-imports/no-unused-imports': 'error',
      'vue/multi-word-component-names': 'off',
      'vue/singleline-html-element-content-newline': 'off',
      'vue/max-attributes-per-line': 'off',
      'vue/html-self-closing': 'off',
      'vue/html-indent': 'off',
    },
  },
];
