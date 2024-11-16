import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import './assets/index.css';
import '@/scripts/math/RoundingExtensions';
import '@/scripts/styling/AutoUpdateBodyDarkClass';

createApp(App).use(router).mount('#app');
