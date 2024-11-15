import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import './assets/index.css';
import '@/scripts/Math/RoundingExtensions';

createApp(App).use(router).mount('#app');
